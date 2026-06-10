using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Model.User
{
    public class AdminInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
