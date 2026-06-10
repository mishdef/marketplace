using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Model.User
{
    public class SellerInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int? CompanyId { get; set; }
        public Company? Company { get; set; } = null;
    }
}
