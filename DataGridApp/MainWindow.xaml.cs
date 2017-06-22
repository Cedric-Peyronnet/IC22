﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Resources;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControls;

namespace DataGridApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string sqlConnection = "datasource=192.168.6.196;port=3306;username=cedric;password=root";
        string sqlQuerry = "select* from sqlbrowsertest.iamgod ";
        
        List<int> aListWhitchContainTheColumnCheckBoxWithTrueOrFalseSQL;

       
        public MainWindow()
        {
            InitializeComponent();
            MyGrid.DeleteAllowed = true;
            //           LoadDataObsColl();
            // LoadDataFromDBF();
            LoadDataFromSQL();
            //     this.DataContext = new WpfControls.Menu.MainWindowViewModel();
            loadColor();
        }
        private void LoadDataObsColl()
        {

            CheckBox p = new CheckBox();
            var records = new ObservableCollection<Record>
            {
                new Record(new Property("FirstName", "Paul"), new Property("LastName", "15"), new Property("bonsoir", "Test")),
                new Record(new Property("FirstName", "Tony"), new Property("LastName", "-15"), new Property("bonsoir", "Test"))
            };
            int count = 0;
            foreach (var bonsoir in records)
            {
                CheckBox cb = new CheckBox();
                records[count].Properties.Insert(2, new Property("dsq", cb));
                // records[count].Properties.Add(new Property("dsq", cb));
                records[count].Properties.Add(new Property("fsdfsdf", cb));
                count++;
            }

            MyGrid.LoadData(records);


        }
        private void LoadDataListColl()
        {
            ListView ca = new ListView();
            var gridView = new GridView();
            ca.View = gridView;
            gridView.Columns.Add(new GridViewColumn { Header = "Id" });
            gridView.Columns.Add(new GridViewColumn { Header = "Id2" });

            //            MyGrid.LoadData(MyGrid.ListViewToOC1(ca));
        }
        private void LoadDataFromDBF()
        {
         //   MyGrid.LoadData(MyGrid.GetYourDataIntoDataGrid());
        }

        private void addColumnButton_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.addColumnButtonClick();
        }
        private void LoadDataFromSQL()
        {
            aListWhitchContainTheColumnCheckBoxWithTrueOrFalseSQL = new List<int> { 3 };
       //     aListIsRead = new List<int> { 2 };
            MyGrid.LoadDataFromSQL(aListWhitchContainTheColumnCheckBoxWithTrueOrFalseSQL, sqlConnection,sqlQuerry);            
        }
        private void loadColor()
        {
            
            var bmp = new System.Drawing.Bitmap(Properties.Resources.def);
            MyGrid.brushCur = System.Windows.Media.Brushes.Yellow ;
            MyGrid.brushValue1 = System.Windows.Media.Brushes.Red ;
            MyGrid.brushValue2 = System.Windows.Media.Brushes.Blue;
            MyGrid.value1 = 25;
            MyGrid.value2 = 25;
            // Column 3 will be display cell by cell a background color from previous value 
            MyGrid.listOfColumnChangeIntegerAsCellDetail = new List<int> {2};
            //Column 2 will display all the columns with a background color
            MyGrid.listOfColumnChangeAllCell = new List<int> { 1,3 };

            MyGrid.listOfString = new List<string> {"cedric","Dick" };

            MyGrid.listOfColumnForString = new List<int> { 0 };

            MyGrid.changeHeaderWithImage(2, bmp);
        }

    }
}
