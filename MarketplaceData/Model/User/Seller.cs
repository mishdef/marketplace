using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Model.User
{
    public class Seller : UserBase
    {
        public int? CompanyId { get; set; }
        public Company? Company { get; set; } = null;
    }
}
