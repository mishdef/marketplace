using MarketplaceData.Interfaces;
using MarketplaceWeb.Data;
using MarketplaceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VetClassLibrary.Services;

namespace MarketplaceWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IItemService _itemService;

        public HomeController(IItemService itemService)
        {
            _itemService = itemService;
        }

        public IActionResult Index()
        {
            var items = _itemService.GetAll();

            return View(items);
        }

        public IActionResult Details(int id)
        {
            var item = _itemService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Search(string query)
        {
            var items = _itemService.Search(query);
            return View(items);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
