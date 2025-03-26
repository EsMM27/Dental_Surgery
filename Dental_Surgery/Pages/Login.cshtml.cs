using Dental_Surgery.Pages.PageViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dental_Surgery.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public Login Login { get; set; } // Changed to LoginViewModel

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Login.Email, Login.Password, Login.RememberMe, false);
                if (result.Succeeded)
                {
					var user = await _userManager.FindByEmailAsync(Login.Email);
					var roles = await _userManager.GetRolesAsync(user);

					if (roles.Contains("Admin"))
					{
						return RedirectToPage("/Index");
					}
					else if (roles.Contains("Receptionist"))
					{
						return RedirectToPage("/Receptionist/Index");
					}
					else if (roles.Contains("Dentist"))
					{
						return RedirectToPage("Dentists/Index");
					}
					else
					{
						// Default fallback if no known role
						return RedirectToPage("/Index");
					}

                }
                else {
                    ModelState.AddModelError(string.Empty, "Invalid login.");
                    return Page();
                }
                
            }
            return Page();
        }
    }
}
