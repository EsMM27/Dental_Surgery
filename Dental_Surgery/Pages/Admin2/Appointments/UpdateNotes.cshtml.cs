using Dental.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    [IgnoreAntiforgeryToken]
    public class UpdateNotesModel : PageModel
    {
        
        private readonly AppDBContext _context;

        public UpdateNotesModel(AppDBContext context)
        {
            _context = context;
        }

        //Data transfer object
        public class NotesUpdateDto
        {
            public int Id { get; set; }
            public string Notes { get; set; }
        }
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            Console.WriteLine("OnPostUpdateAsync hit");

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            Console.WriteLine("Raw body: " + body);

            var data = JsonSerializer.Deserialize<NotesUpdateDto>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
                return BadRequest("Invalid request data.");

            var appointment = await _context.Appointments.FindAsync(data.Id);
            if (appointment == null)
                return NotFound("Appointment not found.");

            appointment.Notes = data.Notes;

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
