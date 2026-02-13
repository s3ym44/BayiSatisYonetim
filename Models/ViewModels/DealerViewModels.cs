using System.ComponentModel.DataAnnotations;
using BayiSatisYonetim.Models.Enums;

namespace BayiSatisYonetim.Models.ViewModels
{
    public class DealerListViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public decimal CommissionRate { get; set; }
        public DealerStatus Status { get; set; }
        public int SalesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DealerDetailViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? BankName { get; set; }
        public string? IBAN { get; set; }
        public decimal CommissionRate { get; set; }
        public DealerStatus Status { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCommission { get; set; }
        public List<SaleListItem> RecentSales { get; set; } = new();
        public List<RecentApplicationData> RecentApplications { get; set; } = new();
    }

    public class DealerCreateViewModel
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon gereklidir.")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Firma adı gereklidir.")]
        [Display(Name = "Firma Adı")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vergi numarası gereklidir.")]
        [Display(Name = "Vergi Numarası")]
        public string TaxNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres gereklidir.")]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir gereklidir.")]
        [Display(Name = "Şehir")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "Komisyon Oranı (%)")]
        [Range(0, 100, ErrorMessage = "Komisyon oranı 0-100 arasında olmalıdır.")]
        public decimal CommissionRate { get; set; } = 10;
    }

    public class DealerEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Firma adı gereklidir.")]
        [Display(Name = "Firma Adı")]
        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "Vergi Numarası")]
        public string TaxNumber { get; set; } = string.Empty;

        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Şehir")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "Banka Adı")]
        public string? BankName { get; set; }

        [Display(Name = "IBAN")]
        public string? IBAN { get; set; }

        [Display(Name = "Komisyon Oranı (%)")]
        [Range(0, 100, ErrorMessage = "Komisyon oranı 0-100 arasında olmalıdır.")]
        public decimal CommissionRate { get; set; }

        [Display(Name = "Durum")]
        public DealerStatus Status { get; set; }

        [Display(Name = "Notlar")]
        public string? Notes { get; set; }
    }

    public class SaleListItem
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal Amount { get; set; }
        public decimal CommissionAmount { get; set; }
        public CommissionStatus CommissionStatus { get; set; }
    }
}
