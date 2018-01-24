using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

namespace CarRentDBApp
{
    /// <summary>
    /// Логика взаимодействия для NewCarRecordDialog.xaml
    /// </summary>
    public partial class NewCarRecordDialog : Window
    {
        SqlConnection _connection;

        string _oldGovNum;

        public NewCarRecordDialog(SqlConnection connection)
        {
            InitializeComponent();

            _connection = connection;
            ShowActivated = true;
            ShowDialog();
            
        }

        public NewCarRecordDialog(SqlConnection connection, DataRowView row)
        {
            InitializeComponent();

            TextFieldsBlock.Children.Remove(ModelBlock);
            TextFieldsBlock.Children.Remove(YearBlock);
            OkButton.MouseLeftButtonDown -= OkButton_MouseLeftButtonDown;
            OkButton.MouseLeftButtonDown += OkButtonForEdit_MouseLeftButtonDown;

            _oldGovNum = row[1].ToString();
            GovNumBox.Text = row[1].ToString();
            ColorBox.Text = row[3].ToString();
            PriceBox.Text =  row[5].ToString();
            WorthBox.Text = row[6].ToString();

            _connection = connection;
            ShowActivated = true;
            ShowDialog();

        }

        private void OkButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            short year = 0;
            decimal price = 0;
            decimal worth = 0;

            if (short.TryParse(YearBox.Text, out year) &&
                decimal.TryParse(PriceBox.Text, out price) &&
                decimal.TryParse(WorthBox.Text, out worth) == true &&
                YearBox.Text.Length == 4)
            {
                CarRentalDbWorker.AddNewCar(_connection,
                                            GovNumBox.Text,
                                            ModelBox.Text,
                                            ColorBox.Text,
                                            year,
                                            price,
                                            worth);
                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Неверно введены данные");
                
        }

        private void OkButtonForEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            decimal price = 0;
            decimal worth = 0;

            if (decimal.TryParse(PriceBox.Text, out price) &&
                decimal.TryParse(WorthBox.Text, out worth) == true)
            {
                CarRentalDbWorker.EditCar(_connection,
                                          _oldGovNum,
                                          GovNumBox.Text,
                                          ColorBox.Text,
                                          price,
                                          worth);
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
