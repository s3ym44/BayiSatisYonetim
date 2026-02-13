using System.ComponentModel.DataAnnotations;
using BayiSatisYonetim.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BayiSatisYonetim.Models.ViewModels
{
    public class ApplicationListViewModel
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? DealerName { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public string InstallationCity { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ApplicationCreateViewModel
    {
        [Required(ErrorMessage = "Ürün seçiniz.")]
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }

        [Display(Name = "Müşteri")]
        public int? CustomerId { get; set; }

        // Yeni müşteri bilgileri (bayi tarafından oluşturulurken)
        [Display(Name = "Müşteri Ad Soyad")]
        public string? CustomerFullName { get; set; }

        [Display(Name = "Müşteri E-posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string? CustomerEmail { get; set; }

        [Display(Name = "Müşteri Telefon")]
        public string? CustomerPhone { get; set; }

        [Display(Name = "TC Kimlik No")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
        public string? CustomerTCKimlik { get; set; }

        [Required(ErrorMessage = "Kurulum adresi gereklidir.")]
        [Display(Name = "Kurulum Adresi")]
        public string InstallationAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kurulum şehri gereklidir.")]
        [Display(Name = "Kurulum Şehri")]
        public string InstallationCity { get; set; } = string.Empty;

        [Display(Name = "Tercih Edilen Tarih")]
        [DataType(DataType.Date)]
        public DateTime? PreferredDate { get; set; }

        [Display(Name = "Notlar")]
        public string? Notes { get; set; }

        public List<SelectListItem> Products { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
    }

    public class ApplicationDetailViewModel
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerTCKimlik { get; set; }
        public string? DealerName { get; set; }
        public string? DealerCompany { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? AdminNotes { get; set; }
        public string InstallationAddress { get; set; } = string.Empty;
        public string InstallationCity { get; set; } = string.Empty;
        public DateTime? PreferredDate { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ApplicationStatusUpdateViewModel
    {
        public int Id { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? AdminNotes { get; set; }
    }
}
