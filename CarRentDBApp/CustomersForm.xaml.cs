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
    /// Логика взаимодействия для ClientsForm.xaml
    /// </summary>
    public partial class CustomersForm : Window
    {

        SqlConnection _connection;
        DataTable _customers;

        int _currentPage = 1;
        int _customersRowCount = 0;

        string _oldPass;
        bool isPageSwitched;

        public CustomersForm(SqlConnection connection)
        {
            InitializeComponent();

            _connection = connection;
            _customers = new DataTable();
            _customersRowCount = CarRentalDbWorker.TotalFormDataRowCount(_connection, "Customers");

            SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", _currentPage, _connection);
            _customers.Load(reader);
            reader.Close();

            Title = string.Format("Клиент № {0}", _customers.Rows[0][0].ToString());

            PassBox.Text = _customers.Rows[0][1].ToString();
            _oldPass = PassBox.Text;

            LNameBox.Text = _customers.Rows[0][2].ToString();
            FNameBox.Text = _customers.Rows[0][3].ToString();
            MNameBox.Text = _customers.Rows[0][4].ToString();

            Show();
        }

        private void PrevButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentPage != 1)
            {
                isPageSwitched = true;

                _customers.Clear();

                SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", _currentPage - 1, _connection);
                _customers.Load(reader);
                reader.Close();

                PassBox.Text = _customers.Rows[0][1].ToString();
                _oldPass = PassBox.Text;

                LNameBox.Text = _customers.Rows[0][2].ToString();
                FNameBox.Text = _customers.Rows[0][3].ToString();
                MNameBox.Text = _customers.Rows[0][4].ToString();

                Title = string.Format("Клиент № {0}", _customers.Rows[0][0].ToString());
                _currentPage--;
            }
        }

        private void NextButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPageSwitched = true;
            _customers.Clear();

            if (_currentPage != _customersRowCount)
            {
                SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", _currentPage + 1, _connection);
                _customers.Load(reader);
                reader.Close();

                PassBox.Text = _customers.Rows[0][1].ToString();
                _oldPass = PassBox.Text;

                LNameBox.Text = _customers.Rows[0][2].ToString();
                FNameBox.Text = _customers.Rows[0][3].ToString();
                MNameBox.Text = _customers.Rows[0][4].ToString();

                Title = string.Format("Клиент № {0}", _customers.Rows[0][0].ToString());
                _currentPage++;
            }
            else
            {
                _oldPass = string.Empty;

                Title = "Новый клиент";

                PassBox.Text = string.Empty;
                LNameBox.Text = string.Empty;
                FNameBox.Text = string.Empty;
                MNameBox.Text = string.Empty;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isPageSwitched)
            {
                int findUserResult = 0;

                if (PassBox.Text != string.Empty &&
                    LNameBox.Text != string.Empty &&
                    FNameBox.Text != string.Empty)
                {
                    string query = string.Format("Select @Count = COUNT(SeriesAndPassNum) From Customers Where SeriesAndPassNum = '{0}'", _oldPass);
                    SqlCommand cmdFindUser = new SqlCommand(query, _connection);
                    SqlParameter parameter = new SqlParameter("@Count", SqlDbType.Int);
                    parameter.Direction = ParameterDirection.Output;
                    cmdFindUser.Parameters.Add(parameter);
                    cmdFindUser.ExecuteNonQuery();

                    findUserResult = (int)parameter.Value;

                    if (findUserResult == 1)
                    {
                        CarRentalDbWorker.EditCustomer(_connection, _oldPass, PassBox.Text, LNameBox.Text, FNameBox.Text, MNameBox.Text);
                        _oldPass = PassBox.Text;
                    }
                    else if (findUserResult == 0 &&
                            PassBox.Text != string.Empty &&
                            LNameBox.Text != string.Empty &&
                            FNameBox.Text != string.Empty)
                    {
                        CarRentalDbWorker.AddNewCustomer(_connection, PassBox.Text, LNameBox.Text, FNameBox.Text, MNameBox.Text);
                        _oldPass = PassBox.Text;
                        _customersRowCount = CarRentalDbWorker.TotalFormDataRowCount(_connection, "Customers");
                    }
                }
                
            }
            isPageSwitched = false;
        }

        private void ToBegin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPageSwitched = true;
            _customers.Clear();

            SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", 1, _connection);
            _customers.Load(reader);
            reader.Close();

            Title = string.Format("Клиент № {0}", _customers.Rows[0][0].ToString());

            PassBox.Text = _customers.Rows[0][1].ToString();
            _oldPass = PassBox.Text;

            LNameBox.Text = _customers.Rows[0][2].ToString();
            FNameBox.Text = _customers.Rows[0][3].ToString();
            MNameBox.Text = _customers.Rows[0][4].ToString();
        }

        private void ToEnd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPageSwitched = true;
            _customers.Clear();

            SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", _customersRowCount, _connection);
            _customers.Load(reader);
            reader.Close();

            Title = string.Format("Клиент № {0}", _customers.Rows[0][0].ToString());

            PassBox.Text = _customers.Rows[0][1].ToString();
            _oldPass = PassBox.Text;

            LNameBox.Text = _customers.Rows[0][2].ToString();
            FNameBox.Text = _customers.Rows[0][3].ToString();
            MNameBox.Text = _customers.Rows[0][4].ToString();
        }

        private void New_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _oldPass = string.Empty;

            Title = "Новый клиент";

            PassBox.Text = string.Empty;
            LNameBox.Text = string.Empty;
            FNameBox.Text = string.Empty;
            MNameBox.Text = string.Empty;
        }
    }
}
