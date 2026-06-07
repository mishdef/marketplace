using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Model;

namespace MarketplaceData.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;

        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public List<Item> Items { get; set; } = new List<Item>();
    }
}
