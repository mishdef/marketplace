using MarketplaceData.Interfaces;
using MarketplaceData.Model.User;
using MarketplaceWeb.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MarketplaceWeb.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IShipmentCompaniesService _shipmentCompaniesService;
        private readonly UserManager<User> _userManager;

        public CheckoutController(ICartService cartService, UserManager<User> userManager, IShipmentCompaniesService shipmentCompaniesService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _shipmentCompaniesService = shipmentCompaniesService;
        }

        public async Task<IActionResult> Index(int companyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cart = await _cartService.GetUserCartAsync(user.Id);
            var companyCart = cart?.CompanyCarts?.FirstOrDefault(cc => cc.CompanyId == companyId);

            if (companyCart == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new CheckoutViewModel
            {
                CompanyCart = companyCart,
                ShipmentCompanies = _shipmentCompaniesService.GetShipmentCompaniesForCompany(companyId),
            };

            return View(viewModel);
        }
    }
}
