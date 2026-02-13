using BayiSatisYonetim.Models.Enums;

namespace BayiSatisYonetim.Models.Entities
{
    public class Dealer
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public string CompanyName { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? BankName { get; set; }
        public string? IBAN { get; set; }
        public decimal CommissionRate { get; set; }
        public DealerStatus Status { get; set; } = DealerStatus.Pending;
        public DateTime? ApprovalDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
