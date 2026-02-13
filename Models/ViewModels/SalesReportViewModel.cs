using System.ComponentModel.DataAnnotations;

namespace BayiSatisYonetim.Models.ViewModels
{
    public class SalesReportViewModel
    {
        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public int? DealerId { get; set; }
        public int? CategoryId { get; set; }

        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCommissions { get; set; }

        public List<SaleReportItem> Sales { get; set; } = new();
        public List<MonthlySalesData> MonthlySales { get; set; } = new();
        public List<CategorySalesData> CategorySales { get; set; } = new();
        public List<TopDealerData> TopDealers { get; set; } = new();
    }

    public class SaleReportItem
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal Amount { get; set; }
        public decimal CommissionAmount { get; set; }
        public string CommissionStatus { get; set; } = string.Empty;
    }
}
