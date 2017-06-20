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
using System.Windows.Shapes;

namespace WpfControls
{
    /// <summary>
    /// Interaction logic for AddMenuButtonInformation.xaml
    /// </summary>
    public partial class AddMenuButtonInformation : Window
    {
        //Boolean which says if the addcolumn text zone is already selected
        public Boolean textZoneSelected = false;
        public AddMenuButtonInformation()
        {
            InitializeComponent();
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if(AddColumnTitle.Opacity != 100)
            {
                AddColumnTitle.Text = "";
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = false;
        }

        private void AddColumnTitle_GotMouseCapture(object sender, MouseEventArgs e)
        {
            
            if (!textZoneSelected)
            {
                AddColumnTitle.Text = " ";
                AddColumnTitle.Opacity = 100;
                textZoneSelected = true;
            }            
        }
    }
}
