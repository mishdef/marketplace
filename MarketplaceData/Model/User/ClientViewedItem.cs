using System;
using VetClassLibrary.Model;

namespace MarketplaceData.Model.User
{
    public class ClientViewedItem
    {
        public int Id { get; set; }
        public int ClientInfoId { get; set; }
        public ClientInfo ClientInfo { get; set; } = null!;
        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public DateTime ViewedAt { get; set; } = DateTime.Now;
    }
}
