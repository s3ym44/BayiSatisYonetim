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
    public class DealerSalesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DealerSalesController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(CommissionStatus? commissionStatus)
        {
            var user = await _userManager.GetUserAsync(User);
            var dealer = await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == user!.Id);
            if (dealer == null) return RedirectToAction("Login", "Account");

            var query = _context.Sales
                .Where(s => s.DealerId == dealer.Id)
                .Include(s => s.Product)
                .Include(s => s.Application).ThenInclude(a => a.Customer).ThenInclude(c => c.User)
                .AsQueryable();

            if (commissionStatus.HasValue)
                query = query.Where(s => s.CommissionStatus == commissionStatus.Value);

            var sales = await query.OrderByDescending(s => s.SaleDate)
                .Select(s => new SaleListItem
                {
                    Id = s.Id,
                    ApplicationNumber = s.Application.ApplicationNumber,
                    ProductName = s.Product.Name,
                    CustomerName = s.Application.Customer.User.FullName,
                    SaleDate = s.SaleDate,
                    Amount = s.Amount,
                    CommissionAmount = s.CommissionAmount,
                    CommissionStatus = s.CommissionStatus
                }).ToListAsync();

            ViewBag.TotalAmount = sales.Sum(s => s.Amount);
            ViewBag.TotalCommission = sales.Sum(s => s.CommissionAmount);
            ViewBag.PendingCommission = sales.Where(s => s.CommissionStatus == CommissionStatus.Pending).Sum(s => s.CommissionAmount);
            ViewBag.CurrentCommissionStatus = commissionStatus;
            return View("~/Views/Dealer/Sales/Index.cshtml", sales);
        }
    }
}
