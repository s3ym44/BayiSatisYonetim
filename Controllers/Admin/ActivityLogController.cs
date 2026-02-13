using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;

namespace BayiSatisYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ActivityLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, string? action)
        {
            var query = _context.ActivityLogs.Include(a => a.User).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(a => a.User.FullName.Contains(search) || a.Detail.Contains(search));
            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            var logs = await query.OrderByDescending(a => a.CreatedAt).Take(500).ToListAsync();

            ViewBag.Actions = await _context.ActivityLogs.Select(a => a.Action).Distinct().ToListAsync();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentAction = action;
            return View("~/Views/Admin/ActivityLogs/Index.cshtml", logs);
        }
    }
}
