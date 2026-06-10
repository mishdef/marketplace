using MarketplaceData.Model.Cart;
using MarketplaceData.Model.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketplaceData.Model.User
{
    public class ClientInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int CartId { get; set; }
        public MarketplaceData.Model.Cart.Cart? Cart { get; set; } = null;
        public string Address { get; set; } = null!;
        public ICollection<ClientViewedItem> ViewedItems { get; set; } = new List<ClientViewedItem>();
    }
}
