using MarketplaceData.Interfaces;
using MarketplaceData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MarketplaceData.Model.User;
using VetClassLibrary.Interfaces;
using VetClassLibrary.Model.User;
using MarketplaceWeb.ViewModels;

namespace MarketplaceWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IShipmentCompaniesService _shipmentCompaniesService;

        public AdminController(ICompanyService companyService, IUserService userService, UserManager<User> userManager, IShipmentCompaniesService shipmentCompaniesService)
        {
            _companyService = companyService;
            _userService = userService;
            _userManager = userManager;
            _shipmentCompaniesService = shipmentCompaniesService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // --- CLIENT CRUD ---
        public async Task<IActionResult> Clients()
        {
            var users = await _userService.GetAllAsync();
            var clients = users.Where(u => u.Role == UserRoles.Client).ToList();
            return View(clients);
        }

        [HttpGet]
        public IActionResult CreateClient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("Password", "Password is required for new clients.");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Role = UserRoles.Client,
                    ClientInfo = new ClientInfo
                    {
                        Address = model.Address
                    }
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Client");
                    return RedirectToAction(nameof(Clients));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditClient(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null || user.Role != UserRoles.Client) return NotFound();

            var model = new ClientViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.ClientInfo?.Address ?? string.Empty
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditClient(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetByIdAsync(model.Id);
                if (user == null || user.Role != UserRoles.Client) return NotFound();

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.Email; // Sync username with email
                user.PhoneNumber = model.PhoneNumber;

                if (user.ClientInfo == null)
                {
                    user.ClientInfo = new ClientInfo { Address = model.Address };
                }
                else
                {
                    user.ClientInfo.Address = model.Address;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.Password);
                    }
                    return RedirectToAction(nameof(Clients));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user != null && user.Role == UserRoles.Client)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Clients));
        }

        // --- COMPANY CRUD ---

        public async Task<IActionResult> Companies()
        {
            var companies = await _companyService.GetAllAsync();
            return View(companies);
        }

        [HttpGet]
        public IActionResult CreateCompany()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(MarketplaceData.Model.User.Company company)
        {
            if (ModelState.IsValid)
            {
                await _companyService.CreateAsync(company);
                return RedirectToAction(nameof(Companies));
            }
            return View(company);
        }

        [HttpGet]
        public async Task<IActionResult> EditCompany(int id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> EditCompany(MarketplaceData.Model.User.Company company)
        {
            if (ModelState.IsValid)
            {
                await _companyService.UpdateAsync(company);
                return RedirectToAction(nameof(Companies));
            }
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            await _companyService.DeleteAsync(id);
            return RedirectToAction(nameof(Companies));
        }

        // --- SHIPMENT COMPANY CRUD ---

        public async Task<IActionResult> ShipmentCompanies()
        {
            var companies = await _shipmentCompaniesService.GetAllAsync();
            return View(companies);
        }

        [HttpGet]
        public IActionResult CreateShipmentCompany()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipmentCompany(MarketplaceData.Model.ShipmentCompany company)
        {
            if (ModelState.IsValid)
            {
                await _shipmentCompaniesService.CreateAsync(company);
                return RedirectToAction(nameof(ShipmentCompanies));
            }
            return View(company);
        }

        [HttpGet]
        public async Task<IActionResult> EditShipmentCompany(int id)
        {
            var company = await _shipmentCompaniesService.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> EditShipmentCompany(MarketplaceData.Model.ShipmentCompany company)
        {
            if (ModelState.IsValid)
            {
                await _shipmentCompaniesService.UpdateAsync(company);
                return RedirectToAction(nameof(ShipmentCompanies));
            }
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShipmentCompany(int id)
        {
            await _shipmentCompaniesService.DeleteAsync(id);
            return RedirectToAction(nameof(ShipmentCompanies));
        }
    }
}
