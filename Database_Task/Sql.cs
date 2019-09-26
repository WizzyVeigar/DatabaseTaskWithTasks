using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Database_Task
{
    class Sql
    {
        private readonly string connectionString = @"Server = RO-SKP-01838; Database=SteamMarket; Integrated Security=true";
        SqlConnection conn;
        SqlCommand sqlCommand;
        SqlDataReader dataReader;
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt;

        public Sql()
        {
            conn = new SqlConnection(connectionString);
            dt = new DataTable();
        }

    

        public DataTable GetDb()
        {
            sqlCommand = new SqlCommand("Select * From SteamMarket_Name", conn);
            
           

            conn.Open();
            da.SelectCommand = sqlCommand;
            dataReader = sqlCommand.ExecuteReader();
            dataReader.Close();
            da.Fill(dt);
            conn.Close();
            da.Dispose();
            return dt;
        }

        public string[] GetVolumeAndAvg(string nameOfItem)
        {
            using (WebClient wc = new WebClient())
            {
                string[] steamString = null;
                string json = wc.DownloadString(@"https://steamcommunity.com/market/priceoverview/?currency=1&appid=570&market_hash_name=" + nameOfItem);
                //Euro string
                //string json = wc.DownloadString(@"https://steamcommunity.com/market/priceoverview/?currency=3&appid=570&market_hash_name=" + nameOfItem);
                //json.Replace(" ", "%20");
                JObject js = JObject.Parse(json);
                if (json.Contains("median_price"))
                {
                    Debug.WriteLine(js["volume"].ToString() + " " + js["median_price"].ToString());
                    steamString = new string[] { js["volume"].ToString(), js["median_price"].ToString() };
                }
                else if (!json.Contains("volume"))
                {
                    steamString = new string[] { "1", js["lowest_price"].ToString() };
                }
                return steamString;
                //string volumeString = js["volume"].ToString();
                //string medianPriceString = js["median_price"].ToString();
                //Debug.WriteLine(volumeString);
                //Debug.WriteLine(medianPriceString);
            }
        }

        internal void Hell(string itemName, string path)
        {
            string query = "Insert into SteamMarket_Name values('[@itemName]', (SELECT * FROM OPENROWSET(BULK N'"+path+"', SINGLE_BLOB) as Blob))";
            string nonQuery = "insert into SteamMarket_Name ([ItemName], [ImageName])" + " VALUES (@ItemName , (SELECT * FROM OPENROWSET(BULK N'" + path + "', SINGLE_BLOB) as Blob))";
            // 1. declare command object with parameter
            sqlCommand = new SqlCommand(nonQuery, conn);



            // 3. add new parameter to command object
            sqlCommand.Parameters.AddWithValue("@ItemName",itemName);
          //  sqlCommand.Parameters.AddWithValue("@ImageName",path);
          //  sqlCommand.Parameters.AddWithValue("@ImageName", path);

            
            //sqlCommand = new SqlCommand("INSERT INTO SteamMarket_Name values('" + itemName + "', (SELECT * FROM OPENROWSET(BULK N'" + path + "', SINGLE_BLOB) as Blob))", conn);
            conn.Open();
            sqlCommand.ExecuteNonQuery();
            conn.Close();
        }

        public bool DoesItemExist(string itemName)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string json = wc.DownloadString(@"https://steamcommunity.com/market/priceoverview/?currency=1&appid=570&market_hash_name=" + itemName);
                    JObject js = JObject.Parse(json);
                    string jsonString = js["success"].ToString();
                    if (jsonString.Contains("True"))
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}
