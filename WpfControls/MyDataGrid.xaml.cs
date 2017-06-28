using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

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

        // Everything here have to be initialize in the main
        public Brush tempBrush { get; set; }

        //Brush and values List storage
        private static List<int> listValues = new List<int>();

        private static List<Brush> listBrush = new List<Brush>();

        public List<int> listOfColumnChangeIntegerAsCellDetail = new List<int>();

        public List<int> listOfColumnChangeAllCell = new List<int>();

        private static List<string> listOfString = new List<string>();

        public List<int> listOfColumnForString = new List<int>();

        public MySqlConnection mySqlConnection = new MySqlConnection("Server=84.246.4.143;port=9131;database=html5webnlkleijn;username=html5webnlkltest;password=testtest1");
        public MySqlCommand mySqlCommand = new MySqlCommand();
        public MySqlDataReader reader;

        public static CultureInfo CurrentCulture { get; set; }

        public MyDataGrid()
        {
            InitializeComponent();
        }

        // Method which delete a column in the dataGrid and in the database, but for the moment,
        //only if you right click on the cells, not ont the headers
        private void MenuItemDeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteAllowed)
            {
                string columnHeader = Ic2DataGrid.CurrentCell.Column.Header.ToString();
                Binding binding = new Binding($"{columnHeader}");

                Ic2DataGrid.Columns.Remove(Ic2DataGrid.CurrentCell.Column);
                mySqlCommand.CommandText = "ALTER TABLE html5webnlkleijn.iamgod DROP COLUMN " + columnHeader;
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = mySqlConnection;

                mySqlConnection.Open();
                try
                {
                    reader = mySqlCommand.ExecuteReader();
                }
                catch
                {

                }

                mySqlConnection.Close();
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
            Ic2DataGrid.IsReadOnly = true;
        }

        /// <summary>
        /// This event start after the main and do everything what we need after.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ic2DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Detail change Column Cell 
            changeColorColumnCellDetailInteger(listOfColumnChangeIntegerAsCellDetail);
            //change all the column with a color
            changeColorAColumn(listBrush[0], listOfColumnChangeAllCell);
            //change color for a string
            changeColorAColumnString(listBrush[3], listOfColumnForString, listOfString);
        }



        /// <summary>
        /// This event check he has to update the layout color background. It should be disablee if we don't want a background color 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnUserControl_LayoutUpdated(object sender, EventArgs e)
        {
            if (updateColor)
            {
                //Detail change Column Cell 
                changeColorColumnCellDetailInteger(listOfColumnChangeIntegerAsCellDetail);
                //change all the column with a color
                changeColorAColumn(listBrush[0], listOfColumnChangeAllCell);
                // change the color for a string
                changeColorAColumnString(listBrush[3], listOfColumnForString, listOfString);
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

        //Call addcolumn method when click on button
        private void addingColumn_Click(object sender, RoutedEventArgs e)
        {
            addColumnButtonClick();
        }

        //Call addrow method when click on button
        private void addRowMenuItem_Click(object sender, RoutedEventArgs e)
        {

            string longerHeaderName = "";

            for (int i = 0; i < Ic2DataGrid.Columns.Count(); i++)
            {
                if (longerHeaderName.Length < Ic2DataGrid.Columns[i].Header.ToString().Length)
                {
                    longerHeaderName = Ic2DataGrid.Columns[i].Header.ToString();
                }

            }

            int YLabel = 10;
            int YTextBox = 10;

            AddRowWindow arw = new AddRowWindow();
            
            for (int i = 0; i < Ic2DataGrid.Columns.Count(); i++)
            {
                string headerName = Ic2DataGrid.Columns[i].Header.ToString();

                TextBlock myLabel = new TextBlock();
                myLabel.Height = 25;
                myLabel.Width = 120;
                myLabel.VerticalAlignment = VerticalAlignment.Top;
                myLabel.Margin = new Thickness(5, YLabel, 350, 0);
                myLabel.Text = headerName + " :";

                TextBox myTextBox = new TextBox();
                myTextBox.Height = 20;
                myTextBox.Width = 150;
                myTextBox.VerticalAlignment = VerticalAlignment.Top;
                myTextBox.Margin = new Thickness(0, YTextBox, 100, 0);

                CheckBox myCheckBox = new CheckBox();
                myCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
                myCheckBox.VerticalAlignment = VerticalAlignment.Top;

                if (Ic2DataGrid.Columns[i].GetType() == typeof(DataGridCheckBoxColumn))
                {
                    myCheckBox.Margin = new Thickness(90, YTextBox, 0, 0);

                    arw.myLabelsGrid.Children.Add(myCheckBox);

                    arw.Height += myTextBox.Height * 1.5;
                }
                else
                {
                    arw.myLabelsGrid.Children.Add(myTextBox);

                    arw.Height += myTextBox.Height * 1.5;
                }

                //Creating textboxes which start position depends on longer column header title


                //Add label and textbox
                arw.myLabelsGrid.Children.Add(myLabel);


                YLabel += 30;
                YTextBox += 30;
            }

            //arw.cancelRowAddButton.VerticalAlignment = VerticalAlignment.Bottom;
            //arw.okRowAddButton.VerticalAlignment = VerticalAlignment.Bottom;

            arw.ShowDialog();

            if (arw.DialogResult == true)
            {
                int textBoxesNumber = (arw.myLabelsGrid.Children.Count) / 2;

                for (int i = 0; i < textBoxesNumber; i++)
                {

                }
            }
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
                conDataBase.Close();
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

            if (add.DialogResult == true && !add.moreThanThatWeHaveToWrite)
            {
                header = add.AddColumnTitle.Text;
                addColumn(header, add.columnIsCheckBox);
                add.Close();
            }
            else
            {
                add.Close();
            }
        }

        /// <summary>
        /// Adding column method, which connects to the database and add if the user selecter check box, add a checkboc
        /// and if the user selected a string, will add a column of strings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void addColumn(string columnHeader, bool isCHeckBox)
        {
            Binding binding = new Binding($"{columnHeader}");

            if (isCHeckBox)
            {
                Ic2DataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = columnHeader, Binding = binding });

                mySqlCommand.CommandText = "ALTER TABLE html5webnlkleijn.iamgod ADD " + columnHeader + "BOOLEAN";
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = mySqlConnection;

                mySqlConnection.Open();
                try
                {
                    reader = mySqlCommand.ExecuteReader();
                }
                catch
                {

                }

                mySqlConnection.Close();
            }
            else
            {
                mySqlCommand.CommandText = "ALTER TABLE html5webnlkleijn.iamgod ADD " + columnHeader + "VARCHAR(20)";
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = mySqlConnection;

                mySqlConnection.Open();
                try
                {
                    reader = mySqlCommand.ExecuteReader();
                }
                catch
                {

                }


                mySqlConnection.Close();

                Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = columnHeader, Binding = binding });

            }
        }

        private void Ic2DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!updateANewCell)
            {
                Ic2DataGrid.IsReadOnly = true;
            }


            updateANewCell = false;

        }

        /// <summary>
        /// event if someone is in editing mode and presses enter then ok will make change in DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            updateANewCell = true;
            var cell = Ic2DataGrid.CurrentCell;
            if (e.Key == Key.Enter)
            {
                EditCheck edit = new EditCheck();
                edit.CheckPopupLabel.Content = "Do you want to update the current cell ?";
                edit.ShowDialog();
                if (edit.DialogResult.Value)
                {
                    var selectedRow = moduleHelper.GetSelectedRow(Ic2DataGrid);
                    int columnIndex = cell.Column.DisplayIndex;
                    writeInTheCell = true;
                    edit.Close();

                    DataGridCell dgc = Ic2DataGrid.GetCell(selectedRow, columnIndex);
                    string dgcs = dgc.Content.ToString().Substring(33);

                    string columnHeader = Ic2DataGrid.CurrentCell.Column.Header.ToString();
                    DataRowView dataRow = (DataRowView)Ic2DataGrid.SelectedItem;

                    string firstCellSelected = dataRow.Row.ItemArray[0].ToString();
                    string secondCellSelected = dataRow.Row.ItemArray[1].ToString();
                    

                    mySqlCommand.CommandText = "UPDATE html5webnlkleijn.iamgod SET " + columnHeader + " = '" + dgcs + "' WHERE " + Ic2DataGrid.Columns[0].Header.ToString() + " = '" + dataRow.Row.ItemArray[0].ToString() + "'";
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.Connection = mySqlConnection;

                    mySqlConnection.Open();
                    try
                    {
                        reader = mySqlCommand.ExecuteReader();
                    }
                    catch
                    {

                    }

                    mySqlConnection.Close();

                    

                    // Make a cell for column could to get a dynamic value

                    if (listOfColumnChangeIntegerAsCellDetail.Contains(columnIndex))
                    {
                        //Replace the value into the content with the column selected.

                        // Check if the enter is a numeric or not return Message Box if it's nt a numeric value
                        int nResult;
                        if (int.TryParse(dgcs, out nResult) == false)
                        {
                            MessageBox.Show("Not a correct entry !");
                            Ic2DataGrid.IsReadOnly = true;

                        }
                        else
                        {
                            Ic2DataGrid.IsReadOnly = false;
                            dgc.Content = dgcs;
                            updateColor = true;
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

        }

        private void EnableEdit(DataGridCell dgc, bool isReadOnly)
        {

        }


        //Here we have all methode who are used for the backgroundcolor
        /// <summary>
        /// This methode will make all the change for every cell you have in the column. It's taking a list as paramater you initialize in the main
        /// </summary>
        /// <param name="listOfColumnChangeInteger"></param>
        private void changeColorColumnCellDetailInteger(List<int> listOfColumnChangeInteger)
        {
            foreach (int index in listOfColumnChangeInteger)
            {

                for (int i = 0; i < Ic2DataGrid.Items.Count; i++)
                {
                    //Initialiaze a new brush
                    Brush color;
                    // get info of a cell 
                    DataGridRow r = Ic2DataGrid.GetRow(i);
                    DataGridCell cell = Ic2DataGrid.GetCell(r, index);
                    // methode to return a brush value
                    color = changeColorConditionIntegerWithValue(cell, tempBrush);
                    cell.Background = color;
                }
            }
        }
        /// <summary>
        /// Change a backgroundcolor with value in code
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private Brush changeColorConditionInteger(DataGridCell cell, Brush tempBrush)
        {

            //getting value of the cell
            string cellContent = cell.ToString();
            string[] getValue = cellContent.Split(':');

            int value = int.Parse(getValue[1]);
            if (value > 50)
            {
                tempBrush = Brushes.Green;

            }
            else if (value < 50)
            {
                tempBrush = Brushes.Red;
            }
            return tempBrush;
        }
        /// <summary>
        /// Brush b is the current backgroundcolor. Value1 will fix if you want a upper stric value backgroundcolor change and value2 will fix if you want a lower strict value backgroundcolor change. Brush 1 and 2 is the color for the value1 and value2
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="b"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="brushValue1"></param>
        /// <param name="brusValue2"></param>
        /// <returns></returns>
        private Brush changeColorConditionIntegerWithValue(DataGridCell cell, Brush b)
        {
            tempBrush = null;
            //Get the information about the value with a spliter 
            //The content content the type + value so the spliter keep the value
            string cellContent = cell.ToString();
            string[] getValue = cellContent.Split(':');

            //Spliter value are in an tab string so we took the 2 value. The 2 value is = at 1 on a tab 
            //1 is egal at the value of the cell
            int value = int.Parse(getValue[1]);

            if (value > listValues[0])
            {
                tempBrush = listBrush[1];

            }
            else if (value < listValues[1])
            {
                tempBrush = listBrush[2];
            }
            return tempBrush;
        }

        /// <summary>
        /// This change every background color of a column
        /// </summary>
        /// <param name="b"></param>
        /// <param name="indexColumn"></param>
        private void changeColorAColumn(Brush b, List<int> listOfColumnChangeAllCell)
        {
            //if you have an integer in the list it will color all the color with the brushCur
            foreach (int indexColumn in listOfColumnChangeAllCell)
            {
                for (int i = 0; i < Ic2DataGrid.Items.Count; i++)
                {
                    // get the cell
                    DataGridRow r = Ic2DataGrid.GetRow(i);
                    DataGridCell cell = Ic2DataGrid.GetCell(r, indexColumn);
                    cell.Background = b;
                }
            }
        }

        private void changeColorAColumnString(Brush b, List<int> listOfColumm, List<string> listOfString)
        {
            b = listBrush[3];
            //if you have an integer in the list it will color all the color with the brushCur
            foreach (int indexColumn in listOfColumnForString)
            {
                for (int i = 0; i < Ic2DataGrid.Items.Count; i++)
                {
                    // get the cell
                    DataGridRow r = Ic2DataGrid.GetRow(i);
                    DataGridCell cell = Ic2DataGrid.GetCell(r, indexColumn);
                    string cellContent = cell.ToString();
                    //get the value of the string
                    string[] getValue = cellContent.Split(':');
                    //loop for the contains of the stringlist
                    for(int j = 0; j < listOfString.Count; j++)               
                        if (getValue.Length > 1)
                        {
                            if (getValue[1].Contains(listOfString[j]))
                            {
                                cell.Background = b;
                            }
                        }                                      
                }
            }
        }

        public void changeHeaderWithImage(int indexColumn, string test)
        {

            Ic2DataGrid.Columns[indexColumn].Header = null;
            Ic2DataGrid.Columns[indexColumn].HeaderTemplate = FindResource(test) as DataTemplate;
        }

        //This methode need to be changde everytime you add new values. If values are null you have to write it. Example for brush = transparent
        /// <summary>
        /// This methode create all the value data info storage for make the backgroundcolor
        /// </summary>
        public void convertionAppDataInfo()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //      tab = config.AppSettings.Settings.AllKeys.ToArray<config.AppSettings.Settings.AllKeys.ToString()>;

            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key.StartsWith("brush"))
                {
                    string brush = ConfigurationManager.AppSettings[key];
                    Brush aBrush;
                    SolidColorBrush scb = (SolidColorBrush)new BrushConverter().ConvertFromString(brush);
                    aBrush = scb;
                    listBrush.Add(aBrush);
                }
                else if (key.StartsWith("valueOfTest"))
                {
                    string value = ConfigurationManager.AppSettings[key];
                    int aValue = int.Parse(value);
                    listValues.Add(aValue);
                }
                else if (key.StartsWith("listOfString"))
                {
                    string stringToSplit = ConfigurationManager.AppSettings[key];
                    string[] stringSplited = stringToSplit.Split(',');
                    foreach (string str in stringSplited)
                    {
                        listOfString.Add(str);
                    }
                }
                else if (key.StartsWith("listOfColumnForString"))
                {
                    string stringToSplit = ConfigurationManager.AppSettings[key];
                    string[] stringSplited = stringToSplit.Split(',');
                    foreach (string str in stringSplited)
                    {
                        listOfColumnForString.Add(int.Parse(str));
                    }
                }
                else if (key.StartsWith("listOfColumnChangeAllCell"))
                {
                    string stringToSplit = ConfigurationManager.AppSettings[key];
                    string[] stringSplited = stringToSplit.Split(',');
                    foreach (string str in stringSplited) ;
                    foreach (string str in stringSplited)
                    {
                        listOfColumnChangeAllCell.Add(int.Parse(str));
                    }
                }
                else if (key.StartsWith("listOfColumnChangeIntegerAsCellDetail"))
                {
                    string stringToSplit = ConfigurationManager.AppSettings[key];
                    string[] stringSplited = stringToSplit.Split(',');
                    foreach (string str in stringSplited)
                    {
                        listOfColumnChangeIntegerAsCellDetail.Add(int.Parse(str));
                    }

                }
            }
        }



        //Microsoft.Office.Interop.Excel reference link of code : 
        //https://stackoverflow.com/questions/11167918/how-to-export-from-datatable-to-excel-file-in-wpf-c-sharp
        /// <summary>
        /// Create a new xls file 
        /// </summary>
        public void toExel()
        {
            DataTable dt = new DataTable();
            dt = ((DataView)Ic2DataGrid.ItemsSource).ToTable();
            {

                Excel.Application excel = null;
                Excel.Workbook wb = null;

                object missing = Type.Missing;
                Excel.Worksheet ws = null;
                Excel.Range rng = null;

                try
                {
                    excel = new Excel.Application();
                    wb = excel.Workbooks.Add();
                    ws = (Excel.Worksheet)wb.ActiveSheet;


                    for (int Idx = 0; Idx < dt.Columns.Count; Idx++)
                    {
                        ws.Range["A1"].Offset[0, Idx].Value = dt.Columns[Idx].ColumnName;
                    }

                    for (int Idx = 0; Idx < dt.Rows.Count; Idx++)
                    {
                        ws.Range["A2"].Offset[Idx].Resize[1, dt.Columns.Count].Value =
                        dt.Rows[Idx].ItemArray;
                    }
                   
                    excel.Visible = true;
                    wb.Activate();

                }
                catch (COMException ex)
                {
                    MessageBox.Show("Error accessing Excel: " + ex.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString());
                }
            }

        }
    }
}