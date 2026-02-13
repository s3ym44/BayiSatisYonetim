using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;

namespace BayiSatisYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin")) return RedirectToAction("Index", "AdminDashboard");
                if (User.IsInRole("Dealer")) return RedirectToAction("Index", "DealerDashboard");
                if (User.IsInRole("Customer")) return RedirectToAction("Index", "CustomerDashboard");
            }

            ViewBag.ProductCount = await _context.Products.CountAsync(p => p.IsActive);
            ViewBag.CompanyCount = await _context.Companies.CountAsync(c => c.IsActive);
            ViewBag.DealerCount = await _context.Dealers.CountAsync(d => d.Status == Models.Enums.DealerStatus.Approved);
            return View();
        }

        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == 404)
            {
                ViewBag.ErrorMessage = "Aradığınız sayfa bulunamadı.";
                ViewBag.ErrorCode = "404";
            }
            else
            {
                ViewBag.ErrorMessage = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                ViewBag.ErrorCode = statusCode?.ToString() ?? "500";
            }
            return View();
        }
    }
}
