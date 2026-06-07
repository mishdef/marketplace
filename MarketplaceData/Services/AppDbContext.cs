using MarketplaceData.Model;
using MarketplaceData.Model.Cart;
using MarketplaceData.Model.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using VetClassLibrary.Model;
using VetClassLibrary.Model.Storage;
using VetClassLibrary.Model.User;
namespace VetClassLibrary.Services
{
    public class AppDbContext :  IdentityDbContext
    {
        private readonly string _connectionString;

        public DbSet<Client> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Item> Products { get; set; }
        public DbSet<StorageItem> StorageItems { get; set; }





        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Client>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Employees)
                .WithOne(s => s.Company)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            //// ==========================================
            //// SEED DATA (Начальные данные)
            //// ==========================================

            // 1. Categories
            modelBuilder.Entity<Category>().HasData(
                new { Id = 1, Name = "Electronics", ImageUrl = "" },
                new { Id = 2, Name = "Clothing", ImageUrl = "" }
            );

            // 2. Carts
            modelBuilder.Entity<Cart>().HasData(
                new { Id = 1 },
                new { Id = 2 }
            );

            // 3. Clients
            modelBuilder.Entity<Client>().HasData(
                new 
                {
                    Id = 1,
                    FullName = "Ivan Petrenko",
                    Username = "ivan_petr",
                    Password = "password",
                    Role = VetClassLibrary.Model.User.UserRoles.Client,
                    Email = "ivan@example.com",
                    PhoneNumber = "+380501112233",
                    CartId = 1,
                    Address = "Kyiv, Khreshchatyk, 1"
                },
                new 
                {
                    Id = 2,
                    FullName = "Maria Kovalenko",
                    Username = "maria_kov",
                    Password = "password",
                    Role = VetClassLibrary.Model.User.UserRoles.Client,
                    Email = "maria@example.com",
                    PhoneNumber = "+380674445566",
                    CartId = 2,
                    Address = "Lviv, Franko, 25"
                }
            );

            // 4. Sellers
            modelBuilder.Entity<Seller>().HasData(
                new 
                {
                    Id = 3,
                    FullName = "Tech Seller",
                    Username = "tech_seller",
                    Password = "password",
                    Role = VetClassLibrary.Model.User.UserRoles.Seller,
                    Email = "seller@techstore.com",
                    PhoneNumber = "+380991122334"
                },
                new 
                {
                    Id = 4,
                    FullName = "Fashion Seller",
                    Username = "fashion_seller",
                    Password = "password",
                    Role = VetClassLibrary.Model.User.UserRoles.Seller,
                    Email = "seller@fashion.com",
                    PhoneNumber = "+380995544332"
                }
            );

            // 5. Companies
            modelBuilder.Entity<Company>().HasData(
                new 
                {
                    Id = 1,
                    Name = "Tech Store",
                    Description = "Best electronics",
                    Address = "Kyiv, Tech St, 1",
                    PhoneNumber = "+380991122334",
                    Email = "contact@techstore.com",
                    OwnerId = 3,
                    LogoUrl = "/images/logo1.jpg",
                    ShippingCompanies = new List<string>()
                },
                new 
                {
                    Id = 2,
                    Name = "Fashion Boutique",
                    Description = "Trendy clothing",
                    Address = "Lviv, Fashion Ave, 2",
                    PhoneNumber = "+380995544332",
                    Email = "contact@fashion.com",
                    OwnerId = 4,
                    LogoUrl = "/images/logo2.jpg",
                    ShippingCompanies = new List<string>()
                }
            );

            // 6. Items (Products)
            modelBuilder.Entity<Item>().HasData(
                new 
                {
                    Id = 1,
                    Name = "Smartphone XYZ",
                    Description = "Latest model smartphone",
                    Price = 999.99m,
                    DiscountPrice = 899.99m,
                    CategoryId = 1,
                    CompanyId = 1,
                    IsDeleted = false,
                    ImageUrls = new List<string>() { "/images/1.png" }
                },
                new 
                {
                    Id = 2,
                    Name = "Laptop Pro",
                    Description = "High performance laptop",
                    Price = 1500.00m,
                    DiscountPrice = 1400.00m,
                    CategoryId = 1,
                    CompanyId = 1,
                    IsDeleted = false,
                    ImageUrls = new List<string>() { "/images/2.png" }
                },
                new 
                {
                    Id = 3,
                    Name = "Cotton T-Shirt",
                    Description = "Comfortable cotton t-shirt",
                    Price = 20.00m,
                    DiscountPrice = 15.00m,
                    CategoryId = 2,
                    CompanyId = 2,
                    IsDeleted = false,
                    ImageUrls = new List<string>() { "/images/3.png" }
                }
            );

            // 7. StorageItems
            modelBuilder.Entity<StorageItem>().HasData(
                new { Id = 1, ItemId = 1, Qty = 50.0 },
                new { Id = 2, ItemId = 2, Qty = 30.0 },
                new { Id = 3, ItemId = 3, Qty = 100.0 }
            );

            // 8. Orders
            modelBuilder.Entity<Order>().HasData(
                new 
                {
                    Id = 1,
                    Date = new DateTime(2026, 6, 1, 10, 0, 0),
                    CompanyId = 1,
                    ClientId = 1,
                    IsPaid = true,
                    TransactionId = 12345,
                    IsPerformed = false,
                    Status = OrderStatus.Shipped
                },
                new 
                {
                    Id = 2,
                    Date = new DateTime(2026, 6, 5, 14, 30, 0),
                    CompanyId = 2,
                    ClientId = 2,
                    IsPaid = false,
                    TransactionId = 0,
                    IsPerformed = false,
                    Status = OrderStatus.Pending
                }
            );

            // 9. CartItems
            modelBuilder.Entity<CartItem>().HasData(
                new { Id = 1, ProductId = 1, Quantity = 1.0, OrderId = 1 },
                new { Id = 2, ProductId = 2, Quantity = 2.0, OrderId = 1 },
                new { Id = 3, ProductId = 3, Quantity = 3.0, OrderId = 2 }
            );
        }
    }
}