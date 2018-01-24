using System.Windows;
using System.Windows.Input;

namespace CarRentDBApp
{
    public partial class MainWindow : Window
    {
        CarsReportForm _carsReportForm;
        
        private void CarsReport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement report = (sender as FrameworkElement);

            if (e.ClickCount == 2)
            {
                if (_carsReportForm == null && report.Name == "CarsReport")
                    _carsReportForm = new CarsReportForm(_connection);
                else
                {
                    _carsReportForm.Close();
                    _carsReportForm = new CarsReportForm(_connection);
                }
            }
        }
    }

}
