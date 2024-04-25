using Coursework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Курсовая
{
    public partial class ProductCategoryWindow : Window
    {
        private StoreContext context;

        public ProductCategoryWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadComboBoxes();
            LoadProductCategories();
        }

        private void LoadComboBoxes()
        {
            // Загружаем товары в ComboBox
            var products = context.Products.ToList();
            ProductComboBox.ItemsSource = products;
            ProductComboBox.DisplayMemberPath = "Name";
            ProductComboBox.SelectedValuePath = "ProductId";

            // Загружаем категории в ComboBox
            var categories = context.Categories.ToList();
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "CategoryId";
        }

        private void LoadProductCategories()
        {
            // Загружаем данные для DataGrid
            var productCategories = context.ProductCategories
                .Include("Product")
                .Include("Category")
                .ToList();
            ProductCategoryDataGrid.ItemsSource = productCategories;
        }

        private void AddProductCategory_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem == null || CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите товар и категорию.", "О-о-у", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int productId = (int)ProductComboBox.SelectedValue;
            int categoryId = (int)CategoryComboBox.SelectedValue;

            // Проверяем, не существует ли уже такой связи
            if (context.ProductCategories.Any(pc => pc.ProductId == productId && pc.CategoryId == categoryId))
            {
                MessageBox.Show("Такая связь уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем новую связь между товаром и категорией
            ProductCategory newProductCategory = new ProductCategory
            {
                ProductId = productId,
                CategoryId = categoryId
            };

            context.ProductCategories.Add(newProductCategory);
            context.SaveChanges(); // Сохраняем изменения в базе данных
            LoadProductCategories(); // Загружаем обновленные данные
        }


        private void ProductCategoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, что выбранная строка не пуста и является объектом типа ProductCategory
            if (ProductCategoryDataGrid.SelectedItem != null && ProductCategoryDataGrid.SelectedItem is ProductCategory selectedProductCategory)
            {
                // Пока не будем реализовывать редактирование связей, так как обычно их изменение не требуется в приложениях
                MessageBox.Show("Редактирование связей не реализовано в данной версии приложения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteProductCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductCategoryDataGrid.SelectedItem != null && ProductCategoryDataGrid.SelectedItem is ProductCategory selectedProductCategory)
            {
                context.ProductCategories.Remove(selectedProductCategory);
                context.SaveChanges();
                LoadProductCategories();
            }
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
