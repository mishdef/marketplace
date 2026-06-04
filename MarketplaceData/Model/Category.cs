using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    }
}
