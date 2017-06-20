using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControls
{
    /// <summary>
    /// Interaction logic for AddRowWindow.xaml
    /// </summary>
    public partial class AddRowWindow : Window
    {
        public AddRowWindow()
        {
            InitializeComponent();
        }

        private void okRowAddButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancelRowAddButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
