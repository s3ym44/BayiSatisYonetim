using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;

namespace BayiSatisYonetim.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userId, string action, string detail, string? ipAddress = null)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                Detail = detail,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
