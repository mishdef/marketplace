using MarketplaceData.Interfaces;
using MarketplaceData.Model.User;
using MarketplaceWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VetClassLibrary.Interfaces;
using VetClassLibrary.Model.User;

namespace MarketplaceWeb.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        private readonly ISellerService _sellerService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<User> _userManager;
        private readonly IPicturesService _picturesService;
        private readonly IStorageService _storageService;

        private readonly IShipmentCompaniesService _shipmentCompaniesService;

        public SellerController(
            ISellerService sellerService, 
            ICompanyService companyService, 
            UserManager<User> userManager,
            IPicturesService picturesService,
            IStorageService storageService,
            IShipmentCompaniesService shipmentCompaniesService)
        {
            _sellerService = sellerService;
            _companyService = companyService;
            _userManager = userManager;
            _picturesService = picturesService;
            _storageService = storageService;
            _shipmentCompaniesService = shipmentCompaniesService;
        }

        [Authorize(Roles = "Seller")]
        public IActionResult Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var seller = _sellerService.GetAll().FirstOrDefault(s => s.UserId == userId);

            if (seller == null)
            {
                return NotFound("Seller profile not found.");
            }

            // A seller is either an owner or an employee
            var company = _companyService.GetAll().FirstOrDefault(c => c.OwnerId == userId) ?? seller.Company;

            return View(company);
        }

        [HttpGet]
        public async Task<IActionResult> EditCompany(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) return Unauthorized();

            var company = _companyService.GetById(id);
            if (company == null) return NotFound();

            if (company.OwnerId != userId) return Forbid();

            var model = new EditCompanyViewModel
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Address = company.Address,
                PhoneNumber = company.PhoneNumber,
                Email = company.Email,
                LogoUrl = company.LogoUrl,
                AvailableShipmentCompanies = (await _shipmentCompaniesService.GetAllAsync()).ToList(),
                SelectedShipmentCompanyIds = company.ShippingCompanies?.Select(sc => sc.Id).ToList() ?? new List<int>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompany(EditCompanyViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) return Unauthorized();

            var company = _companyService.GetById(model.Id);
            if (company == null) return NotFound();

            if (company.OwnerId != userId) return Forbid();

            company.Name = model.Name;
            company.Description = model.Description;
            company.Address = model.Address;
            company.PhoneNumber = model.PhoneNumber;
            company.Email = model.Email;

            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                try
                {
                    var uploadedUrl = await _picturesService.UploadPictureAsync(model.LogoFile);
                    if (!string.IsNullOrEmpty(uploadedUrl))
                    {
                        company.LogoUrl = "/images/" + uploadedUrl;
                    }
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("LogoFile", ex.Message);
                    return View(model);
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.LogoUrl))
            {
                company.LogoUrl = model.LogoUrl;
            }

            _companyService.Update(company);
            await _companyService.UpdateCompanyShipmentsAsync(model.Id, model.SelectedShipmentCompanyIds);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AddEmployee(int companyId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) return Unauthorized();

            var company = _companyService.GetById(companyId);
            if (company == null) return NotFound();
            if (company.OwnerId != userId) return Forbid();

            return View(new AddEmployeeViewModel { CompanyId = companyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(AddEmployeeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) return Unauthorized();

            var company = _companyService.GetById(model.CompanyId);
            if (company == null) return NotFound();
            if (company.OwnerId != userId) return Forbid();

            var newSeller = new User
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = UserRoles.Seller,
                Password = model.Password,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SellerInfo = new SellerInfo { CompanyId = model.CompanyId }
            };

            var result = await _userManager.CreateAsync(newSeller, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newSeller, "Seller");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ManageStorage(int companyId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) return Unauthorized();

            var company = _companyService.GetById(companyId);
            if (company == null) return NotFound();

            var seller = _sellerService.GetAll().FirstOrDefault(s => s.UserId == userId);
            bool isOwner = company.OwnerId == userId;
            bool isEmployee = seller != null && seller.CompanyId == companyId;
            if (!isOwner && !isEmployee) return Forbid();

            var storageItems = _storageService.GetStorageItems(companyId);
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = company.Name;
            return View(storageItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStorageQty(System.Collections.Generic.Dictionary<int, double> quantities, int companyId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) return Unauthorized();

            var company = _companyService.GetById(companyId);
            if (company == null) return NotFound();

            var seller = _sellerService.GetAll().FirstOrDefault(s => s.UserId == userId);
            bool isOwner = company.OwnerId == userId;
            bool isEmployee = seller != null && seller.CompanyId == companyId;
            if (!isOwner && !isEmployee) return Forbid();

            if (quantities != null)
            {
                foreach (var kvp in quantities)
                {
                    try
                    {
                        _storageService.UpdateQty(kvp.Key, kvp.Value);
                    }
                    catch (System.Exception)
                    {
                        // Ignore or handle missing items
                    }
                }
            }

            return RedirectToAction(nameof(ManageStorage), new { companyId = companyId });
        }
    }
}
