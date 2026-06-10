using MarketplaceData.Interfaces;
using MarketplaceWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VetClassLibrary.Interfaces;
using VetClassLibrary.Model;

namespace MarketplaceWeb.Controllers
{
    [Authorize(Roles = "Seller")]
    public class ShopItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private readonly ICompanyService _companyService;
        private readonly ISellerService _sellerService;
        private readonly IPicturesService _picturesService;

        public ShopItemsController(
            IItemService itemService, 
            ICategoryService categoryService, 
            ICompanyService companyService, 
            ISellerService sellerService,
            IPicturesService picturesService)
        {
            _itemService = itemService;
            _categoryService = categoryService;
            _companyService = companyService;
            _sellerService = sellerService;
            _picturesService = picturesService;
        }

        private int? GetCurrentCompanyId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) return null;

            var company = _companyService.GetAll().FirstOrDefault(c => c.OwnerId == userId);
            if (company != null) return company.Id;

            var seller = _sellerService.GetById(userId);
            return seller?.CompanyId;
        }

        public IActionResult Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Unauthorized("No company associated with your account.");

            var items = _itemService.GetAll().Where(i => i.CompanyId == companyId && !i.IsDeleted).ToList();
            
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Unauthorized();

            var categories = _categoryService.GetAll()
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            var model = new ShopItemViewModel { Categories = categories };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShopItemViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                model.Categories = _categoryService.GetAll()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
                return View(model);
            }

            var newItem = new Item
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DiscountPrice = model.DiscountPrice,
                CategoryId = model.CategoryId,
                CompanyId = companyId.Value,
                IsDeleted = false,
                ImageUrls = new List<string>()
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                try
                {
                    var uploadedUrl = await _picturesService.UploadPictureAsync(model.ImageFile);
                    if (!string.IsNullOrEmpty(uploadedUrl))
                    {
                        newItem.ImageUrls.Add("/images/" + uploadedUrl);
                    }
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("ImageFile", ex.Message);
                    model.Categories = _categoryService.GetAll()
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToList();
                    return View(model);
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                newItem.ImageUrls.Add(model.ImageUrl);
            }
            else
            {
                newItem.ImageUrls.Add("/images/noImage.jpg");
            }

            _itemService.Create(newItem);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Unauthorized();

            var item = _itemService.GetById(id);
            if (item == null || item.IsDeleted || item.CompanyId != companyId.Value) return NotFound();

            var categories = _categoryService.GetAll()
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            var model = new ShopItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                DiscountPrice = item.DiscountPrice,
                CategoryId = item.CategoryId,
                ImageUrl = item.ImageUrls != null && item.ImageUrls.Any() && item.ImageUrls.First() != "/images/noImage.jpg" ? item.ImageUrls.First() : string.Empty,
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShopItemViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Unauthorized();

            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Categories = _categoryService.GetAll()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
                return View(model);
            }

            var item = _itemService.GetById(id);
            if (item == null || item.IsDeleted || item.CompanyId != companyId.Value) return NotFound();

            item.Name = model.Name;
            item.Description = model.Description;
            item.Price = model.Price;
            item.DiscountPrice = model.DiscountPrice;
            item.CategoryId = model.CategoryId;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                try
                {
                    var uploadedUrl = await _picturesService.UploadPictureAsync(model.ImageFile);
                    if (!string.IsNullOrEmpty(uploadedUrl))
                    {
                        item.ImageUrls = new List<string> { "/images/" + uploadedUrl };
                    }
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("ImageFile", ex.Message);
                    model.Categories = _categoryService.GetAll()
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToList();
                    return View(model);
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                item.ImageUrls = new List<string> { model.ImageUrl };
            }
            else if (item.ImageUrls == null || !item.ImageUrls.Any())
            {
                item.ImageUrls = new List<string> { "/images/noImage.jpg" };
            }

            _itemService.Update(item);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Unauthorized();

            var item = _itemService.GetById(id);
            if (item == null || item.CompanyId != companyId.Value) return NotFound();

            item.IsDeleted = true;
            _itemService.Update(item);

            return RedirectToAction(nameof(Index));
        }
    }
}
