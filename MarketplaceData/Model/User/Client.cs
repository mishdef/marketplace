using MarketplaceData.Model.Cart;
using MarketplaceData.Model.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace VetClassLibrary.Model.User
{
    public class Client : UserBase
    {
        public int CartId { get; set; }
        public Cart? Cart { get; set; } = null;
        public string Address { get; set; } = null!;
    }
}
