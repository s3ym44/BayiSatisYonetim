using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.ViewModels;
using BayiSatisYonetim.Services;

namespace BayiSatisYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ProductManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActivityLogService _activityLog;

        public ProductManagementController(ApplicationDbContext context, UserManager<AppUser> userManager, IActivityLogService activityLog)
        {
            _context = context;
            _userManager = userManager;
            _activityLog = activityLog;
        }

        public async Task<IActionResult> Index(int? categoryId, int? companyId, string? search)
        {
            var query = _context.Products.Include(p => p.Category).Include(p => p.Company).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);
            if (companyId.HasValue)
                query = query.Where(p => p.CompanyId == companyId.Value);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            var products = await query.OrderBy(p => p.SortOrder)
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

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);
            ViewBag.Companies = new SelectList(await _context.Companies.ToListAsync(), "Id", "Name", companyId);
            ViewBag.CurrentSearch = search;
            return View("~/Views/Admin/Products/Index.cshtml", products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ProductCreateViewModel
            {
                Categories = await GetCategorySelectList(),
                Companies = await GetCompanySelectList()
            };
            return View("~/Views/Admin/Products/Create.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategorySelectList();
                model.Companies = await GetCompanySelectList();
                return View("~/Views/Admin/Products/Create.cshtml", model);
            }

            var product = new Product
            {
                CategoryId = model.CategoryId,
                CompanyId = model.CompanyId,
                Name = model.Name,
                Description = model.Description,
                Speed = model.Speed,
                Quota = model.Quota,
                Price = model.Price,
                ContractDuration = model.ContractDuration,
                ImageUrl = model.ImageUrl,
                IsActive = model.IsActive,
                SortOrder = model.SortOrder
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var adminUser = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(adminUser!.Id, "Ürün Oluşturma", $"{model.Name} ürünü oluşturuldu");

            TempData["Success"] = "Ürün başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var vm = new ProductEditViewModel
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                CompanyId = product.CompanyId,
                Name = product.Name,
                Description = product.Description,
                Speed = product.Speed,
                Quota = product.Quota,
                Price = product.Price,
                ContractDuration = product.ContractDuration,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                SortOrder = product.SortOrder,
                Categories = await GetCategorySelectList(),
                Companies = await GetCompanySelectList()
            };

            return View("~/Views/Admin/Products/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategorySelectList();
                model.Companies = await GetCompanySelectList();
                return View("~/Views/Admin/Products/Edit.cshtml", model);
            }

            var product = await _context.Products.FindAsync(model.Id);
            if (product == null) return NotFound();

            product.CategoryId = model.CategoryId;
            product.CompanyId = model.CompanyId;
            product.Name = model.Name;
            product.Description = model.Description;
            product.Speed = model.Speed;
            product.Quota = model.Quota;
            product.Price = model.Price;
            product.ContractDuration = model.ContractDuration;
            product.ImageUrl = model.ImageUrl;
            product.IsActive = model.IsActive;
            product.SortOrder = model.SortOrder;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var adminUser = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(adminUser!.Id, "Ürün Güncelleme", $"{model.Name} ürünü güncellendi");

            TempData["Success"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetCategorySelectList()
        {
            return await _context.Categories.Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetCompanySelectList()
        {
            return await _context.Companies.Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();
        }
    }
}
