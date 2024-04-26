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
                                        .Include("OrderItem.Product")
                                        .Include("PaymentMethod")
                                        .ToList();

            DataGrid.ItemsSource = orderPayments;
        }

        private void LoadOrders()
        {
            var orderItems = context.OrderItems
                .Include(oi => oi.Product)
                .ToList();

            OrderComboBox.ItemsSource = orderItems;
            OrderComboBox.DisplayMemberPath = "Product.Name";
            OrderComboBox.SelectedValuePath = "OrderItemId";
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