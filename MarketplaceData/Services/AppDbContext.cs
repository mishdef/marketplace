using MarketplaceData.Model;
using MarketplaceData.Model.Cart;
using MarketplaceData.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using VetClassLibrary.Model;
using VetClassLibrary.Model.Storage;
using VetClassLibrary.Model.User;

namespace VetClassLibrary.Services
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        private readonly string _connectionString;

        public DbSet<ClientInfo> Clients { get; set; }
        public DbSet<SellerInfo> Sellers { get; set; }
        public DbSet<AdminInfo> Admins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CompanyCart> CompanyCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ClientViewedItem> ClientViewedItems { get; set; }
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

            modelBuilder.Entity<ClientInfo>()
                .HasOne(c => c.User)
                .WithOne(u => u.ClientInfo)
                .HasForeignKey<ClientInfo>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SellerInfo>()
                .HasOne(s => s.User)
                .WithOne(u => u.SellerInfo)
                .HasForeignKey<SellerInfo>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AdminInfo>()
                .HasOne(a => a.User)
                .WithOne(u => u.AdminInfo)
                .HasForeignKey<AdminInfo>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();

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

            modelBuilder.Entity<Category>()
                .HasMany(c => c.SubCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CompanyCart>()
                .HasOne(cc => cc.Cart)
                .WithMany(c => c.CompanyCarts)
                .HasForeignKey(cc => cc.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.CompanyCart)
                .WithMany(cc => cc.CartItems)
                .HasForeignKey(ci => ci.CompanyCartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientViewedItem>()
                .HasOne(cvi => cvi.ClientInfo)
                .WithMany(ci => ci.ViewedItems)
                .HasForeignKey(cvi => cvi.ClientInfoId)
                .OnDelete(DeleteBehavior.Cascade);

            //// ==========================================
            //// SEED DATA
            //// ==========================================

            var hasher = new PasswordHasher<User>();
            var defaultPasswordHash = hasher.HashPassword(null!, "password");

            // 0. Roles
            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "Client", NormalizedName = "CLIENT", ConcurrencyStamp = "e9a03975-d142-4911-ad81-bd484e5a953e" },
                new IdentityRole<int> { Id = 2, Name = "Seller", NormalizedName = "SELLER", ConcurrencyStamp = "81ba576a-54e4-4d8b-967a-8b8df2839958" },
                new IdentityRole<int> { Id = 3, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "5c52c6f1-a1e6-42d4-8d4e-1282be389812" }
            );

            // 1. Categories
            modelBuilder.Entity<Category>().HasData(
                new { Id = 1, Name = "Electronics", ImageUrl = "", ParentCategoryId = (int?)null },
                new { Id = 2, Name = "Clothing", ImageUrl = "", ParentCategoryId = (int?)null },
                new { Id = 3, Name = "Home Appliances", ImageUrl = "", ParentCategoryId = (int?)null },
                new { Id = 4, Name = "Books", ImageUrl = "", ParentCategoryId = (int?)null },
                new { Id = 5, Name = "Toys", ImageUrl = "", ParentCategoryId = (int?)null },
                new { Id = 6, Name = "Smartphones", ImageUrl = "", ParentCategoryId = (int?)1 },
                new { Id = 7, Name = "Laptops", ImageUrl = "", ParentCategoryId = (int?)1 },
                new { Id = 8, Name = "Men's Clothing", ImageUrl = "", ParentCategoryId = (int?)2 },
                new { Id = 9, Name = "Women's Clothing", ImageUrl = "", ParentCategoryId = (int?)2 }
            );

            // 2. Carts
            modelBuilder.Entity<Cart>().HasData(
                new { Id = 1 },
                new { Id = 2 }
            );

            // 3. Base Users
            modelBuilder.Entity<User>().HasData(
                new 
                {
                    Id = 1,
                    FullName = "Ivan Petrenko",
                    UserName = "ivan@example.com",
                    NormalizedUserName = "IVAN@EXAMPLE.COM",
                    Password = "password",
                    PasswordHash = defaultPasswordHash,
                    Role = VetClassLibrary.Model.User.UserRoles.Client,
                    Email = "ivan@example.com",
                    NormalizedEmail = "IVAN@EXAMPLE.COM",
                    PhoneNumber = "+380501112233",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = "d6e3557e-77cc-44a5-bdf9-2ba831ec41e1",
                    ConcurrencyStamp = "a7b7a2d1-5db8-4034-8c81-4235e197c36a"
                },
                new 
                {
                    Id = 2,
                    FullName = "Maria Kovalenko",
                    UserName = "maria@example.com",
                    NormalizedUserName = "MARIA@EXAMPLE.COM",
                    Password = "password",
                    PasswordHash = defaultPasswordHash,
                    Role = VetClassLibrary.Model.User.UserRoles.Client,
                    Email = "maria@example.com",
                    NormalizedEmail = "MARIA@EXAMPLE.COM",
                    PhoneNumber = "+380674445566",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = "f6fbb5de-75cc-44e2-a0e1-0bc51a2dcd22",
                    ConcurrencyStamp = "b86300df-b118-4ee0-8ba0-0be157ba8d6c"
                },
                new 
                {
                    Id = 3,
                    FullName = "Tech Seller",
                    UserName = "seller@techstore.com",
                    NormalizedUserName = "SELLER@TECHSTORE.COM",
                    Password = "password",
                    PasswordHash = defaultPasswordHash,
                    Role = VetClassLibrary.Model.User.UserRoles.Seller,
                    Email = "seller@techstore.com",
                    NormalizedEmail = "SELLER@TECHSTORE.COM",
                    PhoneNumber = "+380991122334",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = "96d7cfbb-7ac1-4322-9e90-c11efeb9e2d3",
                    ConcurrencyStamp = "4c3b5d3c-d6a0-4ff6-8c4d-8cfbc1fcf3b2"
                },
                new 
                {
                    Id = 4,
                    FullName = "Fashion Seller",
                    UserName = "seller@fashion.com",
                    NormalizedUserName = "SELLER@FASHION.COM",
                    Password = "password",
                    PasswordHash = defaultPasswordHash,
                    Role = VetClassLibrary.Model.User.UserRoles.Seller,
                    Email = "seller@fashion.com",
                    NormalizedEmail = "SELLER@FASHION.COM",
                    PhoneNumber = "+380995544332",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = "2a8e8419-86ab-4cb7-a70d-c07adcd515df",
                    ConcurrencyStamp = "a11d8c1c-43f1-433b-b72e-d01df4fce3f7"
                },
                new 
                {
                    Id = 5,
                    FullName = "System Admin",
                    UserName = "admin@marketplace.com",
                    NormalizedUserName = "ADMIN@MARKETPLACE.COM",
                    Password = "password",
                    PasswordHash = defaultPasswordHash,
                    Role = VetClassLibrary.Model.User.UserRoles.Admin,
                    Email = "admin@marketplace.com",
                    NormalizedEmail = "ADMIN@MARKETPLACE.COM",
                    PhoneNumber = "+380000000000",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = "f1e2d3c4-b5a6-7890-1234-56789abcdef0",
                    ConcurrencyStamp = "0fedcba9-8765-4321-0987-654321fedcba"
                }
            );

            // 3.1 ClientInfos
            modelBuilder.Entity<ClientInfo>().HasData(
                new { Id = 1, UserId = 1, CartId = 1, Address = "Kyiv, Khreshchatyk, 1" },
                new { Id = 2, UserId = 2, CartId = 2, Address = "Lviv, Franko, 25" }
            );

            // 3.2 SellerInfos
            modelBuilder.Entity<SellerInfo>().HasData(
                new { Id = 1, UserId = 3 },
                new { Id = 2, UserId = 4 }
            );

            // 3.3 AdminInfos
            modelBuilder.Entity<AdminInfo>().HasData(
                new { Id = 1, UserId = 5 }
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
                new { Id = 1, Name = "Smartphone XYZ", Description = "Latest model smartphone", Price = 999.99m, DiscountPrice = 899.99m, CategoryId = 6, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 2, Name = "Laptop Pro", Description = "High performance laptop", Price = 1500.00m, DiscountPrice = 1400.00m, CategoryId = 7, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 3, Name = "Cotton T-Shirt", Description = "Comfortable cotton t-shirt", Price = 20.00m, DiscountPrice = 15.00m, CategoryId = 8, CompanyId = 2, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 4, Name = "iPhone 13", Description = "Apple iPhone 13 128GB", Price = 799.99m, DiscountPrice = 750.00m, CategoryId = 6, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 5, Name = "Samsung Galaxy S22", Description = "Samsung Galaxy S22 256GB", Price = 849.99m, DiscountPrice = 800.00m, CategoryId = 6, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 6, Name = "MacBook Air M2", Description = "Apple MacBook Air M2 8GB 256GB", Price = 1199.99m, DiscountPrice = 1150.00m, CategoryId = 7, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 7, Name = "Dell XPS 15", Description = "Dell XPS 15 Laptop 16GB 512GB", Price = 1699.99m, DiscountPrice = 1600.00m, CategoryId = 7, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 8, Name = "Winter Jacket", Description = "Warm winter jacket for men", Price = 120.00m, DiscountPrice = 100.00m, CategoryId = 8, CompanyId = 2, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 9, Name = "Summer Dress", Description = "Light summer dress for women", Price = 60.00m, DiscountPrice = 50.00m, CategoryId = 9, CompanyId = 2, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 10, Name = "Microwave Oven", Description = "800W Microwave Oven", Price = 150.00m, DiscountPrice = 130.00m, CategoryId = 3, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 11, Name = "Vacuum Cleaner", Description = "Cordless vacuum cleaner", Price = 250.00m, DiscountPrice = 220.00m, CategoryId = 3, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 12, Name = "Coffee Maker", Description = "Espresso coffee maker", Price = 300.00m, DiscountPrice = 280.00m, CategoryId = 3, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 13, Name = "C# in Depth", Description = "Book about C# by Jon Skeet", Price = 45.00m, DiscountPrice = 40.00m, CategoryId = 4, CompanyId = 2, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 14, Name = "Clean Code", Description = "A Handbook of Agile Software Craftsmanship", Price = 50.00m, DiscountPrice = 45.00m, CategoryId = 4, CompanyId = 2, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 15, Name = "The Pragmatic Programmer", Description = "Your journey to mastery", Price = 55.00m, DiscountPrice = 50.00m, CategoryId = 4, CompanyId = 2, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 16, Name = "Lego Star Wars", Description = "Millennium Falcon Lego Set", Price = 160.00m, DiscountPrice = 150.00m, CategoryId = 5, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 17, Name = "Monopoly", Description = "Classic Monopoly Board Game", Price = 25.00m, DiscountPrice = 20.00m, CategoryId = 5, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 18, Name = "Batman Action Figure", Description = "12-inch Batman figure", Price = 15.00m, DiscountPrice = 12.00m, CategoryId = 5, CompanyId = 2, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 19, Name = "Wireless Earbuds", Description = "Noise cancelling wireless earbuds", Price = 199.99m, DiscountPrice = 179.99m, CategoryId = 1, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } },
                new { Id = 20, Name = "Smart Watch", Description = "Fitness tracking smart watch", Price = 249.99m, DiscountPrice = 229.99m, CategoryId = 1, CompanyId = 1, IsDeleted = false, ImageUrls = new List<string>() { "/images/noImage.jpg" } }
            );

            // 7. StorageItems
            modelBuilder.Entity<StorageItem>().HasData(
                new { Id = 1, ItemId = 1, Qty = 50.0 },
                new { Id = 2, ItemId = 2, Qty = 30.0 },
                new { Id = 3, ItemId = 3, Qty = 100.0 },
                new { Id = 4, ItemId = 4, Qty = 20.0 },
                new { Id = 5, ItemId = 5, Qty = 25.0 },
                new { Id = 6, ItemId = 6, Qty = 15.0 },
                new { Id = 7, ItemId = 7, Qty = 10.0 },
                new { Id = 8, ItemId = 8, Qty = 60.0 },
                new { Id = 9, ItemId = 9, Qty = 45.0 },
                new { Id = 10, ItemId = 10, Qty = 30.0 },
                new { Id = 11, ItemId = 11, Qty = 40.0 },
                new { Id = 12, ItemId = 12, Qty = 35.0 },
                new { Id = 13, ItemId = 13, Qty = 100.0 },
                new { Id = 14, ItemId = 14, Qty = 150.0 },
                new { Id = 15, ItemId = 15, Qty = 120.0 },
                new { Id = 16, ItemId = 16, Qty = 25.0 },
                new { Id = 17, ItemId = 17, Qty = 80.0 },
                new { Id = 18, ItemId = 18, Qty = 200.0 },
                new { Id = 19, ItemId = 19, Qty = 90.0 },
                new { Id = 20, ItemId = 20, Qty = 110.0 }
            );




            // 10. User Roles
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int> { RoleId = 1, UserId = 1 },
                new IdentityUserRole<int> { RoleId = 1, UserId = 2 },
                new IdentityUserRole<int> { RoleId = 2, UserId = 3 },
                new IdentityUserRole<int> { RoleId = 2, UserId = 4 },
                new IdentityUserRole<int> { RoleId = 3, UserId = 5 }
            );
        }
    }
}
