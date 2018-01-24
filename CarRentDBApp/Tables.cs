using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarRentDBApp
{
    public partial class MainWindow : Window
    {
        TabItem _cars;
        TabItem _customers;
        TabItem _rent;

        TabContentTemplate _carsContainer;
        TabContentTemplate _customersContainer;
        TabContentTemplate _rentContainer;

        int _carsRowsQuantity;
        int _customersRowsQuantity;
        int _rentRowsQuantity;

        private void AddTableTab(string title)
        {
            _carsRowsQuantity = _carRentalDb.TotalTableRowCount(_carRentalDb.Connection, "Cars");
            _customersRowsQuantity = _carRentalDb.TotalTableRowCount(_carRentalDb.Connection, "Customers");
            _rentRowsQuantity = _carRentalDb.TotalTableRowCount(_carRentalDb.Connection, "Rent");

            int pagesQuantity;

            switch (title)
            {
                case "Автомобили":
                    
                    if (_carsContainer == null)
                    {
                        _carsContainer = InitTableTabContainer("Cars", _carRentalDb.Cars, _carsRowsQuantity);

                        InitCarsEditBtnsHandlers();
                        pagesQuantity = PagesCounter(_carsRowsQuantity);
                        InitPageControls(_carsContainer, pagesQuantity);
                    }
                    else
                    {
                        _carsContainer = null;
                        _carRentalDb.Cars.Clear();
                        _carsContainer = InitTableTabContainer("Cars", _carRentalDb.Cars, _carsRowsQuantity);

                        InitCarsEditBtnsHandlers();
                        pagesQuantity = PagesCounter(_carsRowsQuantity);
                        InitPageControls(_carsContainer, pagesQuantity);
                        _cars.Content = _carsContainer;
                    }
     
                    OpenTab(ref _cars, title, _carsContainer);
                    break;
                case "Клиенты":
                    if (_customersContainer == null)
                    {
                        _customersContainer = InitTableTabContainer("Customers", _carRentalDb.Customers, _customersRowsQuantity);

                        InitCustomerEditBtnsHandlers();
                        pagesQuantity = PagesCounter(_customersRowsQuantity);
                        InitPageControls(_customersContainer, pagesQuantity);

                    }
                    else
                    {
                        _customersContainer = null;
                        _carRentalDb.Customers.Clear();
                        _customersContainer = InitTableTabContainer("Customers", _carRentalDb.Customers, _customersRowsQuantity);

                        InitCustomerEditBtnsHandlers();
                        pagesQuantity = PagesCounter(_customersRowsQuantity);
                        InitPageControls(_customersContainer, pagesQuantity);
                        
                        _customers.Content = _customersContainer;
                    }

                    OpenTab(ref _customers, title, _customersContainer);
                    break;
                case "Прокат":
                    if (_rentContainer == null)
                    {
                        _rentContainer = InitTableTabContainer("Rent", _carRentalDb.Rent, _rentRowsQuantity);

                        InitRentBtnsHandlers();
                        pagesQuantity = PagesCounter(_rentRowsQuantity);
                        InitPageControls(_rentContainer, pagesQuantity);
                    }
                    else
                    {
                        _rentContainer = null;
                        _carRentalDb.Rent.Clear();
                        _rentContainer = InitTableTabContainer("Rent", _carRentalDb.Rent, _rentRowsQuantity);

                        InitRentBtnsHandlers();
                        pagesQuantity = PagesCounter(_rentRowsQuantity);
                        InitPageControls(_rentContainer, pagesQuantity);
                        
                        _rent.Content = _rentContainer;
                    }

                    OpenTab(ref _rent, title, _rentContainer);
                    break;
                default:
                    break;
            }
        }

        public TabContentTemplate InitTableTabContainer(string tableName, DataTable table, int rowsQuantity)
        {
            TabContentTemplate container = null;

            if (container == null)
            {
                container = new TabContentTemplate();
                container.Container.Children.Remove(container.TimeParams);
                container.Container.Children.Remove(container.GovNumParams);
                if (!isAdmin)
                {
                    container.AddButton.Visibility = Visibility.Collapsed;
                    container.EditButton.Visibility = Visibility.Collapsed;
                    container.DeleteButon.Visibility = Visibility.Collapsed;
                }

                SqlDataReader reader = _carRentalDb.ExecuteTableCommand("SelectFrom" + tableName, 1, _connection);
                _carRentalDb.GetData(reader, table);
                container.DataGrid.ItemsSource = table.DefaultView;

                MouseButtonEventHandler handler = (object sender, MouseButtonEventArgs e) =>
                {
                    TextBlock button = sender as TextBlock;

                    int rowCount = _carRentalDb.TotalTableRowCount(_connection, tableName);

                    if (!rowCount.Equals(rowsQuantity))
                    {
                        AddTableTab(table.TableName);
                    }

                    PageSwitcherLogic("SelectFrom"+tableName, container, table, button);
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

        private void TablePanel_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            string title;
            
            if (e.ClickCount == 2)
            {
                title = (sender as StackPanel).Uid;
                AddTableTab(title);
            }

        }

        private void InitCarsEditBtnsHandlers()
        {
            _carsContainer.AddB.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                NewCarRecordDialog newCarDialog = new NewCarRecordDialog(_connection);
                if (newCarDialog.DialogResult == true)
                    AddTableTab(_carRentalDb.Cars.TableName);
            };
            _carsContainer.EditB.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                DataRowView row = null;

                try
                {
                    row = _carsContainer.DataGrid.SelectedItem as DataRowView;
                    NewCarRecordDialog editCar = new NewCarRecordDialog(_connection, row);
                    if (editCar.DialogResult == true)
                        AddTableTab(_carRentalDb.Cars.TableName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не выбрано ни одной строки");
                }
            };

            _carsContainer.DeleteB.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                DataRowView row = null;
                try
                {
                    row = _carsContainer.DataGrid.SelectedItem as DataRowView;

                    string message = "Удалить из базы автомобиль Гос.Номер: " + row[1].ToString() + "?";
                    string caption = "Удалить запись";
                    MessageBoxButton buttons = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Question;
                    MessageBoxResult defaultResult = MessageBoxResult.OK;
                    MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon, defaultResult);

                    if (result == MessageBoxResult.OK)
                    {
                        CarRentalDbWorker.DeleteCar(_connection, row[1].ToString());
                        AddTableTab(_carRentalDb.Cars.TableName);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Не выбрано ни одной строки");
                }
            };

            _carsContainer.NewRentB.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                DataRowView row = null;

                try
                {
                    row = _carsContainer.DataGrid.SelectedItem as DataRowView;
                    FormalizeRent NewRent = new FormalizeRent(_connection, row);
                    if (NewRent.DialogResult == true)
                        AddTableTab(_carRentalDb.Cars.TableName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не выбрано ни одной строки");
                }
            };
        }

        private void InitCustomerEditBtnsHandlers()
        {
            _customersContainer.EditingBlock.Children.Remove(_customersContainer.NewRent);
            _customersContainer.EditingBlock.Children.Remove(_customersContainer.AddButton);

            _customersContainer.EditB.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                DataRowView row = null;

                try
                {
                    row = _customersContainer.DataGrid.SelectedItem as DataRowView;
                    EditCustomerDialog EditCustomer = new EditCustomerDialog(_connection, row);
                    if (EditCustomer.DialogResult == true)
                        AddTableTab(_carRentalDb.Customers.TableName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не выбрано ни одной строки");
                }
            };

            _customersContainer.DeleteB.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                DataRowView row = null;
                try
                {
                    row = _customersContainer.DataGrid.SelectedItem as DataRowView;

                    string message = "Удалить из базы запись о клиенте Серия и номер паспорта: " + row[1].ToString() + "?";
                    string caption = "Удалить запись";
                    MessageBoxButton buttons = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Question;
                    MessageBoxResult defaultResult = MessageBoxResult.OK;
                    MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon, defaultResult);

                    if (result == MessageBoxResult.OK)
                    {
                        CarRentalDbWorker.DeleteCustomer(_connection, row[1].ToString());
                        AddTableTab(_carRentalDb.Customers.TableName);
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("Не выбрано ни одной строки");
                }
            };
        }

        private void InitRentBtnsHandlers()
        {
            _rentContainer.EditingBlock.Children.Remove(_rentContainer.NewRent);
            _rentContainer.EditingBlock.Children.Remove(_rentContainer.AddButton);

            _rentContainer.EditB.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                DataRowView row = null;

                try
                {
                    row = _rentContainer.DataGrid.SelectedItem as DataRowView;
                    EditRentDialog EditRent = new EditRentDialog(_connection, row);
                    if (EditRent.DialogResult == true)
                        AddTableTab(_carRentalDb.Rent.TableName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не выбрано ни одной строки");
                }
            };

            _rentContainer.DeleteB.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                DataRowView row = null;
                try
                {
                    row = _rentContainer.DataGrid.SelectedItem as DataRowView;

                    string message = "Удалить из базы запись о прокате №: " + row[1].ToString() + "?";
                    string caption = "Удалить запись";
                    MessageBoxButton buttons = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Question;
                    MessageBoxResult defaultResult = MessageBoxResult.OK;
                    MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon, defaultResult);

                    if (result == MessageBoxResult.OK)
                    {
                        CarRentalDbWorker.DeleteRent(_connection, int.Parse(row[1].ToString()));
                        AddTableTab(_carRentalDb.Rent.TableName);
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("Не выбрано ни одной строки");
                }
            };
        }
    }
}
