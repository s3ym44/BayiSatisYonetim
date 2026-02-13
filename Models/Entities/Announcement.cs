using BayiSatisYonetim.Models.Enums;

namespace BayiSatisYonetim.Models.Entities
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPopup { get; set; }
        public bool IsActive { get; set; } = true;
        public AnnouncementTarget TargetRole { get; set; } = AnnouncementTarget.All;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
