using Coursework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Курсовая
{
    public partial class OrderWindow : Window
    {
        private StoreContext context;
        private bool isEditing = false;

        public OrderWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadTable();
            LoadUsers();
            LoadOrderStatuses();
        }

        private void LoadTable()
        {
            var orders = context.Orders.Include("User").ToList();
            DataGrid.ItemsSource = orders;
        }

        private void LoadUsers()
        {
            var users = context.Users.ToList();
            UserComboBox.ItemsSource = users;
            UserComboBox.DisplayMemberPath = "FirstName";
            UserComboBox.SelectedValuePath = "UserId";
        }

        private void LoadOrderStatuses()
        {
            var orderStatuses = new[] { "Новый", "В работе", "Выполнен", "Отменен" };
            OrderStatusComboBox.ItemsSource = orderStatuses;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null || OrderStatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите пользователя и статус заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DataGrid.SelectedItem != null && isEditing)
            {
                // Обновляем существующий заказ
                if (DataGrid.SelectedItem is Order selectedOrder)
                {
                    selectedOrder.User = (User)UserComboBox.SelectedItem;
                    selectedOrder.OrderStatus = OrderStatusComboBox.SelectedItem.ToString();
                    context.SaveChanges(); // Сохраняем изменения в базе данных
                    LoadTable();
                    ClearInputFields();
                    isEditing = false;
                }
            }
            else
            {
                // Добавляем новый заказ
                Order newOrder = new Order
                {
                    User = (User)UserComboBox.SelectedItem,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatusComboBox.SelectedItem.ToString()
                };

                context.Orders.Add(newOrder);
                context.SaveChanges(); // Сохраняем изменения в базе данных
                LoadTable();
                ClearInputFields();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is Order selectedOrder)
            {
                if (!isEditing)
                {
                    UserComboBox.SelectedItem = selectedOrder.User;
                    OrderStatusComboBox.SelectedItem = selectedOrder.OrderStatus;

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
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is Order selectedOrder)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить заказ {selectedOrder.OrderId}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.Orders.Remove(selectedOrder);
                    context.SaveChanges();
                    LoadTable();
                }
            }
        }

        private void ClearInputFields()
        {
            UserComboBox.SelectedItem = null;
            OrderStatusComboBox.SelectedItem = null;
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