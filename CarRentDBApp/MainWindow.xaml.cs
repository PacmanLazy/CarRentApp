using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarRentDBApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isAdmin;

        SqlConnection _connection;
        CarRentalDbWorker _carRentalDb;

        public MainWindow(SqlConnection connection)
        {
            InitializeComponent();

            _connection = connection;
            _carRentalDb = new CarRentalDbWorker(_connection);

            SqlCommand cmd = new SqlCommand("DefinePermissionLevel" ,_connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            
            string accessSchemaName = cmd.ExecuteScalar().ToString();

            if (accessSchemaName.Equals("CarRentalDbAdmin"))
            {
                isAdmin = true;
            }

            if(!isAdmin)
            {
                TablesMenu.Children.Remove(CustomersNavItem);
                TablesMenu.Children.Remove(RentNavItem);
                RequestMenu.Children.Remove(RentDetailsRequest);
                RequestMenu.Children.Remove(RentsForTimeRequest);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

//Методы управления вкладками
        private TabItem InitNewTab(TabItem newTab, string title)
        {
            newTab = new TabItem();
            newTab.ToolTip = title;
            
            StackPanel header = new StackPanel();
            header.Orientation = Orientation.Horizontal;
            header.Width = double.NaN;
            header.Height = double.NaN;

            Label headerLabel = new Label();
            headerLabel.Foreground = Brushes.White;
            headerLabel.Content = title;

            Button closeButton = new Button();
            closeButton.Margin = new Thickness(5);
            closeButton.Width = double.NaN;
            closeButton.Foreground = Brushes.Red;
            closeButton.Background = Brushes.Transparent;
            closeButton.BorderThickness = new Thickness(0);
            closeButton.Content = "X";
            closeButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Content.Items.Remove(newTab);
            };

            header.Children.Add(headerLabel);
            header.Children.Add(closeButton);
            newTab.Header = header;

            return newTab;
        }

        private void OpenTab(ref TabItem tab, string title, TabContentTemplate contentContainer)
        {
            if (!Content.Items.Contains(tab))
            {
                tab = InitNewTab(tab, title);
                Content.Items.Add(tab);
                Content.SelectedItem = tab;

                tab.Content = contentContainer;
            }
            else
            {
                Content.SelectedItem = tab;
            }
        }

        private void AppWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _connection.Close();
        }
    }
}
