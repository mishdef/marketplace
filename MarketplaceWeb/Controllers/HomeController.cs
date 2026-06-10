using MarketplaceData.Interfaces;
using MarketplaceWeb.Data;
using MarketplaceWeb.DTO;
using MarketplaceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VetClassLibrary.Services;
using VetClassLibrary.Interfaces;
using Microsoft.AspNetCore.Identity;
using MarketplaceData.Model.User;
using System.Threading.Tasks;

namespace MarketplaceWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ICompanyService _companyService;
        private readonly ICategoryService _categoryService;
        private readonly IStorageService _storageService;
        private readonly IRecommendationService _recommendationService;
        private readonly UserManager<User> _userManager;

        public HomeController(IItemService itemService, ICompanyService companyService, ICategoryService categoryService, IStorageService storageService, IRecommendationService recommendationService, UserManager<User> userManager)
        {
            _itemService = itemService;
            _companyService = companyService;
            _categoryService = categoryService;
            _storageService = storageService;
            _recommendationService = recommendationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var items = _itemService.GetAll();
            var model = new IndexViewModel
            {
                Items = items.ToList(),
                Categories = _categoryService.GetCategories()
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    model.RecentlyViewed = await _recommendationService.GetRecentlyViewedItemsAsync(user.Id);
                    model.RecommendedItems = await _recommendationService.GetPersonalizedRecommendationsAsync(user.Id);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = _itemService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    await _recommendationService.RecordItemViewAsync(user.Id, id);
                }
            }
            
            var storageItems = _storageService.GetStorageItems();
            var storageItem = storageItems.FirstOrDefault(s => s.ItemId == id);
            ViewBag.AvailableQuantity = storageItem?.Qty ?? 0;

            ViewBag.CompanyRecommendations = await _recommendationService.GetCompanyRecommendationsAsync(item.CompanyId, item.Id);
            ViewBag.SimilarItems = await _recommendationService.GetSimilarItemsAsync(item.CategoryId, item.CompanyId);

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
