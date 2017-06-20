using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Data;

namespace WpfControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MyDataGrid : UserControl
    {
        public bool updateColor { get; set; }
        public bool updateANewCell { get; set; }
        public bool writeInTheCell { get; set; }
        public bool DeleteAllowed { get; set; }

        ObservableCollection<Record> records;

        public static CultureInfo CurrentCulture { get; set; }

        public MyDataGrid()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loading a datagrid from an observable collection
        /// </summary>
        /// <param name="records"></param>
        public void LoadData(ObservableCollection<Record> records)
        {
          
            //first extract the columns from the collection and bind them to the grid
            var columns = records.First().Properties.Select((x, i) => new { Name = x.Name, Index = i }).ToArray();
            foreach (var column in columns)
            {
                var binding = new Binding($"Properties[{column.Index}].Value");                 
                Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = column.Name, Binding = binding });
             
            }

            //second the records themselves

            //Replace the "Text checkBox into a real checkBox" Redefine a binding
            for(int i = 0; i < records.Count;i++)
            {
                for(int j = 0; j < records[i].Properties.Count(); j++)
                {                  
                    if (records[i].Properties[j].Value is CheckBox  )
                    {
                        string nameOfBox = records[i].Properties[j].Name;
                        Ic2DataGrid.Columns.RemoveAt(j);
                        var binding = new Binding($"Properties[{j}].Value");
                        CheckBox cb = new CheckBox();
                        cb.Name = nameOfBox;
                        DataGridCheckBoxColumn dg = (new DataGridCheckBoxColumn { Header = cb.Name, Binding = binding });
                        Ic2DataGrid.Columns.Insert(j, dg);
                    }
                }                      
            }
            GenerateMenu();
            Ic2DataGrid.ItemsSource = records;
            
            if (!DeleteAllowed) 
            {
                Delete.Visibility = Visibility.Collapsed;                
            }            
        }
      
        public void LoadDataFromSQL()
        {
            string constring = "datasource=localhost;port=3306;username=root;password=root";
            MySqlConnection conDataBase = new MySqlConnection(constring);
            string sql = "select * from test.imagod ;";
            MySqlCommand cmdDataBase = new MySqlCommand(sql, conDataBase);

            try
            {
                MySqlDataAdapter sda = new MySqlDataAdapter(cmdDataBase);
              
                DataTable dbDataTable = new DataTable();

                sda.Fill(dbDataTable);

                //     dbdataset = sda.Fill(dbdataset);

                for (int index = 0; index < dbDataTable.Columns.Count; index++)
                {
                    var binding = new Binding($"Properties[{index}].Value");
                    Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = dbDataTable.Columns[index].ColumnName, Binding = binding });
                }
                
              
                Ic2DataGrid.ItemsSource = dbDataTable.DefaultView;
                
          
                // Source
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            /*
                DataSet dbdataset = new DataSet();
                sda.Fill(dbdataset, "test.iamgod");
              */
        }
        // event for the  menu ,currently not working     
        private void MenuItemDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteAllowed)
            {
                records = (ObservableCollection<Record>)Ic2DataGrid.ItemsSource;

                //Get the clicked MenuItem
                var menuItem = (MenuItem)sender;
    
                //Get the ContextMenu to which the menuItem belongs
                var contextMenu = (ContextMenu)menuItem.Parent;

                //Find the placementTarget
                var item = (DataGrid)contextMenu.PlacementTarget;

                //Get the underlying item, that you cast to your object that is bound
                //to the DataGrid (and has subject and state as property)
                // -1 Because we start at index 0
                if (item.SelectedIndex != -1)
                {
                    var toDeleteFromBindedList = (Record)item.SelectedCells[0].Item;

                    //Remove the toDeleteFromBindedList object from your ObservableCollection
                    records.Remove(toDeleteFromBindedList);
                }
            }
            else
            {
                Delete.Visibility = Visibility.Hidden;
            }
        }
        private void MenuItemAdd_click(object sender, RoutedEventArgs e)
        {
            string header;
            AddMenuButtonInformation add = new AddMenuButtonInformation();
            add.ShowDialog();
            if(add.DialogResult ==true)
            {
                header = add.AddColumnTitle.Text;
                var binding = new Binding($"Properties[{ 1}].Value");
                Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = header, Binding = binding });
                add.Close();
            }
            else
            {
                add.Close();
            }

        }
        private void MenuItemDeleteColumn_Click(object sender, RoutedEventArgs e)
        {

        }

        //Generate a menutop 

        private void GenerateMenu() {
        

        }


        //Methode to get an ObservableCollection from a list. The method isn't finished 100%

        public ObservableCollection<Record> ListViewToOC1(ListView list)
        {
            ObservableCollection<Record> records = new ObservableCollection<Record>();
     
            GridView gridView = new GridView();

            gridView = (GridView)list.View;
            // one list which contains headers titles and second contains items
            List<string> headerList = new List<string>();
            List<string> itemList = new List<string>();
            // A record 
            Record r;
            // Array of property 
            Property[] p = new Property[] {null};
            int columnNumberOfCurrentListView = ((GridView)list.View).Columns.Count;
            int listViewLenght = list.Items.Count;

            //  Add Header name during the first loop
            for (int i = 0; i <= listViewLenght; i++)
            {
                //Header collection + first line 
                if (i == 0)
                {  
                    for (int j = 0; j < columnNumberOfCurrentListView; j++)
                    {
                        
                        string header = ((GridView)list.View).Columns[j].Header.ToString();

                        headerList.Add(header);
                    }
                }
                //Line 2 to X
                //The collection has (Column * row ) items
                else
                {
                    for (int j = 0; j < columnNumberOfCurrentListView * listViewLenght ; j++)
                    {
                        string item = ((GridView)list.View).Columns[j].Header.ToString();
                    }   
                }
            }
            p = new Property[headerList.Count];
            r = new Record();
            //Adding column + item
            for (int i = 0; i < headerList.Count; i++)
            {
                for (int j = 0; j <= 0; j++)
                {
                    //Add a new property with the column Header and the value 
                    if (j == 0)
                    {
                        p[i] = (new Property(headerList[i], "0"));
                    }else
                    {

                    }                   
                }
            }
            r = new Record(p);
            records.Add(r);
            return records;

        }

        // events for editing
        /// <summary>
        /// if double click into a cell go on edit mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Make update available 
            Ic2DataGrid.IsReadOnly = false;
        }
        //If someone change the focus of the  currentCell by clicking somewhere else,it will change the readonly on true.
        private void Ic2DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            Ic2DataGrid.IsReadOnly = false;
            StartColor();
            Ic2DataGrid.IsReadOnly = true;
        }

        /// <summary>
        /// event if someone is in editing mod and he presses enter then ok will make change in DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            updateANewCell = true;
            StartColor();
            var cell = Ic2DataGrid.CurrentCell;
            if (e.Key == Key.Enter)
            {
                EditCheck edit = new EditCheck();
                edit.CheckPopupLabel.Content = "Do you want to update the current cell ?";
                edit.ShowDialog();
                if (edit.DialogResult.Value)
                {
                    //Get information of cell to manage into sql
                    var selectedRow = moduleHelper.GetSelectedRow(Ic2DataGrid);
                    var columnIndex = cell.Column.DisplayIndex;
                    writeInTheCell = true;
                    edit.Close();
            
                    // Make a case for column could to get a dynamic value
                    switch (columnIndex)
                    {
                        //Replace the value into the content with the column selected.
                        case 1:
                            DataGridCell dgc = Ic2DataGrid.GetCell(selectedRow, columnIndex);
                            Record records = (Record)dgc.DataContext;
                            string subStringValue = dgc.ToString();
                            //Delete the content System.window ... 
                            subStringValue = subStringValue.Substring(37);
                            // Check if the enter is a numeric or not return Message Box if it's nt a numeric value
                            int nResult;
                            if (int.TryParse(subStringValue, out nResult) == false)
                            {
                                MessageBox.Show("Not a correct entry !");
                                Ic2DataGrid.IsReadOnly = true;
                                break;
                            }
                            else
                            {
                                //Maybe can be change. We get only the value of the cell with a substring
                                records.Properties[1].Value = subStringValue;
                                //Call the methode to change color after an update   
                                changeColor(Ic2DataGrid.CurrentCell, e);
                                break;
                            }
                    }                                  
                }
                else
                {
                    Ic2DataGrid.CurrentCell = cell;
                    Ic2DataGrid.IsReadOnly = true;
                    writeInTheCell = true;
                    edit.Close();
                }
            }
            else
            {
                updateANewCell = true;
            }
            
        }
        //Prevent from someone click outside from the datagrid
        
        /// <summary>
        /// If someone click outside the cell which he update, that will cancel the readonly he made 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseClick(object sender, MouseButtonEventArgs e)
        {
            Ic2DataGrid.IsReadOnly = false;    
            if (updateANewCell == true)
            {
                Ic2DataGrid.IsReadOnly = true;
                updateANewCell = false;
            }         
        }

        // All these methodes/events are for the background color 
        
        /// <summary>
        /// change the color when someone update a cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeColor(object sender, RoutedEventArgs e)
        {
            DataGridCellInfo dgci = Ic2DataGrid.CurrentCell;
            DataGridCell dgc = GetDataGridCell(dgci);
            ValueToBrushConverter VTC = new ValueToBrushConverter();
            VTC.Convert(dgc, e.GetType(), Color.FromRgb(0, 0, 0), CurrentCulture);
        }

        /// <summary>
        /// Methode to start the update
        /// </summary>      
        public void StartColor()
        {
            //ColorLoad(Ic2DataGrid);
        }

        /// <summary>
        /// This event start after the main and do everything what we need after.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ic2DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            StartColor();
        }

        /// <summary>
        /// That load and update the background color
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>      
     /*   private object ColorLoad(DataGrid value)
        {
          int input;
            int negativOrPositivOrZero;
            try
            {
                DataGrid dg = value;

                //Number of record
                int numberOfRecord = dg.Items.Count;

                //For every column you get is going to check value of integer of the row 
                //You have to insert the column where the color should be changed / applyed
                for (int i = 0; i < dg.Columns.Count; i++)
                {
                   
                    //Add case to have an other column dynamic color
                  /*  switch (i)
                    {
                        case 1:
                            for (int j = 0; j < numberOfRecord; j++)
                            {
                                
                                Record rc = (Record)dg.Items[j];

                                DataGridRow dgr = moduleHelper.GetRow(dg, j);

                                DataGridCell dgc = moduleHelper.GetCell(dg, dgr, i);
                                string a = rc.Properties[i].Value.ToString();

                                input = int.Parse(a);
                                if (input > 0)
                                {
                                    negativOrPositivOrZero = 1;
                                }
                                else if (input == 0)
                                {
                                    negativOrPositivOrZero = 0;
                                }
                                else
                                {
                                    negativOrPositivOrZero = -1;
                                }
                                switch (negativOrPositivOrZero)
                                {
                                    case 1:
                                        dgc.Background = Brushes.Green;
                                        break;
                                    case 0:
                                        dgc.Background = Brushes.Orange;
                                        break;
                                    case -1:
                                        dgc.Background = Brushes.Red;
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            catch (InvalidCastException e)
            {
                Console.Write(e.ToString());
                return DependencyProperty.UnsetValue;
            }
            return DependencyProperty.UnsetValue;
        }*/

            /// <summary>
            /// This event check he has to update the layout color background. It should be disablee if we don't want a background color 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
        private void AnUserControl_LayoutUpdated(object sender, EventArgs e)
        {
            if (updateColor)
            {
                StartColor();
            }

            updateColor = false;
        }
  
        /// <summary>
        /// Change boolean value to update the layout 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>  
        private void columnHeader_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            if (columnHeader != null)
            {
                updateColor = true;
            }
        }

        /// <summary>
        /// Convert DataGridCellInfo into DataGridCell
        /// </summary>
        /// <param name="cellInfo"></param>
        /// <returns></returns>      
        public DataGridCell GetDataGridCell(DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);
            if (cellContent != null)
                return (DataGridCell)cellContent.Parent;
            return null;
        }


        /// <summary>
        /// Methode to get value of a table in DBF file.
        /// </summary>
        /// <returns Return a data table who is used in GetYourDataUbtoDataGrid()></returns>
        public DataTable GetYourData()
        {
            DataTable YourResultSet = new DataTable();

            OleDbConnection yourConnectionHandler = new OleDbConnection(
                @"Provider=VFPOLEDB.1;Data Source=C:\Users\rishe\Desktop\Project Kleijn bBrowser replace\WpfControls\WpfControls");

            // if including the full dbc (database container) reference, just tack that on
            //      OleDbConnection yourConnectionHandler = new OleDbConnection(
            //          "Provider=VFPOLEDB.1;Data Source=C:\\SomePath\\NameOfYour.dbc;" );


            // Open the connection, and if open successfully, you can try to query it
            yourConnectionHandler.Open();

            if (yourConnectionHandler.State == ConnectionState.Open)
            {
                string mySQL = "select * from imatest";  // dbf table name

                OleDbCommand MyQuery = new OleDbCommand(mySQL, yourConnectionHandler);
                OleDbDataAdapter DA = new OleDbDataAdapter(MyQuery);

                DA.Fill(YourResultSet);

                yourConnectionHandler.Close();
            }
            return YourResultSet;
        }

        /// <summary>
        /// Methode to generate a ObservableColletion usable in LoadData
        /// </summary>
        /// <returns>Return a Observable Colletion usable in load data</returns>
        public ObservableCollection<Record> GetYourDataIntoDataGrid()
        {        
            DataTable dr = GetYourData();
            ObservableCollection<Record> DBFRecords = new ObservableCollection<Record>();
            for(int i = 0; i< dr.Rows.Count; i++) 
            {
                Record rc = new Record(); 
                for (int j = 0; j < dr.Columns.Count; j++) {
                    DataRow dtr = dr.Rows[i];
                    var t = dtr[j];
                    string columnName = dr.Columns[j].ColumnName;
                    rc.Properties.Add(new Property(columnName, t));                
                }
                DBFRecords.Add(rc);
            }
            return DBFRecords;
        }
    }
}