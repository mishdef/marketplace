using MarketplaceData.Interfaces;
using MarketplaceWeb.Data;
using MarketplaceWeb.DTO;
using MarketplaceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VetClassLibrary.Services;

namespace MarketplaceWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ICompanyService _companyService;
        private readonly ICategoryService _categoryService;

        public HomeController(IItemService itemService, ICompanyService companyService, ICategoryService categoryService)
        {
            _itemService = itemService;
            _companyService = companyService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var items = _itemService.GetAll();

            return View(
                new IndexViewModel
                {
                    Items = items.ToList(),
                    Categories = _categoryService.GetCategories()
                });
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


        public IActionResult Company(int id)
        {
            var company = _companyService.GetById(id);

            return View(company);
        }

        public IActionResult Category(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(
                new CategoryViewModel 
                { 
                    Category = category, 
                    Items = category.Items, 
                    Categories = category.SubCategories.ToList() 
                });
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
