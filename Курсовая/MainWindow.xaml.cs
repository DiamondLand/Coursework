using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
    public ICollection<ProductSupplier> ProductSuppliers { get; set; }
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

public class ShoppingAddress
{
    public int ShoppingAddressId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}


public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; }
}

public class PaymentMethod
{
    public int PaymentMethodId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class OrderPayment
{
    public int OrderPaymentId { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; }
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

public class Supplier
{
    public int SupplierId { get; set; }
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public ICollection<ProductSupplier> ProductSuppliers { get; set; }
}

public class ProductSupplier
{
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
}

public class StoreContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<ShoppingAddress> ShoppingAddresses { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<ProductSupplier> ProductSuppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=LAPTOP-GB3V8CN4\\SQLYADRO;Database=Курсовая;Trusted_Connection=True;");
        optionsBuilder.UseSqlServer("Server=LAPTOP-GB3V8CN4\\SQLYADRO;Database=Курсовая;Trusted_Connection=True;", options =>
        {
            options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<Category>().ToTable("Categories");
        modelBuilder.Entity<ProductCategory>().ToTable("ProductCategories");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
        modelBuilder.Entity<Supplier>().ToTable("Suppliers");
        modelBuilder.Entity<ShoppingAddress>().ToTable("ShoppingAddresses");
        modelBuilder.Entity<PaymentMethod>().ToTable("PaymentMethods");
        modelBuilder.Entity<OrderPayment>().ToTable("OrderPayments");

        modelBuilder.Entity<ProductSupplier>()
            .HasKey(ps => new { ps.ProductId, ps.SupplierId });

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSuppliers)
            .HasForeignKey(ps => ps.ProductId);

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Supplier)
            .WithMany(s => s.ProductSuppliers)
            .HasForeignKey(ps => ps.SupplierId);
    }
}
