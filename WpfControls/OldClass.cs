using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControls
{
    class OldClass
    {
        //Modify the text of the window which will be open infonction of what element you will add
        //Add a column in the actual record and in the datagrid
        public void addColumnOld(String columnHeader)
        {
            records = (ObservableCollection<Record>)Ic2DataGrid.ItemsSource;

            int count = 0;
            foreach (var bonsoir in records)
            {
                records[count].Properties.Add(new Property(columnHeader, ""));
                count++;
            }
            int columnNumber = Ic2DataGrid.Columns.Count();

            var binding = new Binding($"Properties[{columnNumber}].Value");

            Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = columnHeader, Binding = binding });*/

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
            Property[] p = new Property[] { null };
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
                    for (int j = 0; j < columnNumberOfCurrentListView * listViewLenght; j++)
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
                    }
                    else
                    {

                    }
                }
            }
            r = new Record(p);
            records.Add(r);
            return records;

        }

        /// <summary>
        /// That load and update the background color
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>      
        private object ColorLoad(DataGrid value)
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
                    switch (i)
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
            for (int i = 0; i < dr.Rows.Count; i++)
            {
                Record rc = new Record();
                for (int j = 0; j < dr.Columns.Count; j++)
                {
                    DataRow dtr = dr.Rows[i];
                    var t = dtr[j];
                    string columnName = dr.Columns[j].ColumnName;
                    rc.Properties.Add(new Property(columnName, t));
                }
                DBFRecords.Add(rc);
            }
            return DBFRecords;
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

                    DataGridCell dgc = Ic2DataGrid.GetCell(selectedRow, columnIndex);
                    Record recordsOfDataContext = (Record)dgc.DataContext;

                    string subStringValue = dgc.ToString();

                    //Delete the content System.window ... 
                    subStringValue = subStringValue.Substring(37);

                    // Make a cell for column could to get a dynamic value
                    switch (columnIndex)
                    {
                        //Replace the value into the content with the column selected.
                        case 1:
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
                                recordsOfDataContext.Properties[1].Value = subStringValue;

                                //Call the methode to change color after an update   
                                changeColor(Ic2DataGrid.CurrentCell, e);
                                break;
                            }
                        default:
                            break;
                    }
                    Ic2DataGrid.IsReadOnly = false;

                    recordsOfDataContext.Properties[columnIndex].Value = subStringValue;


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
    }
}
