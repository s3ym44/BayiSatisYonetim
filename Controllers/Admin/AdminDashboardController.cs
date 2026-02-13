using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Enums;
using BayiSatisYonetim.Models.ViewModels;

namespace BayiSatisYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var sixMonthsAgo = now.AddMonths(-6);

            var allSales = await _context.Sales
                .Include(s => s.Dealer)
                .Include(s => s.Product).ThenInclude(p => p.Category)
                .ToListAsync();

            var vm = new AdminDashboardVM
            {
                TotalDealers = await _context.Dealers.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
                TotalApplications = await _context.Applications.CountAsync(),
                PendingApplications = await _context.Applications.CountAsync(a => a.Status == ApplicationStatus.Pending),
                PendingDealers = await _context.Dealers.CountAsync(d => d.Status == DealerStatus.Pending),
                TotalSales = allSales.Count,
                TotalRevenue = allSales.Sum(s => s.Amount),
                TotalCommissions = allSales.Sum(s => s.CommissionAmount),
            };

            vm.MonthlySales = allSales
                .Where(s => s.SaleDate >= sixMonthsAgo)
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new MonthlySalesData
                {
                    Month = g.Key.Month + "/" + g.Key.Year,
                    Count = g.Count(),
                    Amount = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            vm.CategorySales = allSales
                .GroupBy(s => s.Product.Category.Name)
                .Select(g => new CategorySalesData
                {
                    CategoryName = g.Key,
                    Count = g.Count()
                })
                .ToList();

            vm.RecentApplications = await _context.Applications
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Product)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .Select(a => new RecentApplicationData
                {
                    Id = a.Id,
                    ApplicationNumber = a.ApplicationNumber,
                    CustomerName = a.Customer.User.FullName,
                    ProductName = a.Product.Name,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            vm.TopDealers = allSales
                .GroupBy(s => s.Dealer.CompanyName)
                .Select(g => new TopDealerData
                {
                    CompanyName = g.Key,
                    SalesCount = g.Count(),
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .Take(5)
                .ToList();

            return View("~/Views/Admin/Dashboard/Index.cshtml", vm);
        }
    }
}
