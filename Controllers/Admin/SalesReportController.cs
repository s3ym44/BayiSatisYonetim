using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.ViewModels;

namespace BayiSatisYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class SalesReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? dealerId, int? categoryId)
        {
            var query = _context.Sales
                .Include(s => s.Dealer).ThenInclude(d => d.User)
                .Include(s => s.Product).ThenInclude(p => p.Category)
                .Include(s => s.Application).ThenInclude(a => a.Customer).ThenInclude(c => c.User)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value.AddDays(1));
            if (dealerId.HasValue)
                query = query.Where(s => s.DealerId == dealerId.Value);
            if (categoryId.HasValue)
                query = query.Where(s => s.Product.CategoryId == categoryId.Value);

            var sales = await query.OrderByDescending(s => s.SaleDate).ToListAsync();

            var vm = new SalesReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                DealerId = dealerId,
                CategoryId = categoryId,
                TotalSales = sales.Count,
                TotalRevenue = sales.Sum(s => s.Amount),
                TotalCommissions = sales.Sum(s => s.CommissionAmount),
                Sales = sales.Select(s => new SaleReportItem
                {
                    Id = s.Id,
                    ApplicationNumber = s.Application.ApplicationNumber,
                    DealerName = s.Dealer.CompanyName,
                    CustomerName = s.Application.Customer.User.FullName,
                    ProductName = s.Product.Name,
                    CategoryName = s.Product.Category.Name,
                    SaleDate = s.SaleDate,
                    Amount = s.Amount,
                    CommissionAmount = s.CommissionAmount,
                    CommissionStatus = s.CommissionStatus.ToString()
                }).ToList(),
                MonthlySales = sales.GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                    .Select(g => new MonthlySalesData
                    {
                        Month = $"{g.Key.Month:00}/{g.Key.Year}",
                        Count = g.Count(),
                        Amount = g.Sum(x => x.Amount)
                    }).OrderBy(x => x.Month).ToList(),
                CategorySales = sales.GroupBy(s => s.Product.Category.Name)
                    .Select(g => new CategorySalesData
                    {
                        CategoryName = g.Key,
                        Count = g.Count()
                    }).ToList(),
                TopDealers = sales.GroupBy(s => s.Dealer.CompanyName)
                    .Select(g => new TopDealerData
                    {
                        CompanyName = g.Key,
                        SalesCount = g.Count(),
                        TotalAmount = g.Sum(x => x.Amount)
                    }).OrderByDescending(x => x.TotalAmount).Take(5).ToList()
            };

            ViewBag.Dealers = await _context.Dealers.Select(d => new { d.Id, d.CompanyName }).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View("~/Views/Admin/Reports/Index.cshtml", vm);
        }
    }
}
