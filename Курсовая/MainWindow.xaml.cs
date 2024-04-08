using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Курсовая;

namespace Coursework
{
    public partial class MainWindow : Window
    {
        private StoreContext context;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenWindow1_Click(object sender, RoutedEventArgs e)
        {
            var window1 = new Window1(context); // Передача контекста базы данных
            window1.Show(); // Показать Window1
            Hide(); // Скрыть основное окно
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Подключение к базе данных
                context = new StoreContext();
                context.Database.EnsureCreated();
                MessageBox.Show("Подключение к базе данных выполнено успешно.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отключение от базы данных
                context?.Dispose();
                MessageBox.Show("Отключение от базы данных выполнено успешно.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отключения от базы данных: {ex.Message}");
            }
        }
    }
}

public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
}

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
}

public class ProductCategory
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    [Key]
    public int ProductCategoryId { get; set; }
}

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; }
}

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
}

public class Report
{
    public int ReportId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ReportType { get; set; }
}


public class StoreContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=LAPTOP-GB3V8CN4\\SQLYADRO;Database=Курсовая;Trusted_Connection=True;");
    }
}