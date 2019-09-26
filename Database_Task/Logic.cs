using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Database_Task
{
    class Logic
    {
        Sql sql = new Sql();
        private string indexByName = null;

        public string IndexByName
        {
            get { return indexByName; }
            set { indexByName = value; }
        }

        DataTable logicDT;
        List<string> nameList = new List<string>();

        public void UpdateDataRow()
        {
            if(logicDT != null)
                logicDT.Clear();
            if (nameList.Count != 0)
                nameList.Clear();
            logicDT = sql.GetDb();
        }

        public List<string> FillDropDown()
        {
            //UpdateDataRow();
            
            foreach (DataRow row in logicDT.Rows)
            {
                nameList.Add(row[1].ToString());
            }
            //sql.GetSteamInfo();
            return nameList;
        }

        public ImageSource UpdatePicture(string updatedIndex)
        {
            //UpdateDataRow();
            foreach (DataRow row in logicDT.Rows)
            {
                if (row[1].ToString() == updatedIndex)
                {
                    byte[] binarydata = (byte[])row[2];
                    return ConvertToImage(binarydata);
                }
            }
            return null;
        }

        public ImageSource ConvertToImage(byte[] binaryData)
        {
            MemoryStream ms = new MemoryStream(binaryData);

            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = ms;
            imageSource.EndInit();

            return imageSource;
        }

        public void GetSteamInfo(string nameOfItem, out string volumeOut, out string priceOut)
        {
            string[] temp = sql.GetVolumeAndAvg(nameOfItem);
            volumeOut = temp[0];
            priceOut = temp[1];
        }
        
        public bool DoesItemExist(string itemName)
        {
            return sql.DoesItemExist(itemName);
        }

        internal void HighwayToHell(string itemName, string path)
        {
            sql.Hell(itemName, path);
        }
    }
}
