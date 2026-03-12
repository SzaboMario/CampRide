using CampRide.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampRide.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        [BindProperty] public string ConfirmPassword { get; set; } = "";
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet([FromQuery] string? email)
        {
            if (!string.IsNullOrWhiteSpace(email))
                Email = email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "A két jelszó nem egyezik meg.";
                return Page();
            }

            var user = new ApplicationUser { UserName = Email, Email = Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Redirect("/");
            }

            ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            return Page();
        }
    }
}
