using Coursework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Курсовая
{
    public partial class SupplierWindow : Window
    {
        private StoreContext context;
        private bool isEditing = false;

        public SupplierWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadTable();
        }
        private void LoadTable()
        {
            var suppliers = context.Suppliers.ToList();
            DataGrid.ItemsSource = suppliers;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(ContactPersonTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(EmailTextBox.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Некорректный формат email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!PhoneTextBox.Text.StartsWith("+7") || PhoneTextBox.Text.Length != 12)
            {
                MessageBox.Show("Номер телефона должен начинаться с '+7' и состоять из 12 символов.", "О-о-у", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Supplier newSupplier = new Supplier
            {
                Name = NameTextBox.Text,
                ContactPerson = ContactPersonTextBox.Text,
                Email = EmailTextBox.Text,
                Phone = PhoneTextBox.Text
            };


            // Если выбран пользователь из DataGrid, обновляем его данные
            if (DataGrid.SelectedItem != null & isEditing)
            {
                Supplier selectedUser = (Supplier)DataGrid.SelectedItem;
                selectedUser.Name = newSupplier.Name;
                selectedUser.ContactPerson = newSupplier.ContactPerson;
                selectedUser.Email = newSupplier.Email;
                selectedUser.Phone = newSupplier.Phone;
            }
            else // Если создаётся новый пользователь
            {
                context.Suppliers.Add(newSupplier);
            }

            context.SaveChanges(); // Сохраняем изменения в базе данных
            LoadTable();
            ClearInputFields();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is Supplier selectedSupplier)
            {
                if (!isEditing)
                {
                    NameTextBox.Text = selectedSupplier.Name;
                    ContactPersonTextBox.Text = selectedSupplier.ContactPerson;
                    EmailTextBox.Text = selectedSupplier.Email;
                    PhoneTextBox.Text = selectedSupplier.Phone;

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
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is Supplier selectedSupplier)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить поставщика {selectedSupplier.Name}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.Suppliers.Remove(selectedSupplier);
                    context.SaveChanges();
                    LoadTable();
                }
            }
        }

        private void ClearInputFields()
        {
            NameTextBox.Text = "";
            ContactPersonTextBox.Text = "";
            EmailTextBox.Text = "";
            PhoneTextBox.Text = "";
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