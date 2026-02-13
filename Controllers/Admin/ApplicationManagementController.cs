using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.Enums;
using BayiSatisYonetim.Models.ViewModels;
using BayiSatisYonetim.Services;

namespace BayiSatisYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ApplicationManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActivityLogService _activityLog;
        private readonly ICommissionService _commissionService;

        public ApplicationManagementController(ApplicationDbContext context, UserManager<AppUser> userManager, IActivityLogService activityLog, ICommissionService commissionService)
        {
            _context = context;
            _userManager = userManager;
            _activityLog = activityLog;
            _commissionService = commissionService;
        }

        public async Task<IActionResult> Index(ApplicationStatus? status, string? search)
        {
            var query = _context.Applications
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Dealer).ThenInclude(d => d!.User)
                .Include(a => a.Product).ThenInclude(p => p.Company)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(a => a.ApplicationNumber.Contains(search) || a.Customer.User.FullName.Contains(search));

            var applications = await query.OrderByDescending(a => a.CreatedAt)
                .Select(a => new ApplicationListViewModel
                {
                    Id = a.Id,
                    ApplicationNumber = a.ApplicationNumber,
                    CustomerName = a.Customer.User.FullName,
                    DealerName = a.Dealer != null ? a.Dealer.CompanyName : null,
                    ProductName = a.Product.Name,
                    CompanyName = a.Product.Company.Name,
                    Status = a.Status,
                    InstallationCity = a.InstallationCity,
                    CreatedAt = a.CreatedAt
                }).ToListAsync();

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;
            return View("~/Views/Admin/Applications/Index.cshtml", applications);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var app = await _context.Applications
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Dealer).ThenInclude(d => d!.User)
                .Include(a => a.Product).ThenInclude(p => p.Company)
                .Include(a => a.Product).ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null) return NotFound();

            var vm = new ApplicationDetailViewModel
            {
                Id = app.Id,
                ApplicationNumber = app.ApplicationNumber,
                CustomerName = app.Customer.User.FullName,
                CustomerEmail = app.Customer.User.Email!,
                CustomerPhone = app.Customer.User.PhoneNumber ?? "",
                CustomerTCKimlik = app.Customer.TCKimlik,
                DealerName = app.Dealer?.User.FullName,
                DealerCompany = app.Dealer?.CompanyName,
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
                AttachmentUrl = app.AttachmentUrl,
                CreatedAt = app.CreatedAt,
                UpdatedAt = app.UpdatedAt
            };

            return View("~/Views/Admin/Applications/Detail.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(ApplicationStatusUpdateViewModel model)
        {
            var app = await _context.Applications
                .Include(a => a.Dealer)
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.Id == model.Id);

            if (app == null) return NotFound();

            var oldStatus = app.Status;
            app.Status = model.Status;
            app.AdminNotes = model.AdminNotes;
            app.UpdatedAt = DateTime.UtcNow;

            if (model.Status == ApplicationStatus.Completed && app.DealerId.HasValue)
            {
                var existingSale = await _context.Sales.AnyAsync(s => s.ApplicationId == app.Id);
                if (!existingSale)
                {
                    var dealer = app.Dealer!;
                    var commission = _commissionService.CalculateCommission(app.Product.Price, dealer.CommissionRate);

                    var sale = new Sale
                    {
                        ApplicationId = app.Id,
                        DealerId = dealer.Id,
                        ProductId = app.ProductId,
                        SaleDate = DateTime.UtcNow,
                        Amount = app.Product.Price,
                        CommissionAmount = commission,
                        CommissionStatus = CommissionStatus.Pending
                    };
                    _context.Sales.Add(sale);
                }
            }

            await _context.SaveChangesAsync();

            var adminUser = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(adminUser!.Id, "Başvuru Durum Güncelleme",
                $"{app.ApplicationNumber} durumu {oldStatus} → {model.Status} olarak güncellendi");

            TempData["Success"] = "Başvuru durumu güncellendi.";
            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }
    }
}
