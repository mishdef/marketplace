using MarketplaceData.Model;
using VetClassLibrary.Model;

namespace MarketplaceWeb.DTO
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }

        public List<Category> Categories { get; set; }

        public List<Item> Items { get; set; }
    }
}
