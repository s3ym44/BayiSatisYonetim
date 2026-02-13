using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.ViewModels;

namespace BayiSatisYonetim.Controllers.Dealer
{
    [Authorize(Roles = "Dealer")]
    public class DealerProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DealerProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, int? companyId, string? search)
        {
            var query = _context.Products.Where(p => p.IsActive)
                .Include(p => p.Category).Include(p => p.Company).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);
            if (companyId.HasValue)
                query = query.Where(p => p.CompanyId == companyId.Value);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            var products = await query.OrderBy(p => p.Category.SortOrder).ThenBy(p => p.SortOrder)
                .Select(p => new ProductListViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name,
                    CompanyName = p.Company.Name,
                    Speed = p.Speed,
                    Quota = p.Quota,
                    Price = p.Price,
                    ContractDuration = p.ContractDuration,
                    IsActive = p.IsActive,
                    SortOrder = p.SortOrder
                }).ToListAsync();

            ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", categoryId);
            ViewBag.Companies = new SelectList(await _context.Companies.Where(c => c.IsActive).ToListAsync(), "Id", "Name", companyId);
            ViewBag.CurrentSearch = search;
            return View("~/Views/Dealer/Products/Index.cshtml", products);
        }
    }
}
