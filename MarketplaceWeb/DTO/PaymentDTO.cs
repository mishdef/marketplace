using MarketplaceData.Model.Cart;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceWeb.DTO
{
    public class PaymentDTO
    {
        public CompanyCart CompanyCart { get; set; } = null!;
        public int ShipmentCompanyID { get; set; }
        public string ShipmetInfo { get; set; } = null!;
    }
}
