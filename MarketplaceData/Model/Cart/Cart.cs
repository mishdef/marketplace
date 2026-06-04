using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Model.Cart
{
    public class Cart
    {
        public int Id { get; set; }
        ICollection<CompanyCart> CompanyCarts { get; set; } = new List<CompanyCart>();
    }
}
