using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using WpfControls;

namespace DataGridApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyGrid.DeleteAllowed = true;
               LoadDataObsColl();
            // LoadDataFromDBF();
         //   LoadDataFromSQL();
       //     this.DataContext = new WpfControls.Menu.MainWindowViewModel();
        }
        private void LoadDataObsColl()
        {
           
            CheckBox p =  new CheckBox();
            var records = new ObservableCollection<Record>
            {
                new Record(new Property("FirstName", "Paul"), new Property("LastName", "15"), new Property("bonsoir", "Test")),
                new Record(new Property("FirstName", "Tony"), new Property("LastName", "-15"), new Property("bonsoir", "Test"))
            };           
            int count = 0;
            foreach (var bonsoir in records)
            {               
                CheckBox cb = new CheckBox();
                records[count].Properties.Insert(2,new Property("dsq", cb));
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
            MyGrid.LoadData(MyGrid.GetYourDataIntoDataGrid());
        }

        private void addColumnButton_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.addColumnButtonClick();
        }
         private void LoadDataFromSQL()
        {
            MyGrid.LoadDataFromSQL();
        }

    }
}
