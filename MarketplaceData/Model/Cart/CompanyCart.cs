using MarketplaceData.Model.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Model.Cart
{
    public class CompanyCart
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
