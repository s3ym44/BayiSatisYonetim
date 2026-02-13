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

namespace BayiSatisYonetim.Controllers.Customer
{
    [Authorize(Roles = "Customer")]
    public class CustomerApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActivityLogService _activityLog;

        public CustomerApplicationController(ApplicationDbContext context, UserManager<AppUser> userManager, IActivityLogService activityLog)
        {
            _context = context;
            _userManager = userManager;
            _activityLog = activityLog;
        }

        private async Task<Models.Entities.Customer?> GetCurrentCustomerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user!.Id);
        }

        public async Task<IActionResult> Index(ApplicationStatus? status)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var query = _context.Applications
                .Where(a => a.CustomerId == customer.Id)
                .Include(a => a.Product).ThenInclude(p => p.Company)
                .Include(a => a.Dealer)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            var applications = await query.OrderByDescending(a => a.CreatedAt)
                .Select(a => new ApplicationListViewModel
                {
                    Id = a.Id,
                    ApplicationNumber = a.ApplicationNumber,
                    CustomerName = "",
                    DealerName = a.Dealer != null ? a.Dealer.CompanyName : "Bireysel",
                    ProductName = a.Product.Name,
                    CompanyName = a.Product.Company.Name,
                    Status = a.Status,
                    InstallationCity = a.InstallationCity,
                    CreatedAt = a.CreatedAt
                }).ToListAsync();

            ViewBag.CurrentStatus = status;
            return View("~/Views/Customer/Applications/Index.cshtml", applications);
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
            return View("~/Views/Customer/Applications/Create.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.Products = await _context.Products.Where(p => p.IsActive)
                    .Include(p => p.Company)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Company.Name} - {p.Name} (₺{p.Price}/ay)" }).ToListAsync();
                model.Categories = await _context.Categories.Where(c => c.IsActive)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync();
                return View("~/Views/Customer/Applications/Create.cshtml", model);
            }

            var lastApp = await _context.Applications.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            var nextNumber = (lastApp?.Id ?? 0) + 1;
            var appNumber = $"BSV-{DateTime.UtcNow.Year}-{nextNumber:D5}";

            var application = new Application
            {
                ApplicationNumber = appNumber,
                CustomerId = customer.Id,
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
            await _activityLog.LogAsync(user!.Id, "Başvuru Oluşturma", $"{appNumber} numaralı bireysel başvuru oluşturuldu");

            TempData["Success"] = "Başvurunuz başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var app = await _context.Applications
                .Where(a => a.Id == id && a.CustomerId == customer.Id)
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Product).ThenInclude(p => p.Company)
                .Include(a => a.Product).ThenInclude(p => p.Category)
                .Include(a => a.Dealer)
                .FirstOrDefaultAsync();

            if (app == null) return NotFound();

            var vm = new ApplicationDetailViewModel
            {
                Id = app.Id,
                ApplicationNumber = app.ApplicationNumber,
                CustomerName = app.Customer.User.FullName,
                CustomerEmail = app.Customer.User.Email!,
                CustomerPhone = app.Customer.User.PhoneNumber ?? "",
                DealerName = app.Dealer?.CompanyName,
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

            return View("~/Views/Customer/Applications/Detail.cshtml", vm);
        }
    }
}
