using Coursework;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Курсовая
{
    public partial class CategoryWindow : Window
    {
        private StoreContext context;
        private bool isEditing = false;

        public CategoryWindow()
        {
            InitializeComponent();
            context = new StoreContext();
            LoadCategoryTable();
        }

        private void LoadCategoryTable()
        {
            var categories = context.Categories.ToList();
            CategoryDataGrid.ItemsSource = categories;
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите название категории.", "О-о-у", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем новую категорию из введенных данных
            Category newCategory = new Category
            {
                Name = CategoryNameTextBox.Text
            };

            // Если выбрана категория из DataGrid, обновляем ее данные
            if (CategoryDataGrid.SelectedItem != null & isEditing)
            {
                Category selectedCategory = (Category)CategoryDataGrid.SelectedItem;
                selectedCategory.Name = newCategory.Name;
            }
            else // Если создается новая категория
            {
                context.Categories.Add(newCategory);
            }

            context.SaveChanges(); // Сохраняем изменения в базе данных
            LoadCategoryTable();
            ClearCategoryInputFields();
            isEditing = false;
        }

        private void CategoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, что выбранная строка не пуста и является объектом типа Category
            if (CategoryDataGrid.SelectedItem != null && CategoryDataGrid.SelectedItem is Category selectedCategory)
            {
                if (!isEditing) // Если не в режиме редактирования, начинаем редактирование
                {
                    // Заполняем поля ввода данными выбранной категории
                    CategoryNameTextBox.Text = selectedCategory.Name;
                    isEditing = true;
                }
                else // Если уже в режиме редактирования, завершаем редактирование
                {
                    ClearCategoryInputFields();
                    isEditing = false;
                }
            }
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryDataGrid.SelectedItem != null && CategoryDataGrid.SelectedItem is Category selectedCategory)
            {
                // Диалоговое окно подтверждения удаления
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить категорию '{selectedCategory.Name}'?",
                                                          "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.Categories.Remove(selectedCategory);
                    context.SaveChanges();
                    LoadCategoryTable();
                }
            }
        }

        private void ClearCategoryInputFields()
        {
            CategoryNameTextBox.Text = "";
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
