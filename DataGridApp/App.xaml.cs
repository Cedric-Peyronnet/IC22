using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DataGridApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

       
        public class filling {
            public WpfControls.DataStoring ds = new WpfControls.DataStoring();
            public void fillInformation()
            {
                /// list...[0][x]             
                ds.listOfBrusheData.Add(new List<Brush> { Brushes.Red,Brushes.Pink,Brushes.Yellow });

                ds.listOfStringData.Add(new List<string> {"cedric","Dick" });

                ds.listOfColumnIndexer.Add(new List<int> { 0 });              
              
                ///list...[1][x]
                ds.listOfStringData.Add(new List<string> { "peyronnet", "Dick" });

                ds.listOfColumnIndexer.Add(new List<int> { 1 });

                ds.listOfBrusheData.Add(new List<Brush> { Brushes.Blue });

                ///list... [2][x]                
                ds.listOfColumnIndexer.Add(new List<int> { 3 });

                ///list... [3][x]
                ds.listOfColumnIndexer.Add(new List<int> { 2 });

                ds.tableNameSQL = "html5webnlkleijn.iamgod";
            }
        }
    }
}