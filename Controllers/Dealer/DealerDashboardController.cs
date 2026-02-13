using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.Enums;
using BayiSatisYonetim.Models.ViewModels;

namespace BayiSatisYonetim.Controllers.Dealer
{
    [Authorize(Roles = "Dealer")]
    public class DealerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DealerDashboardController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var dealer = await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == user!.Id);

            if (dealer == null) return RedirectToAction("Login", "Account");

            if (dealer.Status != DealerStatus.Approved)
            {
                ViewBag.DealerStatus = dealer.Status.ToString();
                return View("~/Views/Dealer/Dashboard/Pending.cshtml");
            }

            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

            var dealerSales = await _context.Sales
                .Where(s => s.DealerId == dealer.Id)
                .ToListAsync();

            var vm = new DealerDashboardVM
            {
                CompanyName = dealer.CompanyName,
                DealerStatus = dealer.Status.ToString(),
                TotalApplications = await _context.Applications.CountAsync(a => a.DealerId == dealer.Id),
                PendingApplications = await _context.Applications.CountAsync(a => a.DealerId == dealer.Id && a.Status == ApplicationStatus.Pending),
                CompletedApplications = await _context.Applications.CountAsync(a => a.DealerId == dealer.Id && a.Status == ApplicationStatus.Completed),
                TotalSales = dealerSales.Count,
                TotalRevenue = dealerSales.Sum(s => s.Amount),
                TotalCommission = dealerSales.Sum(s => s.CommissionAmount),
                PendingCommission = dealerSales.Where(s => s.CommissionStatus == CommissionStatus.Pending).Sum(s => s.CommissionAmount),
            };

            vm.MonthlySales = dealerSales
                .Where(s => s.SaleDate >= sixMonthsAgo)
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new MonthlySalesData
                {
                    Month = g.Key.Month + "/" + g.Key.Year,
                    Count = g.Count(),
                    Amount = g.Sum(x => x.Amount)
                }).OrderBy(x => x.Month).ToList();

            vm.RecentApplications = await _context.Applications
                .Where(a => a.DealerId == dealer.Id)
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Product)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new RecentApplicationData
                {
                    Id = a.Id,
                    ApplicationNumber = a.ApplicationNumber,
                    CustomerName = a.Customer.User.FullName,
                    ProductName = a.Product.Name,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt
                }).ToListAsync();

            vm.Announcements = await _context.Announcements
                .Where(a => a.IsActive && (a.TargetRole == AnnouncementTarget.All || a.TargetRole == AnnouncementTarget.DealersOnly))
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new AnnouncementData
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    IsPopup = a.IsPopup,
                    CreatedAt = a.CreatedAt
                }).ToListAsync();

            return View("~/Views/Dealer/Dashboard/Index.cshtml", vm);
        }
    }
}
