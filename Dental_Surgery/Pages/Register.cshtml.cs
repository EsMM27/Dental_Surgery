using Dental_Surgery.Pages.PageViewModels;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dental_Surgery.Pages
{
    [BindProperties]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public Register Register { get; set; }

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
			{
				var user = new IdentityUser()
				{
					UserName = Register.Email,
					Email = Register.Email
				};

				var result = await _userManager.CreateAsync(user, Register.Password);
				if (result.Succeeded)
				{					
					await _signInManager.SignInAsync(user, false);
					return RedirectToPage("/Index");
					
				}foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}

				
			}
			return Page();

		}
    }
}
