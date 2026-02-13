using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.Enums;
using BayiSatisYonetim.Models.ViewModels;
using BayiSatisYonetim.Services;

namespace BayiSatisYonetim.Controllers.Dealer
{
    [Authorize(Roles = "Dealer")]
    public class DealerApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActivityLogService _activityLog;

        public DealerApplicationController(ApplicationDbContext context, UserManager<AppUser> userManager, IActivityLogService activityLog)
        {
            _context = context;
            _userManager = userManager;
            _activityLog = activityLog;
        }

        private async Task<Models.Entities.Dealer?> GetCurrentDealerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == user!.Id);
        }

        public async Task<IActionResult> Index(ApplicationStatus? status)
        {
            var dealer = await GetCurrentDealerAsync();
            if (dealer == null) return RedirectToAction("Login", "Account");

            var query = _context.Applications
                .Where(a => a.DealerId == dealer.Id)
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Product).ThenInclude(p => p.Company)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            var applications = await query.OrderByDescending(a => a.CreatedAt)
                .Select(a => new ApplicationListViewModel
                {
                    Id = a.Id,
                    ApplicationNumber = a.ApplicationNumber,
                    CustomerName = a.Customer.User.FullName,
                    ProductName = a.Product.Name,
                    CompanyName = a.Product.Company.Name,
                    Status = a.Status,
                    InstallationCity = a.InstallationCity,
                    CreatedAt = a.CreatedAt
                }).ToListAsync();

            ViewBag.CurrentStatus = status;
            return View("~/Views/Dealer/Applications/Index.cshtml", applications);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ApplicationCreateViewModel
            {
                Products = await _context.Products.Where(p => p.IsActive)
                    .Include(p => p.Company).Include(p => p.Category)
                    .OrderBy(p => p.Category.SortOrder).ThenBy(p => p.SortOrder)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = $"{p.Company.Name} - {p.Name} (₺{p.Price}/ay)"
                    }).ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsActive)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync()
            };
            return View("~/Views/Dealer/Applications/Create.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            var dealer = await GetCurrentDealerAsync();
            if (dealer == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.Products = await _context.Products.Where(p => p.IsActive)
                    .Include(p => p.Company).Include(p => p.Category)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Company.Name} - {p.Name} (₺{p.Price}/ay)" }).ToListAsync();
                model.Categories = await _context.Categories.Where(c => c.IsActive)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync();
                return View("~/Views/Dealer/Applications/Create.cshtml", model);
            }

            Models.Entities.Customer customer;
            if (model.CustomerId.HasValue)
            {
                customer = (await _context.Customers.FindAsync(model.CustomerId.Value))!;
            }
            else
            {
                var existingUser = await _userManager.FindByEmailAsync(model.CustomerEmail!);
                AppUser customerUser;
                if (existingUser != null)
                {
                    customerUser = existingUser;
                    customer = await _context.Customers.FirstAsync(c => c.UserId == customerUser.Id);
                }
                else
                {
                    customerUser = new AppUser
                    {
                        UserName = model.CustomerEmail,
                        Email = model.CustomerEmail,
                        FullName = model.CustomerFullName ?? "",
                        PhoneNumber = model.CustomerPhone,
                        Role = UserRole.Customer,
                        IsActive = true,
                        EmailConfirmed = true
                    };
                    await _userManager.CreateAsync(customerUser, "Musteri123!");
                    await _userManager.AddToRoleAsync(customerUser, "Customer");

                    customer = new Models.Entities.Customer
                    {
                        UserId = customerUser.Id,
                        TCKimlik = model.CustomerTCKimlik ?? "",
                        Address = model.InstallationAddress,
                        City = model.InstallationCity
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }
            }

            var lastApp = await _context.Applications.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            var nextNumber = (lastApp?.Id ?? 0) + 1;
            var appNumber = $"BSV-{DateTime.UtcNow.Year}-{nextNumber:D5}";

            var application = new Application
            {
                ApplicationNumber = appNumber,
                CustomerId = customer.Id,
                DealerId = dealer.Id,
                ProductId = model.ProductId,
                Status = ApplicationStatus.Pending,
                InstallationAddress = model.InstallationAddress,
                InstallationCity = model.InstallationCity,
                PreferredDate = model.PreferredDate,
                Notes = model.Notes
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(user!.Id, "Başvuru Oluşturma", $"{appNumber} numaralı başvuru oluşturuldu");

            TempData["Success"] = "Başvuru başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var dealer = await GetCurrentDealerAsync();
            if (dealer == null) return RedirectToAction("Login", "Account");

            var app = await _context.Applications
                .Where(a => a.Id == id && a.DealerId == dealer.Id)
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Product).ThenInclude(p => p.Company)
                .Include(a => a.Product).ThenInclude(p => p.Category)
                .FirstOrDefaultAsync();

            if (app == null) return NotFound();

            var vm = new ApplicationDetailViewModel
            {
                Id = app.Id,
                ApplicationNumber = app.ApplicationNumber,
                CustomerName = app.Customer.User.FullName,
                CustomerEmail = app.Customer.User.Email!,
                CustomerPhone = app.Customer.User.PhoneNumber ?? "",
                CustomerTCKimlik = app.Customer.TCKimlik,
                ProductName = app.Product.Name,
                CompanyName = app.Product.Company.Name,
                CategoryName = app.Product.Category.Name,
                ProductPrice = app.Product.Price,
                Status = app.Status,
                Notes = app.Notes,
                AdminNotes = app.AdminNotes,
                InstallationAddress = app.InstallationAddress,
                InstallationCity = app.InstallationCity,
                PreferredDate = app.PreferredDate,
                CreatedAt = app.CreatedAt,
                UpdatedAt = app.UpdatedAt
            };

            return View("~/Views/Dealer/Applications/Detail.cshtml", vm);
        }
    }
}
