using Coursework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Курсовая
{
    public partial class OrderItemWindow : Window
    {
        private StoreContext context;
        private bool isEditing = false;

        public OrderItemWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadTable();
            LoadOrders();
            LoadProducts();
        }
        private void LoadTable()
        {
            var orderItems = context.OrderItems
                                    .Include("Order")
                                    .Include("Product")
                                    .Include("Order.User")
                                    .ToList();
            DataGrid.ItemsSource = orderItems;
        }

        private void LoadOrders()
        {
            var ordersWithUsers = context.Orders
                .Include("User")
                .ToList();
            OrderUserComboBox.ItemsSource = ordersWithUsers;
            OrderUserComboBox.DisplayMemberPath = "User.FirstName";
            OrderUserComboBox.SelectedValuePath = "OrderId";
        }

        private void LoadProducts()
        {
            var products = context.Products.ToList();
            ProductComboBox.ItemsSource = products;
            ProductComboBox.DisplayMemberPath = "Name";
            ProductComboBox.SelectedValuePath = "ProductId";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (OrderUserComboBox.SelectedItem == null || ProductComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(QuantityTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное количество товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DataGrid.SelectedItem != null && isEditing)
            {
                // Обновляем существующий элемент заказа
                if (DataGrid.SelectedItem is OrderItem selectedOrderItem)
                {
                    selectedOrderItem.OrderId = (int)OrderUserComboBox.SelectedValue;
                    selectedOrderItem.ProductId = (int)ProductComboBox.SelectedValue;
                    selectedOrderItem.Quantity = quantity;
                    context.SaveChanges(); // Сохраняем изменения в базе данных
                    LoadTable();
                    ClearInputFields();
                    isEditing = false;
                }
            }
            else
            {
                // Добавляем новый элемент заказа
                OrderItem newOrderItem = new OrderItem
                {
                    OrderId = (int)OrderUserComboBox.SelectedValue,
                    ProductId = (int)ProductComboBox.SelectedValue,
                    Quantity = quantity
                };

                context.OrderItems.Add(newOrderItem);
                context.SaveChanges(); // Сохраняем изменения в базе данных
                LoadTable();
                ClearInputFields();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is OrderItem selectedOrderItem)
            {
                if (!isEditing)
                {
                    OrderUserComboBox.SelectedValue = selectedOrderItem.OrderId;
                    ProductComboBox.SelectedValue = selectedOrderItem.ProductId;
                    QuantityTextBox.Text = selectedOrderItem.Quantity.ToString();

                    isEditing = true;
                }
                else
                {
                    ClearInputFields();
                    isEditing = false;
                }
            }
        }

        private void ClearInputFields()
        {
            OrderUserComboBox.SelectedItem = null;
            ProductComboBox.SelectedItem = null;
            QuantityTextBox.Text = "";
        }

        private void ReturnToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            context.Dispose();
        }
    }
}
