using Coursework;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Курсовая
{
    public partial class PaymentMethodWindow : Window
    {
        private StoreContext context;
        private bool isEditing = false;

        public PaymentMethodWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadTable();
            LoadPaymentMethods();
        }
        private void LoadTable()
        {
            var paymentMethods = context.PaymentMethods.ToList();
            DataGrid.ItemsSource = paymentMethods;
        }

        private void LoadPaymentMethods()
        {
            var methods = new[] { "Кредитная карта", "Дебетовая карта", "PayPal", "Банковский перевод" };
            NameComboBox.ItemsSource = methods;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameComboBox.Text) || string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DataGrid.SelectedItem != null && isEditing)
            {
                // Обновляем существующий метод оплаты
                if (DataGrid.SelectedItem is PaymentMethod selectedMethod)
                {
                    selectedMethod.Name = NameComboBox.Text;
                    selectedMethod.Description = DescriptionTextBox.Text;
                    context.SaveChanges(); // Сохраняем изменения в базе данных
                    LoadTable();
                    ClearInputFields();
                    isEditing = false;
                }
            }
            else
            {
                // Добавляем новый метод оплаты
                PaymentMethod newMethod = new PaymentMethod
                {
                    Name = NameComboBox.Text,
                    Description = DescriptionTextBox.Text
                };

                context.PaymentMethods.Add(newMethod);
                context.SaveChanges(); // Сохраняем изменения в базе данных
                LoadTable();
                ClearInputFields();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is PaymentMethod selectedMethod)
            {
                if (!isEditing)
                {
                    NameComboBox.Text = selectedMethod.Name;
                    DescriptionTextBox.Text = selectedMethod.Description;

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
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is PaymentMethod selectedMethod)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить метод оплаты {selectedMethod.Name}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.PaymentMethods.Remove(selectedMethod);
                    context.SaveChanges();
                    LoadTable();
                }
            }
        }

        private void ClearInputFields()
        {
            NameComboBox.Text = "";
            DescriptionTextBox.Text = "";
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
