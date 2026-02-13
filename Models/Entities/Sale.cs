using BayiSatisYonetim.Models.Enums;

namespace BayiSatisYonetim.Models.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; } = null!;
        public int DealerId { get; set; }
        public Dealer Dealer { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public DateTime SaleDate { get; set; }
        public decimal Amount { get; set; }
        public decimal CommissionAmount { get; set; }
        public CommissionStatus CommissionStatus { get; set; } = CommissionStatus.Pending;
        public DateTime? PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
