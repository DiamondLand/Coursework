using Coursework;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace Курсовая
{
    public partial class ShoppingAddressWindow : Window
    {
        private StoreContext context;
        private bool isEditing = false;

        public ShoppingAddressWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadTable();
            LoadUsers();
        }

        private void LoadTable()
        {
            var addresses = context.ShoppingAddresses.Include("User").ToList();
            DataGrid.ItemsSource = addresses;
        }

        private void LoadUsers()
        {
            var users = context.Users.ToList();
            UserComboBox.ItemsSource = users;
            UserComboBox.DisplayMemberPath = "FirstName";
            UserComboBox.SelectedValuePath = "UserId";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AddressLine1TextBox.Text) || string.IsNullOrWhiteSpace(CountryTextBox.Text) ||
                string.IsNullOrWhiteSpace(CityTextBox.Text) || UserComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(PostalCodeTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем новый адрес из введенных данных
            ShoppingAddress newAddress = new ShoppingAddress
            {
                AddressLine1 = AddressLine1TextBox.Text,
                AddressLine2 = AddressLine2TextBox.Text,
                Country = CountryTextBox.Text,
                City = CityTextBox.Text,
                User = (User)UserComboBox.SelectedItem,
                PostalCode = PostalCodeTextBox.Text
            };

            // Если выбран адрес из DataGrid, обновляем его данные
            if (DataGrid.SelectedItem != null & isEditing)
            {
                ShoppingAddress selectedAddress = (ShoppingAddress)DataGrid.SelectedItem;
                selectedAddress.AddressLine1 = newAddress.AddressLine1;
                selectedAddress.AddressLine2 = newAddress.AddressLine2;
                selectedAddress.Country = newAddress.Country;
                selectedAddress.City = newAddress.City;
                selectedAddress.User = newAddress.User;
                selectedAddress.PostalCode = newAddress.PostalCode;
            }
            else // Если создается новый адрес
            {
                context.ShoppingAddresses.Add(newAddress);
            }

            context.SaveChanges(); // Сохраняем изменения в базе данных
            LoadTable();
            ClearInputFields();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is ShoppingAddress selectedAddress)
            {
                if (!isEditing)
                {
                    AddressLine1TextBox.Text = selectedAddress.AddressLine1;
                    AddressLine2TextBox.Text = selectedAddress.AddressLine2;
                    CountryTextBox.Text = selectedAddress.Country;
                    CityTextBox.Text = selectedAddress.City;
                    UserComboBox.SelectedItem = selectedAddress.User;
                    PostalCodeTextBox.Text = selectedAddress.PostalCode;

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
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is ShoppingAddress selectedAddress)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить адрес {selectedAddress.AddressLine1}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.ShoppingAddresses.Remove(selectedAddress);
                    context.SaveChanges();
                    LoadTable();
                }
            }
        }

        private void ClearInputFields()
        {
            AddressLine1TextBox.Text = "";
            AddressLine2TextBox.Text = "";
            CountryTextBox.Text = "";
            CityTextBox.Text = "";
            UserComboBox.SelectedItem = null;
            PostalCodeTextBox.Text = "";
        }

        private void ReturnToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            context.Dispose();
        }
    }
}