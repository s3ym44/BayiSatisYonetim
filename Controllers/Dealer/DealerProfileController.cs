using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.ViewModels;

namespace BayiSatisYonetim.Controllers.Dealer
{
    [Authorize(Roles = "Dealer")]
    public class DealerProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DealerProfileController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var dealer = await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == user!.Id);
            if (dealer == null) return RedirectToAction("Login", "Account");

            var vm = new ProfileEditViewModel
            {
                FullName = user!.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                CompanyName = dealer.CompanyName,
                Address = dealer.Address,
                City = dealer.City,
                BankName = dealer.BankName,
                IBAN = dealer.IBAN
            };
            return View("~/Views/Dealer/Profile/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid) return View("~/Views/Dealer/Profile/Edit.cshtml", model);

            var user = await _userManager.GetUserAsync(User);
            var dealer = await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == user!.Id);
            if (dealer == null) return RedirectToAction("Login", "Account");

            user!.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            dealer.CompanyName = model.CompanyName ?? dealer.CompanyName;
            dealer.Address = model.Address ?? dealer.Address;
            dealer.City = model.City ?? dealer.City;
            dealer.BankName = model.BankName;
            dealer.IBAN = model.IBAN;
            dealer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profil bilgileriniz güncellendi.";
            return RedirectToAction(nameof(Edit));
        }
    }
}
