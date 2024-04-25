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
            var orderItems = context.OrderItems.Include("Order").Include("Product").ToList();
            DataGrid.ItemsSource = orderItems;
        }

        private void LoadOrders()
        {
            var orders = context.Orders.ToList();
            OrderComboBox.ItemsSource = orders;
            OrderComboBox.DisplayMemberPath = "OrderId";
            OrderComboBox.SelectedValuePath = "OrderId";
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
            if (OrderComboBox.SelectedItem == null || ProductComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(QuantityTextBox.Text))
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
                    selectedOrderItem.OrderId = (int)OrderComboBox.SelectedValue;
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
                    OrderId = (int)OrderComboBox.SelectedValue,
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
                    OrderComboBox.SelectedValue = selectedOrderItem.OrderId;
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is OrderItem selectedOrderItem)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить элемент заказа {selectedOrderItem.OrderItemId}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.OrderItems.Remove(selectedOrderItem);
                    context.SaveChanges();
                    LoadTable();
                }
            }
        }

        private void ClearInputFields()
        {
            OrderComboBox.SelectedItem = null;
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
