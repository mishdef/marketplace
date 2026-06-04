using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using VetClassLibrary.Model;

namespace MarketplaceData.Model.Cart
{
    public class CartItem
    {
        public int Id { get; set; }

        public Item? Product { get; set; }

        public double Quantity { get; set; }

        public decimal Subtotal
        {
            get
            {
                if (Product == null) return 0;
                return Math.Round(Product.Price * (decimal)Quantity, 2);
            }
        }
    }
}