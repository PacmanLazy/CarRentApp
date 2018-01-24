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
        TabItem _request1;
        TabItem _request2;
        TabItem _request3;
        TabItem _request4;
        TabItem _request5;
        
        TabContentTemplate _timeReqtContr;
        TabContentTemplate _carInfoReqtContr;
        TabContentTemplate _concreteCarModelReqtContr;
        TabContentTemplate _carsManufContr;
        TabContentTemplate _rentDetailsContr;

        int _timeRequestRowsQuantity;
        int _concreteCarModelRowsQuantity;
        int _carsManufRowsQuantity;
        int _rentDetailsRowsQuantity;

        private void AddRequestTab(string requestId)
        {
            _concreteCarModelRowsQuantity = _carRentalDb.TotalSimpleRequestRowCount(_connection, "ConcreteCarRowCount");
            _carsManufRowsQuantity = _carRentalDb.TotalSimpleRequestRowCount(_connection, "CarsManufBeforeCount");
            _rentDetailsRowsQuantity = _carRentalDb.TotalSimpleRequestRowCount(_connection, "RentDetailsCount");

            int pagesQuantity;
            switch (requestId)
            {
                case "1":
                    if (_rentDetailsContr == null)
                    {
                        _rentDetailsContr = InitSimpleReqtTabContainer("RentDetails",
                                                                    "RentDetailsCount",
                                                                    Request1.Content.ToString(),
                                                                    _carRentalDb.RentDetailsTable,
                                                                    _rentDetailsRowsQuantity);

                        pagesQuantity = PagesCounter(_rentDetailsRowsQuantity);
                        InitPageControls(_rentDetailsContr, pagesQuantity);
                    }
                    else
                    {
                        _rentDetailsContr = null;
                        _carRentalDb.RentDetailsTable.Clear();
                        _rentDetailsContr = InitSimpleReqtTabContainer("RentDetails",
                                                                    "RentDetailsCount",
                                                                    Request1.Content.ToString(),
                                                                    _carRentalDb.RentDetailsTable,
                                                                    _rentDetailsRowsQuantity);

                        pagesQuantity = PagesCounter(_rentDetailsRowsQuantity);
                        InitPageControls(_rentDetailsContr, pagesQuantity);

                        _request1.Content = _rentDetailsContr;
                    }

                    OpenTab(ref _request1, Request1.Content.ToString(), _rentDetailsContr);
                    break;
                case "2":
                    if (_carsManufContr == null)
                    {
                        _carsManufContr = InitSimpleReqtTabContainer("CarsManufBefore",
                                                                    "CarsManufBeforeCount",
                                                                    Request2.Content.ToString(),
                                                                    _carRentalDb.CarsManufTable,
                                                                    _carsManufRowsQuantity);

                        pagesQuantity = PagesCounter(_carsManufRowsQuantity);
                        InitPageControls(_carsManufContr, pagesQuantity);
                    }
                    else
                    {
                        _carsManufContr = null;
                        _carRentalDb.CarsManufTable.Clear();
                        _carsManufContr = InitSimpleReqtTabContainer("CarsManufBefore",
                                                                    "CarsManufBeforeCount",
                                                                    Request2.Content.ToString(),
                                                                    _carRentalDb.CarsManufTable,
                                                                    _carsManufRowsQuantity);

                        pagesQuantity = PagesCounter(_carsManufRowsQuantity);
                        InitPageControls(_carsManufContr, pagesQuantity);

                        _request2.Content = _carsManufContr;
                    }

                    OpenTab(ref _request2, Request2.Content.ToString(), _carsManufContr);
                    break;
                case "3":
                    if (_concreteCarModelReqtContr == null)
                    {
                        _concreteCarModelReqtContr = InitSimpleReqtTabContainer("ConcreteModelRequest", 
                                                                                "ConcreteCarRowCount", 
                                                                                Request3.Content.ToString(), 
                                                                                _carRentalDb.GetCarModelInfoTable, 
                                                                                _concreteCarModelRowsQuantity);

                        pagesQuantity = PagesCounter(_concreteCarModelRowsQuantity);
                        InitPageControls(_concreteCarModelReqtContr, pagesQuantity);
                    }
                    else
                    {
                        _concreteCarModelReqtContr = null;
                        _carRentalDb.GetCarModelInfoTable.Clear();
                        _concreteCarModelReqtContr = InitSimpleReqtTabContainer("ConcreteModelRequest", 
                                                                                "ConcreteCarRowCount", 
                                                                                Request3.Content.ToString(), 
                                                                                _carRentalDb.GetCarModelInfoTable, 
                                                                                _concreteCarModelRowsQuantity);

                        pagesQuantity = PagesCounter(_concreteCarModelRowsQuantity);
                        InitPageControls(_concreteCarModelReqtContr, pagesQuantity);

                        _request3.Content = _concreteCarModelReqtContr;
                    }

                    OpenTab(ref _request3, Request3.Content.ToString(), _concreteCarModelReqtContr);
                    break;
                case "4":
                    _carInfoReqtContr = new TabContentTemplate();
                    _carInfoReqtContr.EditingBlock.Visibility = Visibility.Collapsed;
                    _carInfoReqtContr.PagingBlock.Visibility = Visibility.Collapsed;
                    _carInfoReqtContr.TimeParams.Visibility = Visibility.Collapsed;

                    _carInfoReqtContr.GovNumB.MouseLeftButtonDown += GetGovNumHandler;

                    OpenTab(ref _request4, Request4.Content.ToString(), _carInfoReqtContr);
                    break;
                case "5":
                    if (_timeReqtContr == null)
                    {
                        _timeReqtContr = InitParamReqtTabContainer(TimeRequestPageSwitcherHandler);
                        _timeReqtContr.GovNumParams.Visibility = Visibility.Collapsed;
                        _timeReqtContr.PagingBlock.Visibility = Visibility.Collapsed;

                        _timeReqtContr.ExecTimeRequestB.MouseLeftButtonDown += GetTimePeriodHandler;
                    }
                    
                    OpenTab(ref _request5, Request5.Content.ToString(), _timeReqtContr);
                    break;
                default:
                    break;
            }
        }
        
        public TabContentTemplate InitParamReqtTabContainer(MouseButtonEventHandler handler)
        {
            TabContentTemplate container = null;

            if (container == null)
            {
                container = new TabContentTemplate();
                container.EditingBlock.Visibility = Visibility.Collapsed;

                container.PageNum1.MouseLeftButtonDown += handler;
                container.PageNum2.MouseLeftButtonDown += handler;
                container.PageNum3.MouseLeftButtonDown += handler;
                container.PageNum4.MouseLeftButtonDown += handler;
                container.PageNum5.MouseLeftButtonDown += handler;
                container.PageNum6.MouseLeftButtonDown += handler;
                container.NextPage.MouseLeftButtonDown += handler;
                container.PrevPage.MouseLeftButtonDown += handler;
            }

            return container;
        }

        public TabContentTemplate InitSimpleReqtTabContainer(string requestName, string rowsReqtCount, string tabTitle, DataTable table, int rowsQuantity)
        {
            TabContentTemplate container = null;

            if (container == null)
            {
                container = new TabContentTemplate();
                container.EditingBlock.Visibility = Visibility.Collapsed;
                container.TimeParams.Visibility = Visibility.Collapsed;
                container.GovNumParams.Visibility = Visibility.Collapsed;

                SqlDataReader reader = _carRentalDb.ExecuteTableCommand(requestName, 1, _connection);
                _carRentalDb.GetData(reader, table);
                container.DataGrid.ItemsSource = table.DefaultView;

                MouseButtonEventHandler handler = (object sender, MouseButtonEventArgs e) =>
                {
                    TextBlock button = sender as TextBlock;

                    int rowCount = _carRentalDb.TotalSimpleRequestRowCount(_connection, rowsReqtCount);

                    if (!rowCount.Equals(rowsQuantity))
                    {
                        AddTableTab(tabTitle);
                    }

                    PageSwitcherLogic(requestName, container, table, button);
                };

                container.PageNum1.MouseLeftButtonDown += handler;
                container.PageNum2.MouseLeftButtonDown += handler;
                container.PageNum3.MouseLeftButtonDown += handler;
                container.PageNum4.MouseLeftButtonDown += handler;
                container.PageNum5.MouseLeftButtonDown += handler;
                container.PageNum6.MouseLeftButtonDown += handler;
                container.NextPage.MouseLeftButtonDown += handler;
                container.PrevPage.MouseLeftButtonDown += handler;
            }

            return container;
        }

        private void RequestPanel_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            string id;

            if (e.ClickCount == 2)
            {
                id = (sender as StackPanel).Uid;
                AddRequestTab(id);
            }
        }

        private void TimeRequestPageSwitcherHandler(object sender, MouseButtonEventArgs e)
        {
            TextBlock button = sender as TextBlock;

            int rowCount = _carRentalDb.TotalTimeRequestRowCount(_connection);

            if (!rowCount.Equals(_timeRequestRowsQuantity))
            {
                AddTableTab(Request5.Content.ToString());
            }

            PageSwitcherLogic("RequestTimePeriod", _timeReqtContr, _carRentalDb.TimeRequestTable, button);
        }

        private void GetTimePeriodHandler(object sender, MouseButtonEventArgs e)
        {
            if (_timeReqtContr.StartDate.SelectedDate != null && _timeReqtContr.EndDate.SelectedDate != null)
            {
                _carRentalDb.TimeRequestTable.Clear();

                DateTime startTimePoint = _timeReqtContr.StartDate.SelectedDate.Value;
                DateTime endTimePoint = _timeReqtContr.EndDate.SelectedDate.Value;
                _carRentalDb.StartTimePoint = new DateTime(startTimePoint.Year, startTimePoint.Month, startTimePoint.Day);
                _carRentalDb.EndTimePoint = new DateTime(endTimePoint.Year, endTimePoint.Month, endTimePoint.Day);

                _timeRequestRowsQuantity = _carRentalDb.TotalTimeRequestRowCount(_connection);

                SqlDataReader reader = _carRentalDb.ExecuteTimeRequest(1, _connection);
                _carRentalDb.GetData(reader, _carRentalDb.TimeRequestTable);

                _timeReqtContr.DataGrid.ItemsSource = _carRentalDb.TimeRequestTable.DefaultView;

                int pagesQuantity = PagesCounter(_timeRequestRowsQuantity);
                
                _timeReqtContr.PagingBlock.Visibility = Visibility.Visible;
                InitPageControls(_timeReqtContr, pagesQuantity);
            }
        }

        private void GetGovNumHandler(object sender, MouseButtonEventArgs e)
        {
            _carRentalDb.GetCarInfoTable.Clear();

            string govNum = _carInfoReqtContr.GovNumBox.Text;

            SqlDataReader reader = _carRentalDb.ExecuteGetCarRequest(govNum, _connection);
            _carRentalDb.GetData(reader, _carRentalDb.GetCarInfoTable);

            _carInfoReqtContr.DataGrid.ItemsSource = _carRentalDb.GetCarInfoTable.DefaultView;
        }
    }
}
