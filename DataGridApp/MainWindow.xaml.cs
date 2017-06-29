using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DataGridApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        App.filling fill = new App.filling();
        string sqlConnection = "Server=84.246.4.143;port=9131;database=html5webnlkleijn;username=html5webnlkltest;password=testtest1";
        string sqlQuerry = "select* from html5webnlkleijn.iamgod";
        
    //    string sqlConnection = "datasource=192.168.6.196;port=3306;username=cedric;password=root";
    //    string sqlQuerry = "select* from sqlbrowsertest.iamgod ";

        List<int> aListWhitchContainTheColumnCheckBoxWithTrueOrFalseSQL;

        public MainWindow()
        {
            InitializeComponent();

            MyGrid.DeleteAllowed = true;
            fillInformation();
            LoadDataFromSQL();
            //     this.DataContext = new WpfControls.Menu.MainWindowViewModel();
        }
        private void addColumnButton_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.addColumnButtonClick();

        }
        private void LoadDataFromSQL()
        {
            aListWhitchContainTheColumnCheckBoxWithTrueOrFalseSQL = new List<int> {3, 5 };
       //     aListIsRead = new List<int> { 2 };
            MyGrid.LoadDataFromSQL(aListWhitchContainTheColumnCheckBoxWithTrueOrFalseSQL, sqlConnection,sqlQuerry);            
        }
        private void LoadImage()
        {        
            MyGrid.changeHeaderWithImage(2, "def");
            MyGrid.changeHeaderWithImage(3, "red");
        }

        private void saveExel_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.toExel();
        }

        private void fillInformation()
        {   
            fill.fillInformation();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadImage();

            MyGrid.changeColorAColumnString(fill.ds.listOfBrusheData[0][0],fill.ds.listOfColumnIndexer[0], fill.ds.listOfStringData[0]);

            MyGrid.changeColorAColumnString(fill.ds.listOfBrusheData[0][0], fill.ds.listOfColumnIndexer[1], fill.ds.listOfStringData[1]);

            MyGrid.changeColorAColumn(fill.ds.listOfBrusheData[0][0], fill.ds.listOfColumnIndexer[2]);

            MyGrid.changeColorColumnCellDetailInteger(fill.ds.listOfColumnIndexer[3], 25, 25, fill.ds.listOfBrusheData[0][2], fill.ds.listOfBrusheData[0][1]);
        }

        private void Grid_LayoutUpdated(object sender, System.EventArgs e)
        {
            MyGrid.changeColorAColumnString(fill.ds.listOfBrusheData[0][0], fill.ds.listOfColumnIndexer[0], fill.ds.listOfStringData[0]);

            MyGrid.changeColorAColumnString(fill.ds.listOfBrusheData[0][0], fill.ds.listOfColumnIndexer[1], fill.ds.listOfStringData[1]);

            MyGrid.changeColorAColumn(fill.ds.listOfBrusheData[0][0], fill.ds.listOfColumnIndexer[2]);

            MyGrid.changeColorColumnCellDetailInteger(fill.ds.listOfColumnIndexer[3], 25, 25, fill.ds.listOfBrusheData[0][2], fill.ds.listOfBrusheData[0][1]);

            MyGrid.setDBBooleanValue();
        }
    }
}
