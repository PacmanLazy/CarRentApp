using System.Data;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Reporting.WinForms;


namespace CarRentDBApp
{
    /// <summary>
    /// Логика взаимодействия для CarsReport.xaml
    /// </summary>
    public partial class CarsReportForm : Window
    {
        public CarsReportForm(SqlConnection connection)
        {
            InitializeComponent();

            DataTable carsData = CarRentalDbWorker.GetCarsReport(connection);
            

            ReportDataSource dataSource = new ReportDataSource("CarsReportData", carsData);
            CarsReportViewer.LocalReport.DataSources.Add(dataSource);
            CarsReportViewer.LocalReport.ReportEmbeddedResource = "CarRentDBApp.CarsReport.rdlc";
            CarsReportViewer.RefreshReport();

            Show();
        }
    }
}
