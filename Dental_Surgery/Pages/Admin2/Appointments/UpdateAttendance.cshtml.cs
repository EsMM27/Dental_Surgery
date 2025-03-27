using System.Text.Json;
using Dental.DataAccess;
using Dental.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    [IgnoreAntiforgeryToken]
    public class UpdateAttendanceModel : PageModel
    {
        private readonly AppDBContext _context;

        public UpdateAttendanceModel(AppDBContext context)
        {
            _context = context;
        }

        //Data transfer object
        public class AttendanceUpdateDto
        {
            public int Id { get; set; }
            public bool Attend { get; set; }
        }
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            Console.WriteLine("OnPostUpdateAsync hit");

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            Console.WriteLine("Raw body: " + body);

            var data = JsonSerializer.Deserialize<AttendanceUpdateDto>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
                return BadRequest("Invalid request data.");

            var appointment = await _context.Appointments.FindAsync(data.Id);
            if (appointment == null)
                return NotFound("Appointment not found.");

            appointment.attend = data.Attend;

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }





    }
}
