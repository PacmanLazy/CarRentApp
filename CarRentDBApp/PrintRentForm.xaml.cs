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
using Microsoft.Reporting.WinForms;

namespace CarRentDBApp
{
    /// <summary>
    /// Логика взаимодействия для PrintRentForm.xaml
    /// </summary>
    public partial class PrintRentForm : Window
    {
        public PrintRentForm(SqlConnection connection)
        {
            InitializeComponent();

            RentDataViewer.Reset();

            DataTable rentData = CarRentalDbWorker.RentToPrintData(connection);
            string currentUser = rentData.Rows[0][rentData.Columns[3]].ToString();
            DataTable customerData = CarRentalDbWorker.RentCustomerToPrintData(connection, currentUser);
            string currentCar = rentData.Rows[0][rentData.Columns[4]].ToString();
            DataTable carsData = CarRentalDbWorker.RentCarToPrintData(connection, currentCar);

            ReportDataSource RentDataSource = new ReportDataSource("RentData", rentData);
            ReportDataSource CustomerDataSource = new ReportDataSource("CustomerData", customerData);
            ReportDataSource CarDataSource = new ReportDataSource("CarData", carsData);

            RentDataViewer.LocalReport.DataSources.Add(RentDataSource);
            RentDataViewer.LocalReport.DataSources.Add(CustomerDataSource);
            RentDataViewer.LocalReport.DataSources.Add(CarDataSource);

            RentDataViewer.LocalReport.ReportEmbeddedResource = "CarRentDBApp.RentReport.rdlc";
            RentDataViewer.RefreshReport();

            ShowActivated = true;
            Show();
        }
    }
}
