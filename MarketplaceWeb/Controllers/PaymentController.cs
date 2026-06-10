using MarketplaceData.Interfaces;
using MarketplaceData.Model.Cart;
using MarketplaceData.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stripe.Checkout;
using VetClassLibrary.Interfaces;

namespace MarketplaceWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public PaymentController(UserManager<User> userManager, ICartService cartService, IOrderService orderService)
        {
            _userManager = userManager;
            _cartService = cartService;
            _orderService = orderService;
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
            options.SuccessUrl = domain + "/Payment/Success?sessionId={CHECKOUT_SESSION_ID}&companyId=" + companyCart.CompanyId;
            options.CancelUrl = domain + "/Cart";

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success(string sessionId, int companyId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("Index", "Cart");
            }

            var service = new SessionService();
            Session session = service.Get(sessionId);

            Order? placedOrder = null;

            if (session.PaymentStatus == "paid")
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();
                var cart = await _cartService.GetUserCartAsync(user.Id);
                var companyCart = cart?.CompanyCarts?.FirstOrDefault(cc => cc.CompanyId == companyId);
                if (companyCart != null && companyCart.CartItems.Any())
                {
                    placedOrder = new Order
                    {
                        CompanyId = companyId,
                        IsPaid = true,
                        IsPerformed = false,
                        Status = OrderStatus.Pending,
                        OrderItems = companyCart.CartItems.Select(ci => new OrderItem
                        {
                            ProductId = ci.ProductId,
                            Quantity = ci.Quantity,
                            Price = ci.Product?.Price ?? 0,
                            Subtotal = ci.Subtotal
                        }).ToList(),
                        Total = companyCart.CartItems.Sum(ci => ci.Subtotal)
                    };

                    await _orderService.ProcessCheckoutAsync(placedOrder, user.Id);

                    foreach (var item in companyCart.CartItems.ToList())
                    {
                        await _cartService.RemoveFromCartAsync(user.Id, item.Id);
                    }
                }
            }
            else
            {
                return Cancel();
            }

            return View(placedOrder);
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}