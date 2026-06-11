using MarketplaceData.Model;
using MarketplaceData.Model.Cart;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceWeb.DTO
{
    public class CheckoutViewModel
    {
        public CompanyCart CompanyCart { get; set; } = null!;
        public List<ShipmentCompany> ShipmentCompanies { get; set; }
    }
}
