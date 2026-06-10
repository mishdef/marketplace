using MarketplaceData.Interfaces;
using MarketplaceData.Model.User;
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
        private readonly UserManager<User> _userManager;

        public CheckoutController(ICartService cartService, UserManager<User> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
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

            return View(companyCart);
        }
    }
}
