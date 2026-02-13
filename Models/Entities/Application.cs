using BayiSatisYonetim.Models.Enums;

namespace BayiSatisYonetim.Models.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public int? DealerId { get; set; }
        public Dealer? Dealer { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public string? Notes { get; set; }
        public string? AdminNotes { get; set; }
        public string InstallationAddress { get; set; } = string.Empty;
        public string InstallationCity { get; set; } = string.Empty;
        public DateTime? PreferredDate { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Sale? Sale { get; set; }
    }
}
