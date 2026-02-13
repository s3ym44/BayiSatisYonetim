using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BayiSatisYonetim.Models.ViewModels
{
    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Speed { get; set; }
        public string? Quota { get; set; }
        public decimal Price { get; set; }
        public int? ContractDuration { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }

    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Kategori seçiniz.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Firma seçiniz.")]
        [Display(Name = "Firma")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Ürün adı gereklidir.")]
        [Display(Name = "Ürün Adı")]
        [StringLength(300)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Hız")]
        public string? Speed { get; set; }

        [Display(Name = "Kota")]
        public string? Quota { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir.")]
        [Display(Name = "Fiyat (₺)")]
        [Range(0.01, 999999.99, ErrorMessage = "Geçerli bir fiyat giriniz.")]
        public decimal Price { get; set; }

        [Display(Name = "Sözleşme Süresi (Ay)")]
        public int? ContractDuration { get; set; }

        [Display(Name = "Görsel URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Sıralama")]
        public int SortOrder { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Companies { get; set; } = new();
    }

    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori seçiniz.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Firma seçiniz.")]
        [Display(Name = "Firma")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Ürün adı gereklidir.")]
        [Display(Name = "Ürün Adı")]
        [StringLength(300)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Hız")]
        public string? Speed { get; set; }

        [Display(Name = "Kota")]
        public string? Quota { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir.")]
        [Display(Name = "Fiyat (₺)")]
        [Range(0.01, 999999.99, ErrorMessage = "Geçerli bir fiyat giriniz.")]
        public decimal Price { get; set; }

        [Display(Name = "Sözleşme Süresi (Ay)")]
        public int? ContractDuration { get; set; }

        [Display(Name = "Görsel URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        [Display(Name = "Sıralama")]
        public int SortOrder { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Companies { get; set; } = new();
    }
}
