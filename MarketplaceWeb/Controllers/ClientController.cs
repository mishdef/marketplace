using MarketplaceData.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VetClassLibrary.Interfaces;

namespace MarketplaceWeb.Controllers
{
    public class ClientController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<User> _userManager;

        public ClientController(IOrderService orderService, UserManager<User> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Orders()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetOrdersByClientIdAsync(user.Id);

            return View(orders);
        }
        public async Task<IActionResult> OrderDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.ClientId != user.Id) return NotFound();

            return View(order);
        }
    }
}
