using VetClassLibrary.Model;
using VetClassLibrary.Model.User;
using Microsoft.EntityFrameworkCore;
using VetClassLibrary.Model.Storage;
using MarketplaceData.Model.Cart;

namespace VetClassLibrary.Services
{
    public class AppDbContext : DbContext
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

            //// ==========================================
            //// SEED DATA (Начальные данные)
            //// ==========================================

            //// 1. Сидирование пользователей
            //modelBuilder.Entity<Client>().HasData(
                //new Client
                //{
                    //Id = 1,
                    //FullName = "Михайло Стадніков",
                    //Username = "admin",
                    //Password = "admin",
                    //Role = UserRoles.Admin
                //},
                //new Client
                //{
                    //Id = 2,
                    //FullName = "Олексій Романенко",
                    //Username = "cashier",
                    //Password = "cashier",
                    //Role = UserRoles.Worker
                //}
            //);

            //// 2. Сидирование владельцев (Owners)
            //modelBuilder.Entity<Owner>().HasData(
                //new Owner
                //{
                    //Id = 1,
                    //FullName = "Іван Петренко",
                    //PhoneNumber = "+380501112233",
                    //Address = "м. Київ, вул. Хрещатик, 1",
                    //Email = "ivan.p@example.com",
                    //Notes = "Постійний клієнт"
                //},
                //new Owner
                //{
                    //Id = 2,
                    //FullName = "Марія Коваленко",
                    //PhoneNumber = "+380674445566",
                    //Address = "м. Львів, вул. Франка, 25",
                    //Notes = "Знижка 5%"
                //}
            //);

            //// 3. Сидирование пациентов (Patients)
            //modelBuilder.Entity<Patient>().HasData(
                //new Patient
                //{
                    //Id = 1,
                    //OwnerId = 1,
                    //Name = "Барсік",
                    //Species = "Кіт",
                    //Breed = "Британська короткошерста",
                    //Sex = "Чоловічий",
                    //IsSterilized = true,
                    //DateOfBirth = new DateTime(2020, 5, 10)
                //},
                //new Patient
                //{
                    //Id = 2,
                    //OwnerId = 1,
                    //Name = "Рекс",
                    //Species = "Собака",
                    //Breed = "Німецька вівчарка",
                    //Sex = "Чоловічий",
                    //IsSterilized = false,
                    //DateOfBirth = new DateTime(2018, 8, 15)
                //},
                //new Patient
                //{
                    //Id = 3,
                    //OwnerId = 2,
                    //Name = "Мурка",
                    //Species = "Кішка",
                    //Breed = "Мейн-кун",
                    //Sex = "Жіночий",
                    //IsSterilized = true,
                    //DateOfBirth = new DateTime(2022, 1, 20)
                //}
            //);

            //// 4. Сидирование визитов (Visits)
            //modelBuilder.Entity<Visit>().HasData(
                //// Завершенный визит (Completed)
                //new Visit
                //{
                    //Id = 1,
                    //PatientId = 1, // Кот Барсик
                    //VisitDate = new DateTime(2023, 10, 15, 10, 0, 0),
                    //Status = VisitStatus.Completed,
                    //ReasonForVisit = "Щорічна вакцинація",
                    //ObjectiveExam = "Температура 38.5, слизові рожеві.",
                    //Diagnosis = "Клінічно здоровий",
                    //Treatment = "Вакцина Nobivac Tricat",
                    //Recommendations = "Спостереження 24 години"
                //},
                //// Запланированный визит в будущем (Planned)
                //new Visit
                //{
                    //Id = 2,
                    //PatientId = 2, // Собака Рекс
                    //VisitDate = new DateTime(2025, 12, 1, 14, 30, 0),
                    //Status = VisitStatus.Planned,
                    //ReasonForVisit = "Профілактичний огляд",
                    //Diagnosis = ""
                //},
                //// Просроченный визит (Planned, но дата в прошлом - попадет в IsOverdue)
                //new Visit
                //{
                    //Id = 3,
                    //PatientId = 3, // Кошка Мурка
                    //VisitDate = new DateTime(2023, 11, 10, 9, 0, 0),
                    //Status = VisitStatus.Planned,
                    //ReasonForVisit = "Стерилізація",
                    //Diagnosis = ""
                //},
                //// Отмененный визит (Canceled)
                //new Visit
                //{
                    //Id = 4,
                    //PatientId = 1, // Кот Барсик
                    //VisitDate = new DateTime(2023, 12, 5, 11, 0, 0),
                    //Status = VisitStatus.Canceled,
                    //ReasonForVisit = "Консультація щодо харчування",
                    //Diagnosis = ""
                //}
            //);

            //// 5. Сидирование товаров (Goods)
            //modelBuilder.Entity<Good>().HasData(
                //new Good
                //{
                    //Id = 1,
                    //Name = "Корм Royal Canin 1кг",
                    //Price = 350.0,
                    //BarCode = "1234567890123",
                    //ImagePath = "ea1e8279-a9f3-4507-8a21-bc305021f04c.jpg" // Инициализируем пустым массивом
                //},
                //new Good
                //{
                    //Id = 2,
                    //Name = "Нашийник від бліх",
                    //Price = 150.0,
                    //BarCode = "9876543210987",
                    //ImagePath = "bfe5860a-aa48-49a5-bbad-2dcb390ab85e.jpg"
                //}
            //);

            //// 6. Сидирование услуг (Services)
            //// Id продолжают нумерацию, так как Service и Good делят одну таблицу Items
            //modelBuilder.Entity<Service>().HasData(
                //new Service
                //{
                    //Id = 3,
                    //Name = "Первинний огляд",
                    //Price = 400.0,
                    //Description = "Базовий огляд тварини лікарем-терапевтом"
                //},
                //new Service
                //{
                    //Id = 4,
                    //Name = "Вакцинація комплексна",
                    //Price = 250.0,
                    //Description = "Щеплення полівалентною вакциною"
                //}
            //);

            // 7. Сидирование склада (StorageItems)
            // Используем анонимный тип для заполнения теневого внешнего ключа ItemId
            modelBuilder.Entity<StorageItem>().HasData(
                new { Id = 1, ItemId = 1, Qty = 50.0 }, // Ссылается на Корм Royal Canin (Good Id = 1)
                new { Id = 2, ItemId = 2, Qty = 20.0 }  // Ссылается на Нашийник (Good Id = 2)
            );
        }
    }
}