using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Data;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.ViewModels;

namespace BayiSatisYonetim.Controllers.Customer
{
    [Authorize(Roles = "Customer")]
    public class CustomerProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CustomerProfileController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user!.Id);
            if (customer == null) return RedirectToAction("Login", "Account");

            var vm = new ProfileEditViewModel
            {
                FullName = user!.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Address = customer.Address,
                City = customer.City,
                TCKimlik = customer.TCKimlik,
                BirthDate = customer.BirthDate
            };
            return View("~/Views/Customer/Profile/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid) return View("~/Views/Customer/Profile/Edit.cshtml", model);

            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user!.Id);
            if (customer == null) return RedirectToAction("Login", "Account");

            user!.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            customer.Address = model.Address ?? customer.Address;
            customer.City = model.City ?? customer.City;
            customer.TCKimlik = model.TCKimlik ?? customer.TCKimlik;
            customer.BirthDate = model.BirthDate;
            customer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profil bilgileriniz güncellendi.";
            return RedirectToAction(nameof(Edit));
        }
    }
}
