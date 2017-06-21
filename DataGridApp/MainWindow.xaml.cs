using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
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
        
        List<int> aListWitchContainTheColumnCheckBoxWithTrueOrFalseSQL;

       
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
            aListWitchContainTheColumnCheckBoxWithTrueOrFalseSQL = new List<int> { 3 };
       //     aListIsRead = new List<int> { 2 };
            MyGrid.LoadDataFromSQL(aListWitchContainTheColumnCheckBoxWithTrueOrFalseSQL, sqlConnection,sqlQuerry);            
        }
        private void loadColor()
        {
            MyGrid.brushCur = Brushes.Black ;
            MyGrid.brushValue1 = Brushes.Red ;
            MyGrid.brushValue2 = Brushes.Green;
            MyGrid.value1 = 25;
            MyGrid.value2 = 25;
            MyGrid.listOfColumnChangeIntegerAsCellDetail = new List<int> {2};
        }

    }
}
