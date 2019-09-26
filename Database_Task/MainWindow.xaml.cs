using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Sql;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace Database_Task
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Stopwatch stopwatch = new Stopwatch();
        Logic logic = new Logic();
        string path = null;
        public MainWindow()
        {
            InitializeComponent();
            logic.UpdateDataRow();
            GetDbNames();
            ShowPicture();
        }

        private void GetDbNames()
        {
            dropDownBox.Items.Clear();
            foreach (string item in logic.FillDropDown())
            {
                dropDownBox.Items.Add(item);
            }
            //dropDownBox.SelectedItem = dropDownBox.Items[0];
        }

        private ImageSource ShowPicture()
        {
            ImageSource returnPic = null;
            if ((string)dropDownBox.SelectedValue != logic.IndexByName)
            {
                logic.IndexByName = dropDownBox.SelectedValue.ToString();
                 returnPic = logic.UpdatePicture((string)dropDownBox.SelectedValue);
                
            }return returnPic;
        }

        private void DropDownBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stopwatch.Start();
            Debug.WriteLine("start doing stuff:" + stopwatch.Elapsed);
            string volume;
            string price;
            Debug.WriteLine(dropDownBox.SelectedValue.ToString());

            logic.GetSteamInfo(dropDownBox.SelectedValue.ToString(), out volume, out price);

            //everything that is visible
            apiTextBox_Name.Content = dropDownBox.SelectedValue.ToString();
            apiTextBox_Amount.Content = volume;
            apiTextBox_AvgPrice.Content = price;
            imageBox.Source = logic.UpdatePicture(dropDownBox.SelectedValue.ToString());
            ShowPicture();
            stopwatch.Stop();
            Debug.WriteLine("Done doing stuff:" + stopwatch.Elapsed);
        }

        private void SearchForPicture(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files(*.png;)|*.png;";
            if (openFileDialog.ShowDialog() == true)
                path = openFileDialog.FileName;
            Debug.WriteLine(path);
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (path != null)
            {
                if (logic.DoesItemExist(submitNameBox.Text))
                {
                    logic.HighwayToHell(submitNameBox.Text, path);
                    logic.UpdateDataRow();
                    //dropDownBox.Items.Add(submitNameBox.Text);
                    GetDbNames();
                }
                else
                    submitNameBox.Text = "Something went wrong, try again!";

            }
        }
    }
}
