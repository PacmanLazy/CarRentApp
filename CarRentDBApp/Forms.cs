using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarRentDBApp
{
    public partial class MainWindow : Window
    {
        CustomersForm _customersForm;
        CustomerRentForm _customerRentForm;

        private void Customers_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement form = (sender as FrameworkElement);

            if (e.ClickCount == 2)
            {
                if (_customersForm == null && form.Name == "Customers")
                    _customersForm = new CustomersForm(_connection);
                else
                {
                    _customersForm.Close();
                    _customersForm = new CustomersForm(_connection);
                }
            }
        }
        private void CustomersRents_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement form = (sender as FrameworkElement);

            if (e.ClickCount == 2)
            {
                
                if (_customerRentForm == null && form.Name == "CustomerRent")
                    _customerRentForm = new CustomerRentForm(_connection);
                else
                {
                    _customerRentForm.Close();
                    _customerRentForm = new CustomerRentForm(_connection);
                }

            }
        }
    }
}
