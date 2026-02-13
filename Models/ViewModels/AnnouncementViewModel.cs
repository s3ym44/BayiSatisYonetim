using System.ComponentModel.DataAnnotations;
using BayiSatisYonetim.Models.Enums;

namespace BayiSatisYonetim.Models.ViewModels
{
    public class AnnouncementViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir.")]
        [Display(Name = "Başlık")]
        [StringLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik gereklidir.")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Pop-up Olarak Göster")]
        public bool IsPopup { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Hedef Kitle")]
        public AnnouncementTarget TargetRole { get; set; } = AnnouncementTarget.All;
    }
}
