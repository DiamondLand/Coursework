using Coursework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Курсовая
{
    public partial class OrderPaymentWindow : Window
    {
        private StoreContext context;
        private bool isEditing = false;

        public OrderPaymentWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadTable();
            LoadOrders();
            LoadPaymentMethods();
        }

        private void LoadTable()
        {
            var orderPayments = context.OrderPayments
                                        .Include("OrderItem")
                                        .Include("PaymentMethod")
                                        .ToList();

            // Загрузите только нужные данные, включая имя продукта
            var data = orderPayments.Select(op => new
            {
                op.OrderPaymentId,
                op.OrderId,
                op.OrderItem.Product.Name,
                op.PaymentMethodId,
                op.PaymentMethod,
                op.AmountPaid,
                op.PaymentDate
            }).ToList();

            DataGrid.ItemsSource = data;
        }

        private void LoadOrders()
        {
            var orders = context.Orders.ToList();
            OrderComboBox.ItemsSource = orders;
            OrderComboBox.DisplayMemberPath = "Name";
            OrderComboBox.SelectedValuePath = "ProductId";
        }

        private void LoadPaymentMethods()
        {
            var paymentMethods = context.PaymentMethods.ToList();
            OrderPaymentComboBox.ItemsSource = paymentMethods;
            OrderPaymentComboBox.DisplayMemberPath = "Name";
            OrderPaymentComboBox.SelectedValuePath = "PaymentMethodId";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (OrderComboBox.SelectedItem == null || OrderPaymentComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(AmountTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(AmountTextBox.Text, out int amount) || amount <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное количество оплаченных средств.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DataGrid.SelectedItem != null && isEditing)
            {
                // Обновляем существующую оплату заказа
                if (DataGrid.SelectedItem is OrderPayment selectedOrderPayment)
                {
                    selectedOrderPayment.OrderItem = (OrderItem)OrderComboBox.SelectedItem;
                    selectedOrderPayment.PaymentMethod = (PaymentMethod)OrderPaymentComboBox.SelectedItem;
                    selectedOrderPayment.AmountPaid = amount;
                    context.SaveChanges(); // Сохраняем изменения в базе данных
                    LoadTable();
                    ClearInputFields();
                    isEditing = false;
                }
            }
            else
            {
                // Добавляем новую оплату заказа
                OrderPayment newOrderPayment = new OrderPayment
                {
                    OrderItem = (OrderItem)OrderComboBox.SelectedItem,
                    PaymentMethod = (PaymentMethod)OrderPaymentComboBox.SelectedItem,
                    AmountPaid = amount
                };

                context.OrderPayments.Add(newOrderPayment);
                context.SaveChanges(); // Сохраняем изменения в базе данных
                LoadTable();
                ClearInputFields();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is OrderPayment selectedOrderPayment)
            {
                if (!isEditing)
                {
                    OrderComboBox.SelectedItem = selectedOrderPayment.OrderItem;
                    OrderPaymentComboBox.SelectedItem = selectedOrderPayment.PaymentMethod;
                    AmountTextBox.Text = selectedOrderPayment.AmountPaid.ToString();

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
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is OrderPayment selectedOrderPayment)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить оплату заказа {selectedOrderPayment.OrderPaymentId}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.OrderPayments.Remove(selectedOrderPayment);
                    context.SaveChanges();
                    LoadTable();
                }
            }
        }

        private void ClearInputFields()
        {
            OrderComboBox.SelectedItem = null;
            OrderPaymentComboBox.SelectedItem = null;
            AmountTextBox.Text = "";
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