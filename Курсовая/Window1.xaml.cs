using Coursework;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Курсовая
{
    public partial class Window1 : Window
    {
        private readonly StoreContext dbContext; // Замените на ваш контекст базы данных

        public Window1(StoreContext dbContext)
        {
            InitializeComponent();
            this.dbContext = dbContext;
            LoadData();
        }

        private void LoadData()
        {
            // Получаем данные из таблиц базы данных и привязываем их к DataGrid
            var users = dbContext.Users.ToList();
            var products = dbContext.Products.ToList();
            var categories = dbContext.Categories.ToList();
            var productCategories = dbContext.ProductCategories.ToList();
            var orders = dbContext.Orders.ToList();
            var orderItems = dbContext.OrderItems.ToList();

            // Создаем новый список для отображения в DataGrid
            var tableData = new List<object>();
            tableData.AddRange(users);
            tableData.AddRange(products);
            tableData.AddRange(categories);
            tableData.AddRange(productCategories);
            tableData.AddRange(orders);
            tableData.AddRange(orderItems);

            // Привязываем список к ItemsSource DataGrid
            dataGrid.ItemsSource = tableData;
        }

        // Обработчик нажатия кнопки для возврата к основному окну
        private void ReturnToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show(); // Показать основное окно
            Close(); // Закрыть текущее окно
        }
    }
}
