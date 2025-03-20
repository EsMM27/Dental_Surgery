using Dental_Surgery.Pages.PageViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dental_Surgery.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        [BindProperty]
        public Login Login { get; set; } // Changed to LoginViewModel

        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
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
                    return RedirectToPage("/Index");
                }
                else {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                
            }
            return RedirectToPage("/Index");
        }
    }
}
