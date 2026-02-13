using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.ViewModels;

namespace BayiSatisYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return View("~/Views/Admin/Announcements/Index.cshtml", announcements);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Announcements/Create.cshtml", new AnnouncementViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementViewModel model)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/Announcements/Create.cshtml", model);

            var announcement = new Announcement
            {
                Title = model.Title,
                Content = model.Content,
                IsPopup = model.IsPopup,
                IsActive = model.IsActive,
                TargetRole = model.TargetRole
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            var vm = new AnnouncementViewModel
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                IsPopup = announcement.IsPopup,
                IsActive = announcement.IsActive,
                TargetRole = announcement.TargetRole
            };
            return View("~/Views/Admin/Announcements/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnnouncementViewModel model)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/Announcements/Edit.cshtml", model);

            var announcement = await _context.Announcements.FindAsync(model.Id);
            if (announcement == null) return NotFound();

            announcement.Title = model.Title;
            announcement.Content = model.Content;
            announcement.IsPopup = model.IsPopup;
            announcement.IsActive = model.IsActive;
            announcement.TargetRole = model.TargetRole;
            announcement.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
