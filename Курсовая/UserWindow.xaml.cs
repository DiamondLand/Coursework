using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Coursework
{
    public partial class UserWindow : Window
    {
        private StoreContext context;

        public UserWindow()
        {
            InitializeComponent();
            UsersDataGrid.SelectionChanged += UsersDataGrid_SelectionChanged;
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

            // Добавляем пользователя в базу и сохраняем изменения
            context.Users.Add(newUser);
            context.SaveChanges();

            // Перезагружаем данные, чтобы обновить DataGrid
            LoadUsers();

            // Очищаем поля ввода после добавления пользователя
            ClearInputFields();
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem != null)
            {
                // Получаем выбранного пользователя из DataGrid
                User selectedUser = (User)UsersDataGrid.SelectedItem;

                // Показываем диалоговое окно подтверждения удаления
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {selectedUser.FirstName} {selectedUser.LastName}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Если пользователь подтвердил удаление, удаляем пользователя из базы данных
                if (result == MessageBoxResult.Yes)
                {
                    context.Users.Remove(selectedUser);
                    context.SaveChanges();

                    // Перезагружаем данные, чтобы обновить DataGrid
                    LoadUsers();
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
