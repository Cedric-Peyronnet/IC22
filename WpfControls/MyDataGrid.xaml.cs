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
using System.Runtime.InteropServices;

namespace WpfControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MyDataGrid : UserControl
    {
        public bool updateANewCell { get; set; }
        public bool writeInTheCell { get; set; }
        public bool DeleteAllowed { get; set; }
        public bool headerClicked { get; set; }
        // Everything here have to be initialize in the main
        public Brush tempBrush { get; set; }

        //Brush and values List storage

        public List<int> listOfColumnChangeIntegerAsCellDetail = new List<int>();
        
        private static List<string> headerList = new List<string>();

        private string sqlQuerry = "select* from html5webnlkleijn.iamgod";
        private string connection = "Server=84.246.4.143;port=9131;database=html5webnlkleijn;username=html5webnlkltest;password=testtest1";

        public int index;
        public DataGridCellInfo cellToEdit;
        public MySqlConnection mySqlConnection = new MySqlConnection("Server=84.246.4.143;port=9131;database=html5webnlkleijn;username=html5webnlkltest;password=testtest1");
        public MySqlCommand mySqlCommand = new MySqlCommand();
        public MySqlDataReader reader;
        private string typeOfColumn;

        private DataRowView myDataRow { get; set; }
 

        public MyDataGrid()
        {
            InitializeComponent();
        }

        // Method which delete a column in the dataGrid and in the database, but for the moment,
        //only if you right click on the cells, not ont the headers
        private void MenuItemDeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            //Works only if you left click before on a cell
            headerList.RemoveAt(Ic2DataGrid.CurrentCell.Column.DisplayIndex);

            if (DeleteAllowed)
            {
                string columnHeader = Ic2DataGrid.CurrentCell.Column.Header.ToString();
                Binding binding = new Binding($"{columnHeader}");

                //Create the sql query which will be executed
                Ic2DataGrid.Columns.Remove(Ic2DataGrid.CurrentCell.Column);
                mySqlCommand.CommandText = "ALTER TABLE html5webnlkleijn.iamgod DROP COLUMN " + columnHeader;
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = mySqlConnection;

                //Open the connection to the sql database and execute the query previously written
                mySqlConnection.Open();
                try
                {
                    reader = mySqlCommand.ExecuteReader();
                }
                catch
                {

                }

                //Close the connection
                mySqlConnection.Close();
                
            }
        }

            ///Events on edition

        /// <summary>
        /// if double click into a cell go on edit mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (headerClicked)
            {

            }else
            {
                //Make update available 
                Ic2DataGrid.IsReadOnly = false;

                //Get the type of the cell
                typeOfColumn = Ic2DataGrid.CurrentCell.Column.GetType().ToString();
            }

            cellToEdit = Ic2DataGrid.CurrentCell;
            myDataRow = (DataRowView)Ic2DataGrid.SelectedItem;
            index = Ic2DataGrid.CurrentCell.Column.DisplayIndex;

        }

        //If someone change the focus of the  currentCell by clicking somewhere else,it will change the readonly on true.
        private void Ic2DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            Ic2DataGrid.IsReadOnly = true;
        }

        ///End of events on edition    
        
        //Call addcolumn method when click on ok Button of the addcolumn window
        private void addingColumn_Click(object sender, RoutedEventArgs e)
        {
            addColumnButtonClick();
        }

        //Open a window to enter every information on a row
        private void addRowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Create integers to allow the modification of the position
            //Better to do YLabel += 10; than 150 lines more and write every time a different position
            int YLabel = 10;
            int YTextBox = 10;

            //Create a new window of row creation
            AddRowWindow arw = new AddRowWindow();
            
            for (int i = 0; i < Ic2DataGrid.Columns.Count(); i++)
            {
                //Get the name of the column to add
                string headerName = headerList[i];

                //Create and place a label which contains the header name of one column
                TextBlock myLabel = new TextBlock();
                myLabel.Height = 25;
                myLabel.Width = 120;
                myLabel.VerticalAlignment = VerticalAlignment.Top;
                myLabel.Margin = new Thickness(5, YLabel, 350, 0);
                myLabel.Text = headerName + " :";

                //Create and place a text box which will contain what the user will write in
                TextBox myTextBox = new TextBox();
                myTextBox.Height = 20;
                myTextBox.Width = 150;
                myTextBox.VerticalAlignment = VerticalAlignment.Top;
                myTextBox.Margin = new Thickness(0, YTextBox, 100, 0);

                //Create a checkbox which can be used if the user wanted a check box column
                CheckBox myCheckBox = new CheckBox();
                myCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
                myCheckBox.VerticalAlignment = VerticalAlignment.Top;

                //Place the elements, or check box, or label, or textbox and change the position every loop
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

                //Add label and textbox
                arw.myLabelsGrid.Children.Add(myLabel);

                //Add the position which will change for the next loop
                YLabel += 30;
                YTextBox += 30;
            }

            //arw.cancelRowAddButton.VerticalAlignment = VerticalAlignment.Bottom;
            //arw.okRowAddButton.VerticalAlignment = VerticalAlignment.Bottom;

            //Open the window with every labels, check boxes and text boxes
            arw.ShowDialog();

            //If the user clicks on OK, get the content of every textboxes and check boxes
            if (arw.DialogResult == true)
            {
                int textBoxesNumber = (arw.myLabelsGrid.Children.Count) / 2;

                for (int i = 0; i < textBoxesNumber; i++)
                {

                }
            }
        }

        /// <summary>
        ///Loading a datagrid from a database Param :
        ///(CheckBoxList, which contains the checkboxes columns, 
        ///sqlConnection which is the line to make the connection to the database, 
        ///sqlQuerry, which is the query that you want to send to the database)
        /// </summary>
        /// <param name="CheckBoxList"></param>
        /// <param name="sqlConnection"></param>
        /// <param name="sqlQuerry"></param>
        public void LoadDataFromSQL(List<int> CheckBoxList, string sqlConnection, string sqlQuerry)
        {
            //Create the database command with the query previously entered
            //And with the connection already written
            MySqlCommand cmdDataBase = new MySqlCommand(sqlQuerry, mySqlConnection);
            try
            {
                MySqlDataAdapter sda = new MySqlDataAdapter(cmdDataBase);

                //Data table to store the informations
                DataTable dbDataTable = new DataTable();

                //Fill the inforamation into the datatable
                sda.Fill(dbDataTable);

                //Binding all the information s
                for (int index = 0; index < dbDataTable.Columns.Count; index++)
                {
                    //Binding information (CheckBox part) 
                    if (CheckBoxList.Contains(index))
                    {
                        var binding = new Binding($"{dbDataTable.Columns[index].ToString()}");
                        Ic2DataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = dbDataTable.Columns[index].ColumnName, Binding = binding });
                    }

                    //Binding information (datagrid column part)
                    else
                    {
                        var binding = new Binding($"{dbDataTable.Columns[index].ToString()}");
                        Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = dbDataTable.Columns[index].ColumnName, Binding = binding });
                    }
                }

                //Insert the information into itemsource 
                Ic2DataGrid.ItemsSource = dbDataTable.DefaultView;
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {MessageBox.Show(ex.Message);}

            //Refill the header list to be sure that every column names are in this lsit
            fillHeaderList(connection);
        }

        //Open a window with a label, a text box and allow the user to choose
        //to create a string column or a check box column
        public void addColumnButtonClick()
        {
            string header;

            AddMenuButtonInformation add = new AddMenuButtonInformation();

            //Create the window dynamicly
            add.AddTextBoxLabel.Content = "Enter column name :";
            add.addOneElementWindow.Title = "Add a column";
            add.AddColumnTitle.Text = "Enter column name";
            add.ShowDialog();

            //If the user clicks on "ok", and the name of the column isn't too long,
            //Create this column
            //Else, close the window.
            if (add.DialogResult == true && add.moreThanThatWeHaveToWrite == false)
            {
                header = add.AddColumnTitle.Text;
                addColumn(header, add.columnIsCheckBox);
                add.Close();
            }
            else
            {
                add.Close();
            }

           
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            AutoClosingMessageBox.Show("Loading", "HEHE TU ATTENDS", 1000);
            Application.Current.Shutdown();
            

        }

        /// <summary>
        /// Adding column method, which connects to the database,
        /// add if the user selected check box, add a checkbox
        /// and if the user selected a string, will add a column of strings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void addColumn(string columnHeader, bool isCHeckBox)
        {
            Binding binding = new Binding($"{columnHeader}");

            //Test if it's a check box column or a string column
            if (isCHeckBox)
            {
                //Add in the grid a check box column
                Ic2DataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = columnHeader, Binding = binding });

                //Add in the database a column and her type is boolean
                mySqlCommand.CommandText = "ALTER TABLE html5webnlkleijn.iamgod ADD " + columnHeader + " BOOLEAN";
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = mySqlConnection;

                mySqlConnection.Open();
                try
                {
                    reader = mySqlCommand.ExecuteReader();
                }
                catch{}
                mySqlConnection.Close();


                mySqlCommand.CommandText = "UPDATE html5webnlkleijn.iamgod SET " + columnHeader + " = '0'";
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = mySqlConnection;

                mySqlConnection.Open();
                try
                {
                    reader = mySqlCommand.ExecuteReader();
                }
                catch { }
                mySqlConnection.Close();

            }
            else
            {
                //Add in the database a column and her type is varchar of 20 chars
                mySqlCommand.CommandText = "ALTER TABLE html5webnlkleijn.iamgod ADD " + columnHeader + "VARCHAR(20)";
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = mySqlConnection;

                mySqlConnection.Open();
                try
                {
                    reader = mySqlCommand.ExecuteReader();
                }
                catch{}

                mySqlConnection.Close();

                //Add to the grid a new text column
                Ic2DataGrid.Columns.Add(new DataGridTextColumn { Header = columnHeader, Binding = binding });
            }

            //Add to the header list the new column created
            fillHeaderList(connection);
        }

        //Confirm if you stopped the edition mode or not
        private void Ic2DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!updateANewCell)
            {
                Ic2DataGrid.IsReadOnly = true;
            }
            updateANewCell = true;
        }

        //Fill the header list which contains every columns names of the database
        public void fillHeaderList(string sqlConnection)
        {
            //Refer to load data
            MySqlCommand cmdDataBase = new MySqlCommand(sqlQuerry, mySqlConnection);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmdDataBase);
            DataTable dbDataTable = new DataTable();

            sda.Fill(dbDataTable);
            
            //For every columns the database has, fill her name in a list
            for (int index = 0; index < dbDataTable.Columns.Count; index++)
            {
                headerList.Add(dbDataTable.Columns[index].ColumnName);
            }
        }


        /// <summary>
        /// Event if someone is in editing mode and presses enter then ok will make change in DataGrid
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

                //Tests if the user press "Enter"
                if (edit.DialogResult.Value)
                {
                    var selectedRow = moduleHelper.GetSelectedRow(Ic2DataGrid);
                    int columnIndex = cell.Column.DisplayIndex;
                    writeInTheCell = true;
                    edit.Close();

                    DataGridCell dgc = Ic2DataGrid.GetCell(selectedRow, columnIndex);
                    string dgcs = dgc.Content.ToString().Substring(33);

                    //Create a string which contains the actual name of the column that you are editing
                    string columnHeader = headerList[columnIndex];
                    DataRowView dataRow = (DataRowView)Ic2DataGrid.SelectedItem;                 
                    
                    //Creates the SQL Query
                    mySqlCommand.CommandText = "UPDATE html5webnlkleijn.iamgod SET " + columnHeader + " = '" + dgcs + "' WHERE " + headerList[0] + " = '" + dataRow.Row.ItemArray[0].ToString() + "'";
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.Connection = mySqlConnection;

                    //Open the connection to sql database and execure the query previously written
                    mySqlConnection.Open();
                    try
                    {
                        reader = mySqlCommand.ExecuteReader();
                    }
                    catch{}

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
                        }
                    }
                }
                else
                {
                    // If dialogue result = false
                    Ic2DataGrid.IsReadOnly = true;
                    writeInTheCell = true;
                    edit.Close();
                }
            }else
            {
         
            }
        }

        public void setDBBooleanValue()
        {
            
            if (typeOfColumn == typeof(DataGridCheckBoxColumn).ToString())
            {
                //MessageBox.Show("chackbox");
                string b = myDataRow.Row.ItemArray[index].ToString();

                if (b == "True")
                {
                    mySqlCommand.CommandText = "UPDATE html5webnlkleijn.iamgod SET " + cellToEdit.Column.Header.ToString() + " = '1' WHERE " + headerList[0] + " = '" + myDataRow.Row.ItemArray[0].ToString() + "'";
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.Connection = mySqlConnection;

                    mySqlConnection.Open();
                    try
                    {
                        reader = mySqlCommand.ExecuteReader();
                    }
                    catch { }
                    mySqlConnection.Close();
                }
                else if (b.Contains("False"))
                {

                   mySqlCommand.CommandText = "UPDATE html5webnlkleijn.iamgod SET " + cellToEdit.Column.Header.ToString() + " = '0' WHERE " + headerList[0] + " = '" + myDataRow.Row.ItemArray[0].ToString() + "'";
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.Connection = mySqlConnection;

                    mySqlConnection.Open();
                    try
                    {
                        reader = mySqlCommand.ExecuteReader();
                    }
                    catch { }
                    mySqlConnection.Close();
                }
            }
        }
        
        
        //Here we have all methode who are used for the backgroundcolor

        /// <summary>
        /// This methode will make all the change for every cell you have in the column.
        /// It's taking a list as paramater you initialize in the main
        /// </summary>
        /// <param name="listOfColumnChangeInteger"></param>
        public void changeColorColumnCellDetailInteger(List<int> listOfColumnChangeInteger, int value1, int value2, Brush brushValue1, Brush brushValue2)
        {
            foreach (int index in listOfColumnChangeInteger)
            {

                for (int i = 0; i < Ic2DataGrid.Items.Count; i++)
                {
                    // get info of a cell 
                    DataGridRow r = Ic2DataGrid.GetRow(i);
                    DataGridCell cell = Ic2DataGrid.GetCell(r, index);
                    // methode to return a brush value
                    Brush color = changeColorConditionIntegerWithValue(cell, value1, value2,brushValue1, brushValue2);
                    cell.Background = color;
                    listOfColumnChangeIntegerAsCellDetail.Add(index);
                }
            }
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

        private Brush changeColorConditionIntegerWithValue(DataGridCell cell, int value1, int value2, Brush brushValue1, Brush brushValue2)
        {
            tempBrush = null;
            //Get the information about the value with a spliter 
            //The content content the type + value so the spliter keep the value
            string cellContent = cell.ToString();
            string[] getValue = cellContent.Split(':');

            //Spliter value are in an tab string so we took the 2 value. The 2 value is = at 1 on a tab 
            //1 is egal at the value of the cell
            int value = int.Parse(getValue[1]);

            if (value > value1)
            {
                tempBrush = brushValue1;

            }
            else if (value < value2)
            {
                tempBrush = brushValue2;
            }
            return tempBrush;
        }

        /// <summary>
        /// This change every background color of a column
        /// </summary>
        /// <param name="b"></param>
        /// <param name="indexColumn"></param>
        public void changeColorAColumn(Brush b, List<int> listOfColumnChangeAllCell)
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
     
        public void changeColorAColumnString(Brush b, List<int> listOfColumm, List<string> listOfString)
        {
         
            //if you have an integer in the list it will color all the color with the brushCur
            foreach (int indexColumn in listOfColumm)
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
        
        /// <summary>
        /// Change the header text by a name 
        /// </summary>
        /// <param name="indexColumn"></param>
        /// <param name="test"></param>
        public void changeHeaderWithImage(int indexColumn, string test)
        {

            Ic2DataGrid.Columns[indexColumn].Header = null;
            Ic2DataGrid.Columns[indexColumn].HeaderTemplate = FindResource(test) as DataTemplate;
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

        private void userMouseDown(object sender, MouseButtonEventArgs e)
        {
            headerClicked = false;
        }

        public class AutoClosingMessageBox
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                    null, timeout, System.Threading.Timeout.Infinite);
                using (_timeoutTimer)
                    MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }
    }
}