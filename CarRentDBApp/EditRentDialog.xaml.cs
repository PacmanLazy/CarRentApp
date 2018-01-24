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
    /// Логика взаимодействия для EditRentDialog.xaml
    /// </summary>
    public partial class EditRentDialog : Window
    {
        SqlConnection _connection;

        int _orderId;

        public EditRentDialog(SqlConnection connection, DataRowView row)
        {
            InitializeComponent();

            _connection = connection;

            _orderId = int.Parse(row[1].ToString());

            ShowActivated = true;
            ShowDialog();
        }

        private void OkButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            byte rentDays = 0;

            if (byte.TryParse(RentDaysBox.Text, out rentDays))
            {
                CarRentalDbWorker.EditRent(_connection, _orderId, rentDays);

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
