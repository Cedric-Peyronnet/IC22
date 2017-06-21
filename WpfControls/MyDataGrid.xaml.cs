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
using System.Data;
using MySql.Data.MySqlClient;

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
            for (int i = 0; i < records.Count; i++)
            {
                for (int j = 0; j < records[i].Properties.Count(); j++)
                {
                    if (records[i].Properties[j].Value is CheckBox)
                    {
                        string nameOfBox = records[i].Properties[j].Name;
                        Ic2DataGrid.Columns.RemoveAt(j);
                        var binding = new Binding($"Properties[{j}].Value");
                        CheckBox cb = new CheckBox();
                        cb.Name = nameOfBox;
                        DataGridCheckBoxColumn dg = new DataGridCheckBoxColumn { Header = cb.Name, Binding = binding };
                        Ic2DataGrid.Columns.Insert(j, dg);
                    }
                }
            }
        
            Ic2DataGrid.ItemsSource = records;

            if (!DeleteAllowed)
            {
                Delete.Visibility = Visibility.Collapsed;
            }
        }

        // ??
        private void MenuItemDeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteAllowed)
            {

            }
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
            // Ic2DataGrid.IsReadOnly = true;
        }

        /// <summary>
        /// event if someone is in editing mod and he presses enter then ok will make change in DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_PreviewKeyDown1(object sender, KeyEventArgs e)
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
        //Prevent from someone click outside from the datagrid

        /// <summary>
        /// If someone click outside the cell which he update, that will cancel the readonly he made 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseClick(object sender, MouseButtonEventArgs e)
        {
            Ic2DataGrid.IsReadOnly = false;
            if (updateANewCell)
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
        /// This event check he has to update the layout color background. It should be disablee if we don't want a background color 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnUserControl_LayoutUpdated(object sender, EventArgs e)
        {
            /*    if (updateColor)
                 {
                     StartColor();
                 }

                 updateColor = false;*/
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
                
        //Call addcolumn method when click on button
        private void addingColumn_Click(object sender, RoutedEventArgs e)
        {
            addColumnButtonClick();
        }

        //Call addrow method when click on button
        private void addRowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddRowWindow arw = new AddRowWindow();
            arw.ShowDialog();
            //test
            //retest
        }

        ////////////---------------SQL PART------------------///////////////////////////////////

        /// <summary>
        ///Loading a datagrid from a database Param :
        ///(CheckBoxList witch contain the column of checkbox, sqlConnection line to make a connection , sqlQuerry the querry for the sql database)
        /// </summary>
        public void LoadDataFromSQL(List<int> CheckBoxList, string sqlConnection, string sqlQuerry)
        {
            //connection
            MySqlConnection conDataBase = new MySqlConnection(sqlConnection);
            //Sql query to Load
            MySqlCommand cmdDataBase = new MySqlCommand(sqlQuerry, conDataBase);
            try
            {
                MySqlDataAdapter sda = new MySqlDataAdapter(cmdDataBase);
                //Data table to store the information
                DataTable dbDataTable = new DataTable();
                //Fill the inforamation into the datatable
                sda.Fill(dbDataTable);
                //Binding the information 
                for (int index = 0; index < dbDataTable.Columns.Count; index++)
                {
                    //Binding information (CheckBox part) 
                    if (CheckBoxList.Contains(index))
                    {
                        var binding = new Binding($"{dbDataTable.Columns[index].ToString()}");
                        Ic2DataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = dbDataTable.Columns[index].ColumnName, Binding = binding });
                    }
                    //Binding information (
                    else
                    {
                        var binding = new Binding($"{dbDataTable.Columns[index].ToString()}");
                        Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = dbDataTable.Columns[index].ColumnName, Binding = binding });
                    }

                }
                //Insert the information into itemsource 
                Ic2DataGrid.ItemsSource = dbDataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void addColumnButtonClick()
        {
            string header;
            AddMenuButtonInformation add = new AddMenuButtonInformation();
            add.AddTextBoxLabel.Content = "Enter column name :";
            add.addOneElementWindow.Title = "Add a column";
            add.AddColumnTitle.Text = "Enter column name";
            add.ShowDialog();
            if (add.DialogResult == true)
            {
                header = add.AddColumnTitle.Text;
                addColumn(header);
                add.Close();
            }
            else
            {
                add.Close();
            }
        }

        public void addColumn(string columnHeader)
        {
            int index = Ic2DataGrid.Columns.Count;
            Binding binding = new Binding($"{columnHeader}");
           
            Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = columnHeader, Binding = binding });
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
                    
                    var selectedRow = moduleHelper.GetSelectedRow(Ic2DataGrid);
                    var columnIndex = cell.Column.DisplayIndex;
                    writeInTheCell = true;
                    edit.Close();

                    DataGridCell dgc = Ic2DataGrid.GetCell(selectedRow, columnIndex);
                    string dgcs =  dgc.Content.ToString().Substring(33);
                    
                    // Make a cell for column could to get a dynamic value
                    switch (columnIndex)
                    {
                        //Replace the value into the content with the column selected.
                        case 85:
                            // Check if the enter is a numeric or not return Message Box if it's nt a numeric value
                            int nResult;
                            if (int.TryParse(dgcs, out nResult) == false)
                            {
                                MessageBox.Show("Not a correct entry !");
                                Ic2DataGrid.IsReadOnly = true;
                                break;
                            }
                            else
                            {
                                //Maybe can be change. We get only the value of the cell with a substring
//                                recordsOfDataContext.Properties[1].Value = subStringValue;

                                //Call the methode to change color after an update   
                                changeColor(Ic2DataGrid.CurrentCell, e);
                                break;
                            }
                        default:
                            break;
                    }
                    Ic2DataGrid.IsReadOnly = false;
                    dgc.Content = dgcs;
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