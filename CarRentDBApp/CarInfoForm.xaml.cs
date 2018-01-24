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
    /// Логика взаимодействия для CarInfoForm.xaml
    /// </summary>
    public partial class CarInfoForm : Window
    {
        //SqlConnection _connection;

        public CarInfoForm(SqlConnection connection, string govNum)
        {
            InitializeComponent();

            SqlDataReader reader = CarRentalDbWorker.GetCarInfo(govNum, connection);

            TextBlock GovNumData = new TextBlock();
            GovNumData.Margin = new Thickness(4);
            GovNumData.HorizontalAlignment = HorizontalAlignment.Center;
            
            TextBlock ModelData = new TextBlock();
            ModelData.Margin = new Thickness(4);
            ModelData.HorizontalAlignment = HorizontalAlignment.Center;
            
            TextBlock ColorData = new TextBlock();
            ColorData.Margin = new Thickness(4);
            ColorData.HorizontalAlignment = HorizontalAlignment.Center;
            
            TextBlock YearData = new TextBlock();
            YearData.Margin = new Thickness(4);
            YearData.HorizontalAlignment = HorizontalAlignment.Center;
            
            TextBlock RentPriceData = new TextBlock();
            RentPriceData.Margin = new Thickness(4);
            RentPriceData.HorizontalAlignment = HorizontalAlignment.Center;
            
            TextBlock WorthData = new TextBlock();
            WorthData.Margin = new Thickness(4);
            WorthData.HorizontalAlignment = HorizontalAlignment.Center;
            
            while(reader.Read())
            {
                GovNumData.Text = reader[0].ToString();
                ModelData.Text = reader[1].ToString();
                ColorData.Text = reader[2].ToString();
                YearData.Text = reader[3].ToString();
                RentPriceData.Text = reader[4].ToString();
                WorthData.Text = reader[5].ToString();
            }
            reader.Close();

            GovNumBlock.Children.Add(GovNumData);
            ModelBlock.Children.Add(ModelData);
            ColorBlock.Children.Add(ColorData);
            YearBlock.Children.Add(YearData);
            RentPriceBlock.Children.Add(RentPriceData);
            WorthBlock.Children.Add(WorthData);

            ShowActivated = true;
            ShowDialog();
        }
    }
}
