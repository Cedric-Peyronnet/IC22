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
        public bool textZoneSelected = false;
        public bool moreThanThatWeHaveToWrite { get; set; }

        public bool columnIsCheckBox = false;
        public AddMenuButtonInformation()
        {
            InitializeComponent();
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {            
            if (AddColumnTitle.Opacity != 100)
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

        private void AddColumnTitle_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            moreThan15();
            
        }

        private void moreThan15()
        {
            moreThanThatWeHaveToWrite = false;
            
            if (AddColumnTitle.Text.Length >= 15)
            {
                moreThanThatWeHaveToWrite = true;
                AddColumnTitle.IsEnabled = false;
                MessageBoxResult tooLongBox;
                tooLongBox = MessageBox.Show("This text is too long, please, enter a shorter text", "Too long !", MessageBoxButton.OKCancel);
                if (tooLongBox == MessageBoxResult.OK)
                {
                    string newText = AddColumnTitle.Text;
                    newText = newText.Substring(0, 14);
                    AddColumnTitle.Text = newText;
                    AddColumnTitle.IsEnabled = true;
                    moreThanThatWeHaveToWrite = false;
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void chooseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(myChooseBox.SelectedIndex == 1)
            {
                columnIsCheckBox = true;
            }
            else if(myChooseBox.SelectedIndex == 0)
            {
                columnIsCheckBox = false;
            }
        }
    }
}
