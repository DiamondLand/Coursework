using Coursework;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Курсовая
{
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();

            LoadReport1Data();
            LoadReport2Data();
        }

        // Метод для загрузки и отображения данных для первого отчета
        private void LoadReport1Data()
        {
            using (var context = new StoreContext())
            {
                var reportData = from order in context.Orders
                                 join orderItem in context.OrderItems
                                 on order.OrderId equals orderItem.OrderId
                                 join product in context.Products
                                 on orderItem.ProductId equals product.ProductId
                                 join user in context.Users
                                 on order.UserId equals user.UserId
                                 select new
                                 {
                                     OrderId = order.OrderId,
                                     ProductName = product.Name,
                                     Quantity = orderItem.Quantity,
                                     UserName = user.FirstName + " " + user.LastName
                                 };

                report1DataGrid.ItemsSource = reportData.ToList();
            }
        }

        // Метод для загрузки и отображения данных для второго отчета
        private void LoadReport2Data()
        {
            using (var context = new StoreContext())
            {
                var reportData = from product in context.Products
                                 join productCategory in context.ProductCategories
                                 on product.ProductId equals productCategory.ProductId
                                 join category in context.Categories
                                 on productCategory.CategoryId equals category.CategoryId
                                 select new
                                 {
                                     ProductName = product.Name,
                                     CategoryName = category.Name
                                 };

                report2DataGrid.ItemsSource = reportData.ToList();
            }
        }

        private void ReturnToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
