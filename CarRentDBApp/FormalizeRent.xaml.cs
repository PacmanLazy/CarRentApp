using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Data;

namespace CarRentDBApp
{
    /// <summary>
    /// Логика взаимодействия для FormalizeRent.xaml
    /// </summary>
    public partial class FormalizeRent : Window
    {
        SqlConnection _connection;

        string _govNum;

        DataRowView _row;

        public FormalizeRent(SqlConnection connection, DataRowView row)
        {
            InitializeComponent();

            _connection = connection;
            _row = row;

            CustomerInDb.Visibility = Visibility.Collapsed;

            TextBlock GovNumData = new TextBlock();
            GovNumData.Margin = new Thickness(4);
            GovNumData.HorizontalAlignment = HorizontalAlignment.Center;
            GovNumData.Text = row[1].ToString();
            _govNum = row[1].ToString();

            TextBlock ModelData = new TextBlock();
            ModelData.Margin = new Thickness(4);
            ModelData.HorizontalAlignment = HorizontalAlignment.Center;
            ModelData.Text = row[2].ToString();

            TextBlock ColorData = new TextBlock();
            ColorData.Margin = new Thickness(4);
            ColorData.HorizontalAlignment = HorizontalAlignment.Center;
            ColorData.Text = row[3].ToString();

            TextBlock YearData = new TextBlock();
            YearData.Margin = new Thickness(4);
            YearData.HorizontalAlignment = HorizontalAlignment.Center;
            YearData.Text = row[4].ToString();

            TextBlock RentPriceData = new TextBlock();
            RentPriceData.Margin = new Thickness(4);
            RentPriceData.HorizontalAlignment = HorizontalAlignment.Center;
            RentPriceData.Text = row[5].ToString();

            TextBlock WorthData = new TextBlock();
            WorthData.Margin = new Thickness(4);
            WorthData.HorizontalAlignment = HorizontalAlignment.Center;
            WorthData.Text = row[6].ToString();

            GovNumBlock.Children.Add(GovNumData);
            ModelBlock.Children.Add(ModelData);
            ColorBlock.Children.Add(ColorData);
            YearBlock.Children.Add(YearData);
            RentPriceBlock.Children.Add(RentPriceData);
            WorthBlock.Children.Add(WorthData);

            ShowActivated = true;
            ShowDialog();
        }

        private void OkButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            byte rentDays = 0;
            int findUserResult = 0;

            if (PassBox.Text != string.Empty)
            {
                string query = string.Format("Select @Count = COUNT(SeriesAndPassNum) From Customers Where SeriesAndPassNum = '{0}'", PassBox.Text);
                SqlCommand cmdFindUser = new SqlCommand(query, _connection);
                SqlParameter parameter = new SqlParameter("@Count", SqlDbType.Int);
                parameter.Direction = ParameterDirection.Output;
                cmdFindUser.Parameters.Add(parameter);
                cmdFindUser.ExecuteNonQuery();

                findUserResult = (int)parameter.Value;
            }

            if (findUserResult == 1 && byte.TryParse(RentDaysBox.Text, out rentDays) && rentDays != 0)
            {
                CarRentalDbWorker.AddNewRent(_connection, rentDays, PassBox.Text, _govNum);

                new PrintRentForm(_connection);
                DialogResult = true;
                Close();
            }
            else {
                if (PassBox.Text != string.Empty &&
                   LNameBox.Text != string.Empty &&
                   FNameBox.Text != string.Empty &&
                   byte.TryParse(RentDaysBox.Text, out rentDays) && 
                   rentDays != 0)
                {
                    CarRentalDbWorker.AddNewCustomer(_connection, PassBox.Text, LNameBox.Text, FNameBox.Text, MNameBox.Text);
                    CarRentalDbWorker.AddNewRent(_connection, rentDays, PassBox.Text, _govNum);

                    new PrintRentForm(_connection);
                    DialogResult = true;
                    Close();
                }
                else
                    MessageBox.Show("Неверно введены данные");
            }

        }

        private void CancelButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void RentDaysBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte rentDays = 0;
            decimal totalRentPrice = 0;

            if (byte.TryParse(RentDaysBox.Text, out rentDays) && rentDays != 0)
            {
                totalRentPrice = (decimal.Parse(_row[5].ToString()) * rentDays) + (decimal.Parse(_row[6].ToString()) * (decimal)0.1);
                TotalPrice.Text = string.Format("Стоимость проката: {0:0.00}", totalRentPrice);
            }
        }

        private void PassBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int findUserResult = 0;
            SqlDataReader reader;

            if (PassBox.Text.Length == PassBox.MaxLength)
            {
                string query = string.Format("Select @Count = COUNT(SeriesAndPassNum) From Customers Where SeriesAndPassNum = '{0}'", PassBox.Text);
                SqlCommand cmdFindUser = new SqlCommand(query, _connection);
                SqlParameter parameter = new SqlParameter("@Count", SqlDbType.Int);
                parameter.Direction = ParameterDirection.Output;
                cmdFindUser.Parameters.Add(parameter);
                cmdFindUser.ExecuteNonQuery();

                findUserResult = (int)parameter.Value;

                if (findUserResult == 1)
                {
                    CustomerInDb.Visibility = Visibility.Visible;

                    query = string.Format("Select LName, FName, MName From Customers Where SeriesAndPassNum= '{0}'", PassBox.Text);
                    cmdFindUser = new SqlCommand(query, _connection);
                    reader = cmdFindUser.ExecuteReader();

                    while(reader.Read())
                    {
                        LNameBox.Text = reader[0].ToString();
                        FNameBox.Text = reader[1].ToString();
                        MNameBox.Text = reader[2].ToString();
                    }
                    reader.Close();
                }

            }
            else
            {
                CustomerInDb.Visibility = Visibility.Collapsed;

                LNameBox.Text = string.Empty;
                FNameBox.Text = string.Empty;
                MNameBox.Text = string.Empty;
            }
        }
    }
}
