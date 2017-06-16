using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        public class MenuService
        {
            private List<MenuItem> allMenuItems;

            public MenuService()
            {
                allMenuItems = new List<MenuItem>();
            }

            public List<MenuItem> GetParentMenuItems()
            {
                List<MenuItem> parentItems = allMenuItems.FindAll(x => x.Parent == null);

                return parentItems;
            }

            public void AddMenuItem(MenuItem item, string parentName = "")
            {
                if (parentName == string.Empty)
                {
                    this.allMenuItems.Add(item);
                }
                else
                {
                    MenuItem parent = allMenuItems.Find(x => x.Name == parentName);
                    if (parent != null)
                    {
                        item.Parent = parent;
                        parent.AddSubMenu(item);
                    }
                    allMenuItems.Add(item);
                }
            }

            public void RemoveMenuItem(MenuItem menuItem)
            {
                foreach (MenuItem item in allMenuItems)
                {
                    item.RemoveSubMenu(menuItem);
                }

                if (this.allMenuItems.Contains(menuItem))
                {
                    this.allMenuItems.Remove(menuItem);
                }
            }


        }

        public abstract class MenuItem
        {
            private string name;

            private string text;

            private ObservableCollection<MenuItem> subItems;

            private ICommand onSelected;

            private MenuItem parent;

            public MenuItem(string name, string text)
            {
                this.name = name;
                this.text = text;
                this.subItems = new ObservableCollection<MenuItem>();
            }

            public string Name { get { return this.name; } }

            public string Text { get { return this.text; } }

            public MenuItem Parent { get { return this.parent; } set { this.parent = value; } }

            public ICommand OnSelected
            {
                get
                {
                    if (this.onSelected == null)
                    {
                        this.onSelected = new MenuCommand(this.OnItemSelected, this.ItemCanBeSelected);
                    }
                    return this.onSelected;
                }
            }

            public ObservableCollection<MenuItem> SubItems
            {
                get
                {
                    return this.subItems;
                }
            }

            public void AddSubMenu(MenuItem menuItem)
            {
                this.subItems.Add(menuItem);
            }

            public void RemoveSubMenu(MenuItem menuItem)
            {
                if (this.subItems.Contains(menuItem))
                {
                    this.subItems.Remove(menuItem);
                }
            }

            public abstract void OnItemSelected();

            public virtual bool ItemCanBeSelected()
            {
                return true;
            }

        }

        class MenuCommand : ICommand
        {
            private Action execute;

            private Func<bool> canExecute;

            public MenuCommand(Action execute, Func<bool> canExecute)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            public void Execute(object parameter)
            {
                execute();
            }

            public bool CanExecute(object parameter)
            {
                return this.canExecute();
            }

            private void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        class FileMenuItem : MenuItem
        {
            public FileMenuItem() : base("File", "_File")
            {

            }

            public override void OnItemSelected()
            {
                // Top level item, we don't need to do anything.
            }
        }

        class AddMenuItem : MenuItem
        {
            MenuService menuService;

            public AddMenuItem(MenuService menuService) : base("AddMenu", "Add MenuItem")
            {
                this.menuService = menuService;
            }

            public override void OnItemSelected()
            {
                RemoveMenuItem removeItem = new RemoveMenuItem(this.menuService);
                menuService.AddMenuItem(removeItem, "File");
            }
        }

        class RemoveMenuItem : MenuItem
        {
            private MenuService menuService;

            public RemoveMenuItem(MenuService menuService) : base("Remove", "Remove Me")
            {
                this.menuService = menuService;
            }

            public override void OnItemSelected()
            {
                menuService.RemoveMenuItem(this);
            }
        }

        class DisableMenuItem : MenuItem
        {
            private bool enabled = true;

            public DisableMenuItem() : base("DisableMenuItem", "Disable Me!")
            {

            }

            public override void OnItemSelected()
            {
                this.enabled = false;
            }

            public override bool ItemCanBeSelected()
            {
                return this.enabled;
            }
        }
        public class MainWindowViewModel : INotifyPropertyChanged
        {
            MenuService menuService;

            public MainWindowViewModel()
            {
                this.menuService = new MenuService();

                FileMenuItem fileMenu = new FileMenuItem();
                AddMenuItem addMenu = new AddMenuItem(this.menuService);
                DisableMenuItem disableableMenuItem = new DisableMenuItem();

                menuService.AddMenuItem(fileMenu);
                menuService.AddMenuItem(addMenu, "File");
                menuService.AddMenuItem(disableableMenuItem, ("File"));
            }

            public List<MenuItem> ParentItems
            {
                get
                {
                    return this.menuService.GetParentMenuItems();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

    }
}
