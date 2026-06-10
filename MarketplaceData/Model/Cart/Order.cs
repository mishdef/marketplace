using MarketplaceData.Model.User;
using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Model.User;

namespace MarketplaceData.Model.Cart
{
    public class Order
    {
        public int Id { get; set; }
        public IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();

        public DateTime Date { get; set; }


        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;


        public int ClientId { get; set; }
        public MarketplaceData.Model.User.User Client {  get; set; }


        public bool IsPaid { get; set; }
        public int TransactionId { get; set; }
        public bool IsPerformed { get; set; }


        public OrderStatus Status { get; set; } = OrderStatus.Pending;

    }
}
