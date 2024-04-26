using Coursework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Курсовая
{
    public partial class ProductSupplierWindow : Window
    {
        private StoreContext context;
        private bool isEditing = false;

        public ProductSupplierWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadTable();
            LoadSuppliers();
            LoadProducts();
        }

        private void LoadTable()
        {
            var productSuppliers = context.ProductSuppliers
                                    .Include("Product")
                                    .Include("Supplier")
                                    .ToList();
            DataGrid.ItemsSource = productSuppliers;
        }

        private void LoadProducts()
        {
            var products = context.Products.ToList();
            ProductComboBox.ItemsSource = products;
            ProductComboBox.DisplayMemberPath = "Name";
            ProductComboBox.SelectedValuePath = "ProductId";
        }

        private void LoadSuppliers()
        {
            var suppliers = context.Suppliers.ToList();
            SupplierComboBox.ItemsSource = suppliers;
            SupplierComboBox.DisplayMemberPath = "Name";
            SupplierComboBox.SelectedValuePath = "SupplierId";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem == null || SupplierComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите поставщика и продукт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int productId = (int)ProductComboBox.SelectedValue;
            int supplierId = (int)SupplierComboBox.SelectedValue;

            // Проверяем, не существует ли уже такой связи
            if (context.ProductSuppliers.Any(pc => pc.ProductId == productId && pc.SupplierId == supplierId))
            {
                MessageBox.Show("Такая связь уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Обновляем существующую связь продукта и поставщика
            if (DataGrid.SelectedItem is ProductSupplier selectedProductSupplier)
            {
                // Удаляем существующую связь продукта и поставщика
                context.ProductSuppliers.Remove(selectedProductSupplier);
                context.SaveChanges(); // Удаляем существующую связь из базы данных

                // Создаем новую связь продукта и поставщика с новыми значениями
                ProductSupplier newProductSupplier = new ProductSupplier
                {
                    Product = (Product)ProductComboBox.SelectedItem,
                    Supplier = (Supplier)SupplierComboBox.SelectedItem
                };

                // Добавляем новую связь в базу данных
                context.ProductSuppliers.Add(newProductSupplier);
                context.SaveChanges(); // Сохраняем изменения в базе данных
                LoadTable();
                ClearInputFields();
                isEditing = false;
            }
            else
            {
                // Добавляем новую связь продукта и поставщика
                ProductSupplier newProductSupplier = new ProductSupplier
                {
                    Product = (Product)ProductComboBox.SelectedItem,
                    Supplier = (Supplier)SupplierComboBox.SelectedItem
                };

                context.ProductSuppliers.Add(newProductSupplier);
                context.SaveChanges(); // Сохраняем изменения в базе данных
                LoadTable();
                ClearInputFields();
                isEditing = false;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is ProductSupplier selectedProductSupplier)
            {
                if (!isEditing)
                {
                    ProductComboBox.SelectedItem = selectedProductSupplier.Product;
                    SupplierComboBox.SelectedItem = selectedProductSupplier.Supplier;

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
            if (DataGrid.SelectedItem != null && DataGrid.SelectedItem is ProductSupplier selectedProductSupplier)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить связь продукта {selectedProductSupplier.Product.Name} и поставщика {selectedProductSupplier.Supplier.Name}?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.ProductSuppliers.Remove(selectedProductSupplier);
                    context.SaveChanges();
                    LoadTable();
                }
            }
        }

        private void ClearInputFields()
        {
            ProductComboBox.SelectedItem = null;
            SupplierComboBox.SelectedItem = null;
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
