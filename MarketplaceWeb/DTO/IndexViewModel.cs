using MarketplaceData.Model;
using VetClassLibrary.Model;

namespace MarketplaceWeb.DTO
{
    public class IndexViewModel
    {
        public List<Item> Items { get; set; }
        public List<Category> Categories { get; set; }

        public List<Item> RecentlyViewed { get; set; } = new List<Item>();
        public List<Item> RecommendedItems { get; set; } = new List<Item>();
    }
}
