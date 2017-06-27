using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfControls
{
    public class Property : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Property(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public object Value { get; set; }


    }
    public class Record
    {
        public Record(params Property[] properties)
        {
            foreach (var property in properties)
            {
                Properties.Add(property);
            }
        }

        public ObservableCollection<Property> Properties { get; set; } = new ObservableCollection<Property>();
    }
 
    /// <summary>
    /// Support for change a color background it could be implement for other things with colors 
    /// </summary>  
    public class ValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int input;
            int negativOrPositivOrZero;
            try
            {
                DataGridCell dgc = (DataGridCell)value;
                Record records = (Record)dgc.DataContext;
                //For every column you get is going to check value of integer of the row 
                //You have to insert the column where the color should be changed or apply 
                for (int i = 0; i < records.Properties.Count; i++)
                {
                    //The case you want a dynamic change color ! :)
                    //Add case to have an other column dynamic color
                    switch (i)
                    {
                        case 1:

                            string a = records.Properties[i].Value.ToString();
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
                            break;
                    }
                }


            }
            catch (InvalidCastException e)
            {
                return DependencyProperty.UnsetValue;
            }
            return DependencyProperty.UnsetValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// this class helps forget some information about the current data grid
    /// </summary>
    public static class moduleHelper
    {
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static DataGridRow GetSelectedRow(this DataGrid grid)
        {
            return (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
        }

        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public static DataGridCell GetCell(this DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = grid.GetRow(row);
            return grid.GetCell(rowContainer, column);
        }
    }
}
