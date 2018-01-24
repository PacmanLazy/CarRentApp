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
    /// Логика взаимодействия для ClientRentForm.xaml
    /// </summary>
    public partial class CustomerRentForm : Window
    {
        SqlConnection _connection;

        DataTable _customers;
        DataTable _rents;
        DataTable _carsCatalog;

        DataRowView _carCatalogRow;

        int _carsLastPage;

        int _currentPage = 1;

        int _customersRowCount = 0;
        int _rentsRowCount = 0;
        int _carsCatalogRowCount = 0;

        string _oldPass;
        bool isPageSwitched;

        public CustomerRentForm(SqlConnection connection)
        {
            InitializeComponent();
            CarsCatalogBlock.Visibility = Visibility.Collapsed;
            
            _connection = connection;
            _customers = new DataTable();
            _rents = new DataTable();
            _carsCatalog = new DataTable();

            _customersRowCount = CarRentalDbWorker.TotalFormDataRowCount(_connection, "Customers");
            _rentsRowCount = CarRentalDbWorker.TotalFormDataRowCount(_connection, "Rent");
            _carsCatalogRowCount = CarRentalDbWorker.TotalFormDataRowCount(_connection, "Cars");

            SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", _currentPage, _connection);
            _customers.Load(reader);
            reader.Close();

            Title = string.Format("Клиент № {0}", _customers.Rows[0][0].ToString());

            PassBox.Text = _customers.Rows[0][1].ToString();
            _oldPass = PassBox.Text;
            ////
            SqlDataReader readerRent = CarRentalDbWorker.ExecuteFormRentDataCommand(_oldPass, _connection);
            _rents.Load(readerRent);
            readerRent.Close();

            DataGrid.ItemsSource = _rents.DefaultView;
            ////
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
                _rents.Clear();

                SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", _currentPage - 1, _connection);
                _customers.Load(reader);
                reader.Close();

                PassBox.Text = _customers.Rows[0][1].ToString();
                _oldPass = PassBox.Text;
                ////
                SqlDataReader readerRent = CarRentalDbWorker.ExecuteFormRentDataCommand(_oldPass, _connection);
                _rents.Load(readerRent);
                readerRent.Close();

                DataGrid.ItemsSource = _rents.DefaultView;
                ////
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
            _rents.Clear();

            if (_currentPage != _customersRowCount)
            {
                SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", _currentPage + 1, _connection);
                _customers.Load(reader);
                reader.Close();

                PassBox.Text = _customers.Rows[0][1].ToString();
                _oldPass = PassBox.Text;
                ////
                SqlDataReader readerRent = CarRentalDbWorker.ExecuteFormRentDataCommand(_oldPass, _connection);
                _rents.Load(readerRent);
                readerRent.Close();

                DataGrid.ItemsSource = _rents.DefaultView;
                ////
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
            //else if(!isPageSwitched && _oldPass == string.Empty &&
            //        PassBox.Text == string.Empty &&
            //        LNameBox.Text == string.Empty &&
            //        FNameBox.Text == string.Empty)
            //{
            //    Title = "asdfs";
            //    int findUserResult = 0;
            //    if(PassBox.Text != string.Empty)
            //    {
            //        string query = string.Format("Select @Count = COUNT(SeriesAndPassNum) From Customers Where SeriesAndPassNum = '{0}'", _oldPass);
            //        SqlCommand cmdFindUser = new SqlCommand(query, _connection);
            //        SqlParameter parameter = new SqlParameter("@Count", SqlDbType.Int);
            //        parameter.Direction = ParameterDirection.Output;
            //        cmdFindUser.Parameters.Add(parameter);
            //        cmdFindUser.ExecuteNonQuery();

            //        findUserResult = (int)parameter.Value;

            //        if (findUserResult == 1)
            //        {
            //            query = string.Format("Select LName, FName, MName From Customers Where SeriesAndPassNum= '{0}'", PassBox.Text);
            //            cmdFindUser = new SqlCommand(query, _connection);
            //            SqlDataReader reader = cmdFindUser.ExecuteReader();

            //            while (reader.Read())
            //            {
            //                LNameBox.Text = reader[0].ToString();
            //                FNameBox.Text = reader[1].ToString();
            //                MNameBox.Text = reader[2].ToString();
            //            }
            //            reader.Close();
            //            CarRentalDbWorker.EditCustomer(_connection, _oldPass, PassBox.Text, LNameBox.Text, FNameBox.Text, MNameBox.Text);
            //            _oldPass = PassBox.Text;
            //        }
            //    }
            //}

            isPageSwitched = false;
        }

        private void ToBegin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPageSwitched = true;
            _customers.Clear();
            _rents.Clear();

            SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", 1, _connection);
            _customers.Load(reader);
            reader.Close();

            Title = string.Format("Клиент № {0}", _customers.Rows[0][0].ToString());

            PassBox.Text = _customers.Rows[0][1].ToString();
            _oldPass = PassBox.Text;
            ////
            SqlDataReader readerRent = CarRentalDbWorker.ExecuteFormRentDataCommand(_oldPass, _connection);
            _rents.Load(readerRent);
            readerRent.Close();

            DataGrid.ItemsSource = _rents.DefaultView;
            ////
            LNameBox.Text = _customers.Rows[0][2].ToString();
            FNameBox.Text = _customers.Rows[0][3].ToString();
            MNameBox.Text = _customers.Rows[0][4].ToString();
        }

        private void ToEnd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPageSwitched = true;
            _customers.Clear();
            _rents.Clear();

            SqlDataReader reader = CarRentalDbWorker.ExecuteFormDataCommand("SelectFromCustomers", _customersRowCount, _connection);
            _customers.Load(reader);
            reader.Close();

            Title = string.Format("Клиент № {0}", _customers.Rows[0][0].ToString());

            PassBox.Text = _customers.Rows[0][1].ToString();
            _oldPass = PassBox.Text;
            /////
            SqlDataReader readerRent = CarRentalDbWorker.ExecuteFormRentDataCommand(_oldPass, _connection);
            _rents.Load(readerRent);
            readerRent.Close();

            DataGrid.ItemsSource = _rents.DefaultView;
            /////
            LNameBox.Text = _customers.Rows[0][2].ToString();
            FNameBox.Text = _customers.Rows[0][3].ToString();
            MNameBox.Text = _customers.Rows[0][4].ToString();
        }

        private void New_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _oldPass = string.Empty;
            _currentPage = _customersRowCount;
            _rents.Clear();

            Title = "Новый клиент";

            PassBox.Text = string.Empty;
            LNameBox.Text = string.Empty;
            FNameBox.Text = string.Empty;
            MNameBox.Text = string.Empty;
        }

        private void CarInfoB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataRowView row = DataGrid.SelectedItem as DataRowView;

                new CarInfoForm(_connection, row[5].ToString());
            }
            catch(Exception)
            {
                MessageBox.Show("Выберите строку с данными о заказе");
            }
        }

        private void NewRentB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CarsCatalogBlock.Visibility = Visibility.Visible;
            _carsCatalog.Clear();

            int pagesQuantity = 0;

            ExecutePage(_connection, "SelectFromCars", 1, _carsCatalog, CarsDataGrid);

            MouseButtonEventHandler handler = (object sender1, MouseButtonEventArgs e1) =>
            {
                TextBlock button = sender1 as TextBlock;

                int rowCount = CarRentalDbWorker.TotalFormDataRowCount(_connection, "Cars");

                if (!rowCount.Equals(_carsCatalogRowCount))
                {
                    ExecutePage(_connection, "SelectFromCars", 1, _carsCatalog, CarsDataGrid);
                    pagesQuantity = PagesCounter(_carsCatalogRowCount);
                    InitPageControls(this, pagesQuantity);
                }

                PageSwitcherLogic("SelectFromCars", this, _carsCatalog, button);
            };

            PageNum1.MouseLeftButtonDown += handler;
            PageNum2.MouseLeftButtonDown += handler;
            PageNum3.MouseLeftButtonDown += handler;
            PageNum4.MouseLeftButtonDown += handler;
            PageNum5.MouseLeftButtonDown += handler;
            PageNum6.MouseLeftButtonDown += handler;
            NextPage.MouseLeftButtonDown += handler;
            PrevPage.MouseLeftButtonDown += handler;

            pagesQuantity = PagesCounter(_carsCatalogRowCount);
            InitPageControls(this, pagesQuantity);
        }

        private void SubmitRent_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            byte rentDays = 0;

            try
            {
                _carCatalogRow = CarsDataGrid.SelectedItem as DataRowView;

                if (byte.TryParse(RentDaysBox.Text, out rentDays) && 
                    rentDays != 0 &&
                    PassBox.Text != string.Empty && 
                    LNameBox.Text != string.Empty &&
                    FNameBox.Text != string.Empty)
                {
                    CarRentalDbWorker.AddNewRent(_connection, rentDays, PassBox.Text, _carCatalogRow[1].ToString());
                    new PrintRentForm(_connection);

                    SqlDataReader readerRent = CarRentalDbWorker.ExecuteFormRentDataCommand(_oldPass, _connection);
                    _rents.Clear();
                    _rents.Load(readerRent);
                    readerRent.Close();

                    CarsCatalogBlock.Visibility = Visibility.Collapsed;

                    DataGrid.ItemsSource = _rents.DefaultView;
                }
                else
                {
                    MessageBox.Show("Неверно введены данные");
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Выберите авто из каталога");
            }
        }

        private void RentDaysBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte rentDays = 0;
            decimal totalRentPrice = 0;

            try
            {
                _carCatalogRow = CarsDataGrid.SelectedItem as DataRowView;

                if (byte.TryParse(RentDaysBox.Text, out rentDays) && rentDays != 0)
                {
                    totalRentPrice = (decimal.Parse(_carCatalogRow[5].ToString()) * rentDays) + (decimal.Parse(_carCatalogRow[6].ToString()) * (decimal)0.1);
                    TotalPrice.Text = string.Format("Стоимость проката: {0:0.00}", totalRentPrice);
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Выберите авто из каталога");
            }
        }

        private void Cancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CarsCatalogBlock.Visibility = Visibility.Collapsed;
        }

        private void ExecutePage(SqlConnection connection, string targetRequest, int pageNum, DataTable table, DataGrid dataGrid)
        {
            SqlCommand cmd = new SqlCommand(targetRequest, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PageNumber", pageNum);
            cmd.Parameters.AddWithValue("@RowspPage", 3);

            SqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();

            table.Clear();
            table.Load(reader);
            reader.Close();

            dataGrid.ItemsSource = table.DefaultView;
        }

        private void PageSwitcherLogic(string targetProc, CustomerRentForm container,  DataTable table, TextBlock button)
        {
            int currentPage;

            switch (button.Name)
            {
                case "PrevPage":
                    if (int.Parse(container.PageNum1.Text) != 1)
                    {
                        container.PageNum1.Text = (int.Parse(container.PageNum1.Text) - 1).ToString();
                        container.PageNum2.Text = (int.Parse(container.PageNum2.Text) - 1).ToString();
                        container.PageNum3.Text = (int.Parse(container.PageNum3.Text) - 1).ToString();
                        ExecutePage(_connection, targetProc, int.Parse(container.PageNum1.Text), table, container.CarsDataGrid);
                    }
                    if (int.Parse(container.PageNum1.Text) == 1)
                        container.Prev.Visibility = Visibility.Hidden;
                    if (container.Next.Visibility == Visibility.Hidden)
                        container.Next.Visibility = Visibility.Visible;
                    break;
                case "PageNum1":
                    currentPage = int.Parse(button.Text);
                    if (currentPage == 1)
                        container.Prev.Visibility = Visibility.Hidden;
                    ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                    break;
                case "PageNum2":
                    currentPage = int.Parse(button.Text);
                    if (container.PageButton4.Visibility == Visibility.Hidden &&
                        container.PageButton5.Visibility == Visibility.Hidden &&
                        container.PageButton6.Visibility == Visibility.Hidden)
                        ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                    else
                    {
                        if (currentPage == 2 &&
                            container.PageButton4.Visibility != Visibility.Hidden &&
                            container.PageButton5.Visibility != Visibility.Hidden &&
                            container.PageButton6.Visibility != Visibility.Hidden)
                        {
                            container.Prev.Visibility = Visibility.Visible;
                        }

                        if (int.Parse(container.PageNum3.Text) == _carsLastPage)
                        {
                            container.Next.Visibility = Visibility.Hidden;
                            ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                        }
                        else {
                            container.PageNum1.Text = int.Parse(container.PageNum2.Text).ToString();
                            container.PageNum2.Text = int.Parse(container.PageNum3.Text).ToString();
                            container.PageNum3.Text = (int.Parse(container.PageNum3.Text) + 1).ToString();
                            ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                        }
                    }
                    break;
                case "PageNum3":
                    currentPage = int.Parse(button.Text);
                    if (_carsLastPage > 3)
                        container.Prev.Visibility = Visibility.Visible;
                    if (container.PageButton4.Visibility == Visibility.Hidden &&
                        container.PageButton5.Visibility == Visibility.Hidden &&
                        container.PageButton6.Visibility == Visibility.Hidden)
                        ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                    else {
                        if (currentPage == _carsLastPage)
                        {
                            container.Next.Visibility = Visibility.Hidden;
                            ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                        }
                        else if (currentPage != _carsLastPage - 1)
                        {
                            container.PageNum1.Text = int.Parse(container.PageNum3.Text).ToString();
                            container.PageNum2.Text = (int.Parse(container.PageNum2.Text) + 2).ToString();
                            container.PageNum3.Text = (int.Parse(container.PageNum1.Text) + 2).ToString();
                            ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                        }
                        else
                        {
                            container.PageNum1.Text = (int.Parse(container.PageNum1.Text) + 1).ToString();
                            container.PageNum2.Text = (int.Parse(container.PageNum2.Text) + 1).ToString();
                            container.PageNum3.Text = (int.Parse(container.PageNum3.Text) + 1).ToString();
                            ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                        }
                    }
                    break;
                case "PageNum4":
                    currentPage = int.Parse(button.Text);
                    ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                    break;
                case "PageNum5":
                    currentPage = int.Parse(button.Text);
                    ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                    break;
                case "PageNum6":
                    currentPage = int.Parse(button.Text);
                    ExecutePage(_connection, targetProc, currentPage, table, container.CarsDataGrid);
                    break;
                case "NextPage":
                    if (int.Parse(container.PageNum3.Text) == _carsLastPage - 1)
                    {
                        container.Next.Visibility = Visibility.Hidden;
                        container.PageNum1.Text = (int.Parse(container.PageNum1.Text) + 1).ToString();
                        container.PageNum2.Text = (int.Parse(container.PageNum2.Text) + 1).ToString();
                        container.PageNum3.Text = (int.Parse(container.PageNum3.Text) + 1).ToString();

                        ExecutePage(_connection, targetProc, int.Parse(container.PageNum1.Text), table, container.CarsDataGrid);
                        container.Prev.Visibility = Visibility.Visible;
                    }
                    else {
                        container.PageNum1.Text = int.Parse(container.PageNum2.Text).ToString();
                        container.PageNum2.Text = int.Parse(container.PageNum3.Text).ToString();
                        container.PageNum3.Text = (int.Parse(container.PageNum3.Text) + 1).ToString();
                        ExecutePage(_connection, targetProc, int.Parse(container.PageNum1.Text), table, container.CarsDataGrid);
                        container.Prev.Visibility = Visibility.Visible;
                    }
                    break;
                default:
                    break;
            }
        }

        private int PagesCounter(int rowCount)
        {
            int pagesQuantity;
            if (rowCount % 3 == 0)
                pagesQuantity = rowCount / 3;
            else
                pagesQuantity = rowCount / 3 + 1;

            return pagesQuantity;
        }

        private void InitPageControls(CustomerRentForm container, int pagesQuantity)
        {
            container.Prev.Visibility = Visibility.Hidden;

            container.PageButton1.Visibility = Visibility.Visible;
            container.PageButton2.Visibility = Visibility.Visible;
            container.PageButton3.Visibility = Visibility.Visible;
            container.Splitter.Visibility = Visibility.Visible;
            container.PageButton4.Visibility = Visibility.Visible;
            container.PageButton5.Visibility = Visibility.Visible;
            container.PageButton6.Visibility = Visibility.Visible;

            container.PageNum1.Text = 1.ToString();
            container.PageNum2.Text = 2.ToString();
            container.PageNum3.Text = 3.ToString();

            container.Next.Visibility = Visibility.Visible;

            if (pagesQuantity > 6)
            {
                container.PageNum4.Text = (pagesQuantity - 2).ToString();
                container.PageNum5.Text = (pagesQuantity - 1).ToString();
                container.PageNum6.Text = (pagesQuantity).ToString();

                _carsLastPage = int.Parse(container.PageNum6.Text);
            }
            else if (pagesQuantity == 6)
            {
                container.Next.Visibility = Visibility.Hidden;
                container.Prev.Visibility = Visibility.Hidden;
                container.PageNum4.Text = (pagesQuantity - 2).ToString();
                container.PageNum5.Text = (pagesQuantity - 1).ToString();
                container.PageNum6.Text = (pagesQuantity).ToString();

                _carsLastPage = int.Parse(container.PageNum6.Text);
            }
            else if (pagesQuantity == 5)
            {
                container.Prev.Visibility = Visibility.Hidden;
                container.PageButton6.Visibility = Visibility.Hidden;
                container.PageNum4.Text = (pagesQuantity - 1).ToString();
                container.PageNum5.Text = (pagesQuantity).ToString();

                _carsLastPage = int.Parse(container.PageNum5.Text);
            }
            else if (pagesQuantity == 4)
            {
                container.Prev.Visibility = Visibility.Hidden;
                container.PageButton6.Visibility = Visibility.Hidden;
                container.PageButton5.Visibility = Visibility.Hidden;
                container.PageNum4.Text = (pagesQuantity).ToString();

                _carsLastPage = int.Parse(container.PageNum4.Text);
            }
            else if (pagesQuantity == 3)
            {
                container.Next.Visibility = Visibility.Hidden;
                container.Prev.Visibility = Visibility.Hidden;
                container.PageButton6.Visibility = Visibility.Hidden;
                container.PageButton5.Visibility = Visibility.Hidden;
                container.PageButton4.Visibility = Visibility.Hidden;
                container.Splitter.Visibility = Visibility.Hidden;
            }
            else if (pagesQuantity == 2)
            {
                container.Next.Visibility = Visibility.Hidden;
                container.Prev.Visibility = Visibility.Hidden;
                container.PageButton6.Visibility = Visibility.Hidden;
                container.PageButton5.Visibility = Visibility.Hidden;
                container.PageButton4.Visibility = Visibility.Hidden;
                container.PageButton3.Visibility = Visibility.Hidden;
                container.Splitter.Visibility = Visibility.Hidden;
            }
            else if (pagesQuantity <= 1)
            {
                container.PagingBlock.Visibility = Visibility.Hidden;
            }
        }
    }
}
