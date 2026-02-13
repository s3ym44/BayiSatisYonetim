namespace BayiSatisYonetim.Models.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalDealers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int PendingDealers { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCommissions { get; set; }
        public List<MonthlySalesData> MonthlySales { get; set; } = new();
        public List<CategorySalesData> CategorySales { get; set; } = new();
        public List<RecentApplicationData> RecentApplications { get; set; } = new();
        public List<TopDealerData> TopDealers { get; set; } = new();
    }

    public class DealerDashboardVM
    {
        public string CompanyName { get; set; } = string.Empty;
        public string DealerStatus { get; set; } = string.Empty;
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int CompletedApplications { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal PendingCommission { get; set; }
        public List<MonthlySalesData> MonthlySales { get; set; } = new();
        public List<RecentApplicationData> RecentApplications { get; set; } = new();
        public List<AnnouncementData> Announcements { get; set; } = new();
    }

    public class CustomerDashboardVM
    {
        public string FullName { get; set; } = string.Empty;
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int CompletedApplications { get; set; }
        public List<RecentApplicationData> RecentApplications { get; set; } = new();
        public List<AnnouncementData> Announcements { get; set; } = new();
    }

    public class MonthlySalesData
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }

    public class CategorySalesData
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class RecentApplicationData
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TopDealerData
    {
        public string CompanyName { get; set; } = string.Empty;
        public int SalesCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class AnnouncementData
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPopup { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
