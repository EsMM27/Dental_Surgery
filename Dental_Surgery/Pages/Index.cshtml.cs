using Dental.DataAccess;
using Dental.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dental_Surgery.Pages
{

	public class IndexModel : PageModel
	{
		private readonly AppDBContext _context;

		public IndexModel(AppDBContext context)
		{
			_context = context;
		}
		//redirect dentist page if dentist logged in
		public IActionResult OnGet()
		{
			if (User.IsInRole("Dentist"))
			{
				return RedirectToPage("DentistView/Index");
			}
			else if (User.IsInRole("Receptionist"))
			{
				return RedirectToPage("ReceptionistView/Index");
			}
			else if (User.IsInRole("Admin"))
			{
				return RedirectToPage("Admin2/Analytics/Index");
			}
			else
			{
				// Default fallback if no known role
				return RedirectToPage("Login");
			}
		}
	}
}
