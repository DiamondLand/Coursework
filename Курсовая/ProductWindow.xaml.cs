using Coursework;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Курсовая
{
    public partial class ProductWindow : Window
    {

        private StoreContext context;
        private bool isEditing = false;

        public ProductWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadTable();
        }

        private void LoadTable()
        {
            var products = context.Products.ToList();
            DataGrid.ItemsSource = products;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text) || string.IsNullOrWhiteSpace(QuantityInStockTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "О-о-у", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0 ||
                !int.TryParse(QuantityInStockTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Некорректный формат цены или количества.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем новый продукт из введённых данных
            Product newProduct = new Product
            {
                Name = NameTextBox.Text,
                Description = DescriptionTextBox.Text,
                Price = price,
                QuantityInStock = quantity
            };

            // Если выбран продукт из DataGrid, обновляем его данные
            if (DataGrid.SelectedItem != null & isEditing)
            {
                Product selectedProduct = (Product)DataGrid.SelectedItem;
                selectedProduct.Name = newProduct.Name;
                selectedProduct.Description = newProduct.Description;
                selectedProduct.Price = newProduct.Price;
                selectedProduct.QuantityInStock = newProduct.QuantityInStock;
            }
            else // Если создаётся новый продукт
            {
                context.Products.Add(newProduct);
            }

            context.SaveChanges(); // Сохраняем изменения в базе данных
            LoadTable();
            ClearInputFields();

            isEditing = false;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, что выбранная строка не пуста и является объектом типа Product
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is Product selectedProduct)
            {
                if (!isEditing) // Если не в режиме редактирования, начинаем редактирование
                {
                    // Заполняем поля ввода данными выбранного продукта
                    NameTextBox.Text = selectedProduct.Name;
                    DescriptionTextBox.Text = selectedProduct.Description;
                    PriceTextBox.Text = selectedProduct.Price.ToString();
                    QuantityInStockTextBox.Text = selectedProduct.QuantityInStock.ToString();

                    isEditing = true;
                }
                else // Если уже в режиме редактирования, завершаем редактирование
                {
                    ClearInputFields();
                    isEditing = false;
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is Product selectedProduct)
            {
                // Диалоговое окно подтверждения удаления
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить продукт {selectedProduct.Name}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.Products.Remove(selectedProduct);
                    context.SaveChanges();
                    LoadTable();
                }
            }
        }

        private void ClearInputFields()
        {
            NameTextBox.Text = "";
            DescriptionTextBox.Text = "";
            PriceTextBox.Text = "";
            QuantityInStockTextBox.Text = "";
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
