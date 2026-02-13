using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.Enums;
using BayiSatisYonetim.Models.ViewModels;

namespace BayiSatisYonetim.Controllers.Customer
{
    [Authorize(Roles = "Customer")]
    public class CustomerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CustomerDashboardController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user!.Id);
            if (customer == null) return RedirectToAction("Login", "Account");

            var vm = new CustomerDashboardVM
            {
                FullName = user!.FullName,
                TotalApplications = await _context.Applications.CountAsync(a => a.CustomerId == customer.Id),
                PendingApplications = await _context.Applications.CountAsync(a => a.CustomerId == customer.Id && (a.Status == ApplicationStatus.Pending || a.Status == ApplicationStatus.InReview)),
                ApprovedApplications = await _context.Applications.CountAsync(a => a.CustomerId == customer.Id && a.Status == ApplicationStatus.Approved),
                CompletedApplications = await _context.Applications.CountAsync(a => a.CustomerId == customer.Id && a.Status == ApplicationStatus.Completed),
            };

            vm.RecentApplications = await _context.Applications
                .Where(a => a.CustomerId == customer.Id)
                .Include(a => a.Product).ThenInclude(p => p.Company)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new RecentApplicationData
                {
                    Id = a.Id,
                    ApplicationNumber = a.ApplicationNumber,
                    CustomerName = user.FullName,
                    ProductName = a.Product.Name,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt
                }).ToListAsync();

            vm.Announcements = await _context.Announcements
                .Where(a => a.IsActive && (a.TargetRole == AnnouncementTarget.All || a.TargetRole == AnnouncementTarget.CustomersOnly))
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

            return View("~/Views/Customer/Dashboard/Index.cshtml", vm);
        }
    }
}
