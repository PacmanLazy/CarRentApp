using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace CarRentDBApp
{
    /// <summary>
    /// Логика взаимодействия для EditCustomerDialog.xaml
    /// </summary>
    public partial class EditCustomerDialog : Window
    {
        SqlConnection _connection;

        string _oldPassData;
        
        public EditCustomerDialog(SqlConnection connection, DataRowView row)
        {
            InitializeComponent();

            _connection = connection;

            _oldPassData = row[1].ToString();
            PassBox.Text = row[1].ToString();
            LNameBox.Text = row[2].ToString();
            FNameBox.Text = row[3].ToString();
            MNameBox.Text = row[4].ToString();

            ShowActivated = true;
            ShowDialog();
        }

        private void OkButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PassBox.Text != string.Empty &&
                LNameBox.Text != string.Empty &&
                FNameBox.Text != string.Empty)
            {
                CarRentalDbWorker.EditCustomer(_connection, _oldPassData, PassBox.Text, LNameBox.Text, FNameBox.Text, MNameBox.Text);

                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Неверно введены данные");
        }

        private void CancelButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
