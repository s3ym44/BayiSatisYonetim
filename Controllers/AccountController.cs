using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.Enums;
using BayiSatisYonetim.Models.ViewModels;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Services;

namespace BayiSatisYonetim.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogService _activityLog;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context,
            IActivityLogService activityLog)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _activityLog = activityLog;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToDashboard();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                await _activityLog.LogAsync(user.Id, "Giriş", "Kullanıcı sisteme giriş yaptı", HttpContext.Connection.RemoteIpAddress?.ToString());

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToDashboard(user.Role);
            }

            ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToDashboard();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }

            var role = model.RegisterType == "Dealer" ? UserRole.Dealer : UserRole.Customer;

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Role = role,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            var roleName = role.ToString();
            await _userManager.AddToRoleAsync(user, roleName);

            if (role == UserRole.Dealer)
            {
                var dealer = new Models.Entities.Dealer
                {
                    UserId = user.Id,
                    CompanyName = model.CompanyName ?? "",
                    TaxNumber = model.TaxNumber ?? "",
                    Address = model.Address ?? "",
                    City = model.City ?? "",
                    CommissionRate = 10,
                    Status = DealerStatus.Pending
                };
                _context.Dealers.Add(dealer);
            }
            else
            {
                var customer = new Models.Entities.Customer
                {
                    UserId = user.Id,
                    TCKimlik = model.TCKimlik ?? "",
                    Address = model.Address ?? "",
                    City = model.City ?? "",
                    BirthDate = model.BirthDate
                };
                _context.Customers.Add(customer);
            }

            await _context.SaveChangesAsync();
            await _activityLog.LogAsync(user.Id, "Kayıt", $"{roleName} olarak kayıt olundu", HttpContext.Connection.RemoteIpAddress?.ToString());

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToDashboard(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
                await _activityLog.LogAsync(user.Id, "Çıkış", "Kullanıcı sistemden çıkış yaptı", HttpContext.Connection.RemoteIpAddress?.ToString());

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToDashboard(UserRole? role = null)
        {
            if (role == null && User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin")) role = UserRole.Admin;
                else if (User.IsInRole("Dealer")) role = UserRole.Dealer;
                else role = UserRole.Customer;
            }

            return role switch
            {
                UserRole.Admin => RedirectToAction("Index", "AdminDashboard"),
                UserRole.Dealer => RedirectToAction("Index", "DealerDashboard"),
                UserRole.Customer => RedirectToAction("Index", "CustomerDashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
