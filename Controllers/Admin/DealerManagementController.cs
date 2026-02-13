using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.Enums;
using BayiSatisYonetim.Models.ViewModels;
using BayiSatisYonetim.Services;
using Microsoft.AspNetCore.Identity;

namespace BayiSatisYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class DealerManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActivityLogService _activityLog;

        public DealerManagementController(ApplicationDbContext context, UserManager<AppUser> userManager, IActivityLogService activityLog)
        {
            _context = context;
            _userManager = userManager;
            _activityLog = activityLog;
        }

        public async Task<IActionResult> Index(DealerStatus? status, string? search)
        {
            var query = _context.Dealers.Include(d => d.User).Include(d => d.Sales).AsQueryable();

            if (status.HasValue)
                query = query.Where(d => d.Status == status.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(d => d.CompanyName.Contains(search) || d.User.FullName.Contains(search) || d.User.Email!.Contains(search));

            var dealers = await query.OrderByDescending(d => d.CreatedAt)
                .Select(d => new DealerListViewModel
                {
                    Id = d.Id,
                    CompanyName = d.CompanyName,
                    FullName = d.User.FullName,
                    Email = d.User.Email!,
                    Phone = d.User.PhoneNumber ?? "",
                    City = d.City,
                    TaxNumber = d.TaxNumber,
                    CommissionRate = d.CommissionRate,
                    Status = d.Status,
                    SalesCount = d.Sales.Count,
                    CreatedAt = d.CreatedAt
                }).ToListAsync();

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;
            return View("~/Views/Admin/Dealers/Index.cshtml", dealers);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var dealer = await _context.Dealers
                .Include(d => d.User)
                .Include(d => d.Sales).ThenInclude(s => s.Product)
                .Include(d => d.Sales).ThenInclude(s => s.Application).ThenInclude(a => a.Customer).ThenInclude(c => c.User)
                .Include(d => d.Applications).ThenInclude(a => a.Product)
                .Include(d => d.Applications).ThenInclude(a => a.Customer).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dealer == null) return NotFound();

            var vm = new DealerDetailViewModel
            {
                Id = dealer.Id,
                CompanyName = dealer.CompanyName,
                FullName = dealer.User.FullName,
                Email = dealer.User.Email!,
                Phone = dealer.User.PhoneNumber ?? "",
                TaxNumber = dealer.TaxNumber,
                Address = dealer.Address,
                City = dealer.City,
                BankName = dealer.BankName,
                IBAN = dealer.IBAN,
                CommissionRate = dealer.CommissionRate,
                Status = dealer.Status,
                ApprovalDate = dealer.ApprovalDate,
                Notes = dealer.Notes,
                CreatedAt = dealer.CreatedAt,
                TotalSales = dealer.Sales.Count,
                TotalRevenue = dealer.Sales.Sum(s => s.Amount),
                TotalCommission = dealer.Sales.Sum(s => s.CommissionAmount),
                RecentSales = dealer.Sales.OrderByDescending(s => s.SaleDate).Take(10).Select(s => new SaleListItem
                {
                    Id = s.Id,
                    ApplicationNumber = s.Application.ApplicationNumber,
                    ProductName = s.Product.Name,
                    CustomerName = s.Application.Customer.User.FullName,
                    SaleDate = s.SaleDate,
                    Amount = s.Amount,
                    CommissionAmount = s.CommissionAmount,
                    CommissionStatus = s.CommissionStatus
                }).ToList(),
                RecentApplications = dealer.Applications.OrderByDescending(a => a.CreatedAt).Take(10).Select(a => new RecentApplicationData
                {
                    Id = a.Id,
                    ApplicationNumber = a.ApplicationNumber,
                    CustomerName = a.Customer.User.FullName,
                    ProductName = a.Product.Name,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt
                }).ToList()
            };

            return View("~/Views/Admin/Dealers/Detail.cshtml", vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Dealers/Create.cshtml", new DealerCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DealerCreateViewModel model)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/Dealers/Create.cshtml", model);

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Role = UserRole.Dealer,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View("~/Views/Admin/Dealers/Create.cshtml", model);
            }

            await _userManager.AddToRoleAsync(user, "Dealer");

            var dealer = new Models.Entities.Dealer
            {
                UserId = user.Id,
                CompanyName = model.CompanyName,
                TaxNumber = model.TaxNumber,
                Address = model.Address,
                City = model.City,
                CommissionRate = model.CommissionRate,
                Status = DealerStatus.Approved,
                ApprovalDate = DateTime.UtcNow
            };

            _context.Dealers.Add(dealer);
            await _context.SaveChangesAsync();

            var adminUser = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(adminUser!.Id, "Bayi Oluşturma", $"{model.CompanyName} bayisi oluşturuldu");

            TempData["Success"] = "Bayi başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null) return NotFound();

            var vm = new DealerEditViewModel
            {
                Id = dealer.Id,
                CompanyName = dealer.CompanyName,
                TaxNumber = dealer.TaxNumber,
                Address = dealer.Address,
                City = dealer.City,
                BankName = dealer.BankName,
                IBAN = dealer.IBAN,
                CommissionRate = dealer.CommissionRate,
                Status = dealer.Status,
                Notes = dealer.Notes
            };

            return View("~/Views/Admin/Dealers/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DealerEditViewModel model)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/Dealers/Edit.cshtml", model);

            var dealer = await _context.Dealers.FindAsync(model.Id);
            if (dealer == null) return NotFound();

            var oldStatus = dealer.Status;

            dealer.CompanyName = model.CompanyName;
            dealer.TaxNumber = model.TaxNumber;
            dealer.Address = model.Address;
            dealer.City = model.City;
            dealer.BankName = model.BankName;
            dealer.IBAN = model.IBAN;
            dealer.CommissionRate = model.CommissionRate;
            dealer.Status = model.Status;
            dealer.Notes = model.Notes;
            dealer.UpdatedAt = DateTime.UtcNow;

            if (oldStatus != DealerStatus.Approved && model.Status == DealerStatus.Approved)
                dealer.ApprovalDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var adminUser = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(adminUser!.Id, "Bayi Güncelleme", $"{model.CompanyName} bayisi güncellendi");

            TempData["Success"] = "Bayi bilgileri güncellendi.";
            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null) return NotFound();

            dealer.Status = DealerStatus.Approved;
            dealer.ApprovalDate = DateTime.UtcNow;
            dealer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var adminUser = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(adminUser!.Id, "Bayi Onay", $"Bayi #{id} onaylandı");

            TempData["Success"] = "Bayi onaylandı.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null) return NotFound();

            dealer.Status = DealerStatus.Rejected;
            dealer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var adminUser = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(adminUser!.Id, "Bayi Red", $"Bayi #{id} reddedildi");

            TempData["Success"] = "Bayi reddedildi.";
            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}
