using MarketplaceData.Interfaces;
using MarketplaceData.Model.Cart;
using MarketplaceData.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout; 

namespace MarketplaceWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICartService _cartService;

        public PaymentController(UserManager<User> userManager, ICartService cartService)
        {
            _userManager = userManager;
            _cartService = cartService;
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

            return CreateCheckoutSession(companyCart);
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession(CompanyCart companyCart)
        {
            var domain = "http://localhost:5209";

            var options = new SessionCreateOptions();

            options.LineItems = new List<SessionLineItemOptions>();

            foreach (var item in companyCart.CartItems)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description
                        },
                    },
                    Quantity = (long)item.Quantity,
                }
                );
            }

            options.Mode = "payment";
            options.SuccessUrl = domain + "/Payment/Success";
            options.CancelUrl = domain + "/Cart";

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}