using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coursework
{
    public partial class UserWindow : Window
    {
        private StoreContext context;

        public UserWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = context.Users.ToList();
            UsersDataGrid.ItemsSource = users;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что все поля ввода заполнены
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "О-о-у", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверяем корректность email с помощью регулярного выражения
            if (!Regex.IsMatch(EmailTextBox.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Некорректный формат email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверяем, что номер телефона начинается с "+7"
            if (!PhoneTextBox.Text.StartsWith("+7") || PhoneTextBox.Text.Length != 12)
            {
                MessageBox.Show("Номер телефона должен начинаться с '+7' и состоять из 12 символов.", "О-о-у", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем нового пользователя из введённых данных
            User newUser = new User
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                Phone = PhoneTextBox.Text,
            };

            if (UsersDataGrid.SelectedItem != null)
            {
                // Если выбран пользователь из DataGrid, обновляем его данные
                User selectedUser = (User)UsersDataGrid.SelectedItem;
                selectedUser.FirstName = newUser.FirstName;
                selectedUser.LastName = newUser.LastName;
                selectedUser.Email = newUser.Email;
                selectedUser.Phone = newUser.Phone;
            }
            else
            {
                // Если не выбран ни один пользователь из DataGrid, добавляем нового пользователя в базу
                context.Users.Add(newUser);
            }

            // Сохраняем изменения в базе данных
            context.SaveChanges();

            // Перезагружаем данные, чтобы обновить DataGrid
            LoadUsers();

            // Очищаем поля ввода после добавления или обновления пользователя
            ClearInputFields();
        }

        private void UsersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, что выбранная строка не пуста и является объектом типа User
            if (UsersDataGrid.SelectedItem != null && UsersDataGrid.SelectedItem is User selectedUser)
            {
                // Заполняем поля ввода данными выбранного пользователя
                FirstNameTextBox.Text = selectedUser.FirstName;
                LastNameTextBox.Text = selectedUser.LastName;
                EmailTextBox.Text = selectedUser.Email;
                PhoneTextBox.Text = selectedUser.Phone;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem != null && UsersDataGrid.SelectedItem is User selectedUser)
            {
                // Показываем диалоговое окно подтверждения удаления
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {selectedUser.FirstName} {selectedUser.LastName}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Если пользователь подтвердил удаление, удаляем пользователя из базы данных
                if (result == MessageBoxResult.Yes)
                {
                    if (selectedUser.FirstName != null && selectedUser.LastName != null &&
                        selectedUser.Email != null && selectedUser.Phone != null)
                    {
                        context.Users.Remove(selectedUser);
                        context.SaveChanges();

                        // Перезагружаем данные, чтобы обновить DataGrid
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show("Удалить пустую ячейку не получится!", "О-о-у", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        private void ClearInputFields()
        {
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            EmailTextBox.Text = "";
            PhoneTextBox.Text = "";
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            context.Dispose();
        }
    }
}
