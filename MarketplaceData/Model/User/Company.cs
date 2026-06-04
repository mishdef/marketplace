using VetClassLibrary.Model;

namespace MarketplaceData.Model.User
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; }


        public int OwnerId { get; set; }
        public Seller Owner { get; set; } = null!;


        public string? LogoUrl { get; set; }

        public ICollection<string> ShippingCompanies { get; set; } = new List<string>();

        public ICollection<Seller>? Employees { get; set; } = null!;
        public ICollection<Item>? StoreItems { get; set; } = null!;
    }
}