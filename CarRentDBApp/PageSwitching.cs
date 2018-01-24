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
    public partial class MainWindow: Window
    {
        int _lastPage = 0;

        private void ExecutePage(string targetRequest, int pageNum, DataTable table, TabContentTemplate container)
        {
            SqlDataReader reader = null;

            if (targetRequest.Equals("RequestTimePeriod"))
                reader = _carRentalDb.ExecuteTimeRequest(pageNum, _connection);
            else
                reader = _carRentalDb.ExecuteTableCommand(targetRequest, pageNum, _connection);

            table.Clear();
            _carRentalDb.GetData(reader, table);

            container.DataGrid.ItemsSource = table.DefaultView;
        }

        private void PageSwitcherLogic(string targetProc, TabContentTemplate container, DataTable table, TextBlock button)
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
                        ExecutePage(targetProc, int.Parse(container.PageNum1.Text), table, container);
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
                    ExecutePage(targetProc, currentPage, table, container);
                    break;
                case "PageNum2":
                    currentPage = int.Parse(button.Text);
                    if (container.PageButton4.Visibility == Visibility.Hidden &&
                        container.PageButton5.Visibility == Visibility.Hidden &&
                        container.PageButton6.Visibility == Visibility.Hidden)
                        ExecutePage(targetProc, currentPage, table, container);
                    else
                    {
                        if (currentPage == 2 &&
                            container.PageButton4.Visibility != Visibility.Hidden &&
                            container.PageButton5.Visibility != Visibility.Hidden &&
                            container.PageButton6.Visibility != Visibility.Hidden)
                        {
                            container.Prev.Visibility = Visibility.Visible;
                        }

                        if (int.Parse(container.PageNum3.Text) == _lastPage)
                        {
                            container.Next.Visibility = Visibility.Hidden;
                            ExecutePage(targetProc, currentPage, table, container);
                        }
                        else {
                            container.PageNum1.Text = int.Parse(container.PageNum2.Text).ToString();
                            container.PageNum2.Text = int.Parse(container.PageNum3.Text).ToString();
                            container.PageNum3.Text = (int.Parse(container.PageNum3.Text) + 1).ToString();
                            ExecutePage(targetProc, currentPage, table, container);
                        }
                    }
                    break;
                case "PageNum3":
                    currentPage = int.Parse(button.Text);
                    if(_lastPage > 3)
                        container.Prev.Visibility = Visibility.Visible;
                    if (container.PageButton4.Visibility == Visibility.Hidden &&
                        container.PageButton5.Visibility == Visibility.Hidden &&
                        container.PageButton6.Visibility == Visibility.Hidden)
                        ExecutePage(targetProc, currentPage, table, container);
                    else {
                        if (currentPage == _lastPage)
                        {
                            container.Next.Visibility = Visibility.Hidden;
                            ExecutePage(targetProc, currentPage, table, container);
                        }
                        else if (currentPage != _lastPage - 1)
                        {
                            container.PageNum1.Text = int.Parse(container.PageNum3.Text).ToString();
                            container.PageNum2.Text = (int.Parse(container.PageNum2.Text) + 2).ToString();
                            container.PageNum3.Text = (int.Parse(container.PageNum1.Text) + 2).ToString();
                            ExecutePage(targetProc, currentPage, table, container);
                        }
                        else
                        {
                            container.PageNum1.Text = (int.Parse(container.PageNum1.Text) + 1).ToString();
                            container.PageNum2.Text = (int.Parse(container.PageNum2.Text) + 1).ToString();
                            container.PageNum3.Text = (int.Parse(container.PageNum3.Text) + 1).ToString();
                            ExecutePage(targetProc, currentPage, table, container);
                        }
                    }
                    break;
                case "PageNum4":
                    currentPage = int.Parse(button.Text);
                    ExecutePage(targetProc, currentPage, table, container);
                    break;
                case "PageNum5":
                    currentPage = int.Parse(button.Text);
                    ExecutePage(targetProc, currentPage, table, container);
                    break;
                case "PageNum6":
                    currentPage = int.Parse(button.Text);
                    ExecutePage(targetProc, currentPage, table, container);
                    break;
                case "NextPage":
                    if (int.Parse(container.PageNum3.Text) == _lastPage - 1)
                    {
                        container.Next.Visibility = Visibility.Hidden;
                        container.PageNum1.Text = (int.Parse(container.PageNum1.Text) + 1).ToString();
                        container.PageNum2.Text = (int.Parse(container.PageNum2.Text) + 1).ToString();
                        container.PageNum3.Text = (int.Parse(container.PageNum3.Text) + 1).ToString();

                        ExecutePage(targetProc, int.Parse(container.PageNum1.Text), table, container);
                        container.Prev.Visibility = Visibility.Visible;
                    }
                    else {
                        container.PageNum1.Text = int.Parse(container.PageNum2.Text).ToString();
                        container.PageNum2.Text = int.Parse(container.PageNum3.Text).ToString();
                        container.PageNum3.Text = (int.Parse(container.PageNum3.Text) + 1).ToString();
                        ExecutePage(targetProc, int.Parse(container.PageNum1.Text), table, container);
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
            if (rowCount % _carRentalDb.RowsPerPage == 0)
                pagesQuantity = rowCount / _carRentalDb.RowsPerPage;
            else
                pagesQuantity = rowCount / _carRentalDb.RowsPerPage + 1;

            return pagesQuantity;
        }

        private void InitPageControls(TabContentTemplate container, int pagesQuantity)
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

                _lastPage = int.Parse(container.PageNum6.Text);
            }
            else if (pagesQuantity == 6)
            {
                container.Next.Visibility = Visibility.Hidden;
                container.Prev.Visibility = Visibility.Hidden;
                container.PageNum4.Text = (pagesQuantity - 2).ToString();
                container.PageNum5.Text = (pagesQuantity - 1).ToString();
                container.PageNum6.Text = (pagesQuantity).ToString();

                _lastPage = int.Parse(container.PageNum6.Text);
            }
            else if (pagesQuantity == 5)
            {
                container.Prev.Visibility = Visibility.Hidden;
                container.PageButton6.Visibility = Visibility.Hidden;
                container.PageNum4.Text = (pagesQuantity - 1).ToString();
                container.PageNum5.Text = (pagesQuantity).ToString();

                _lastPage = int.Parse(container.PageNum5.Text);
            }
            else if (pagesQuantity == 4)
            {
                container.Prev.Visibility = Visibility.Hidden;
                container.PageButton6.Visibility = Visibility.Hidden;
                container.PageButton5.Visibility = Visibility.Hidden;
                container.PageNum4.Text = (pagesQuantity).ToString();

                _lastPage = int.Parse(container.PageNum4.Text);
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
