using System;
using System.Data.SqlClient;
using System.Windows;

namespace CarRentDBApp
{
    /// <summary>
    /// Логика взаимодействия для LogInForm.xaml
    /// </summary>
    public partial class LogInForm : Window
    {
        public LogInForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = @"PAC-PC\SQLEXPRESS";       
                builder.InitialCatalog = "CarRentalDB";          
                builder.UserID = LoginBox.Text;
                builder.Password = PassWordBox.Password;
                
                MainWindow window =  new MainWindow(new SqlConnection(builder.ConnectionString));
                window.Show();

                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Неверный логин или пароль(возможны проблемы с подключением)");
            }
        }
    }
}
