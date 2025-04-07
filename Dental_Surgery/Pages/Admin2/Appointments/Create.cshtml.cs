using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dental.DataAccess;
using Dental.Model;
using Dental.Service;
using Microsoft.Extensions.Logging;
using Dental_Surgery.Utilities;
using Microsoft.AspNetCore.Authorization;
using Dental_Surgery.DataAccess.Repo;


namespace Dental_Surgery.Pages.Admin2.Appointments
{
	[Authorize(Roles = "Admin,Receptionist")]
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailService _emailService;

        public CreateModel(IUnitOfWork unitOfWork, ILogger<IndexModel> logger, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailService = emailService; // Initialize email service
            TimeSlotsWithAvailability = new List<(DateTime Date, string TimeSlot, bool IsBooked)>();
            SelectedTime = string.Empty;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment { AppointmentDate = DateTime.Today };

        [BindProperty]
        public string SelectedTime { get; set; }
        [BindProperty]
        public string PatientSearchQuery { get; set; }
        [BindProperty]
        public int SelectedTreatmentId { get; set; }

        public List<Dentist> Dentists { get; set; } = new();
        public List<(DateTime Date, string TimeSlot, bool IsBooked)> TimeSlotsWithAvailability { get; set; }
        public List<Patient> Patients { get; set; } = new();
        public List<Treatment> Treatments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("OnGetAsync called");
            await LoadDataAsync();

            if (Appointment.DentistId > 0)
            {
                await LoadTimeSlotsAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostFetchAvailabilityAsync()
        {
            _logger.LogInformation("OnPostFetchAvailabilityAsync called");
            await LoadDataAsync();

            if (Appointment.DentistId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a dentist.";
                _logger.LogWarning("No dentist selected");
                return Page();
            }

            var startOfWeek = Appointment.AppointmentDate.StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(4);

            TimeSlotsWithAvailability = new List<(DateTime Date, string TimeSlot, bool IsBooked)>();

            for (var date = startOfWeek; date <= endOfWeek; date = date.AddDays(1))
            {
                var dailySlots = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(Appointment.DentistId, date);
                TimeSlotsWithAvailability.AddRange(dailySlots.Select(slot => (date, slot.TimeSlot, slot.IsBooked)));
            }

            if (Appointment.PatientId > 0)
            {
                var patient = await _unitOfWork.Patients.GetByIdAsync(Appointment.PatientId);
                if (patient != null)
                {
                    PatientSearchQuery = $"{patient.FirstName} {patient.LastName} ({patient.PPS})";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAppointmentAsync()
        {
            _logger.LogInformation("OnPostCreateAppointmentAsync called");
            if (!ValidateAppointment())
            {
                await LoadDataAsync();
                return Page();
            }

            var timeParts = SelectedTime.Split(':');
            Appointment.AppointmentDate = new DateTime(
                Appointment.AppointmentDate.Year,
                Appointment.AppointmentDate.Month,
                Appointment.AppointmentDate.Day,
                int.Parse(timeParts[0]),
                int.Parse(timeParts[1]),
                0
            );

            Appointment.TreatmentId = SelectedTreatmentId;

            _logger.LogInformation("Creating appointment for PatientId: {PatientId}, DentistId: {DentistId}, Date: {Date}, Time: {Time}, TreatmentId: {TreatmentId}",
                Appointment.PatientId, Appointment.DentistId, Appointment.AppointmentDate, SelectedTime, Appointment.TreatmentId);

            await _unitOfWork.Appointments.AddAsync(Appointment);
            await _unitOfWork.SaveAsync();

            // Send email to the patient
            var patient = await _unitOfWork.Patients.GetByIdAsync(Appointment.PatientId);
            var dentist = await _unitOfWork.Dentists.GetByIdAsync(Appointment.DentistId);
            var treatment = await _unitOfWork.Treatments.GetByIdAsync(SelectedTreatmentId);

            if (patient != null && dentist != null && treatment != null)
            {
                var subject = "Appointment Confirmation";
                var message = $@"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Appointment Confirmation - Tooth Hurty Dental</title>
    <link href=""https://fonts.googleapis.com/css2?family=Volkhov&display=swap"" rel=""stylesheet"">
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css"" />
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            width: 100%;
            max-width: 600px;
            margin: auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}

        .header {{
            background-color: #4caf50;
            color: white;
            text-align: center;
            padding: 20px;
            display: flex;
            justify-content: center;
            align-items: center;
        }}

        .header .fas {{
            font-size: 3rem;
        }}

        .content {{
            padding: 20px;
            color: #333;
            line-height: 1.6;
        }}

        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 12px;
            color: #888;
            background-color: #f1f1f1;
        }}

        .appointment-details {{
            background-color: #f0f8ff;
            padding: 15px;
            border-radius: 4px;
            margin-top: 15px;
            border: 1px solid #d1e7dd;
        }}

        .appointment-details strong {{
            color: #333;
        }}

        .font-volkhov {{
            font-family: 'Volkhov', serif;
        }}

        .ms-3 {{
            margin-left: 1rem;
        }}

        .fs-1 {{
            font-size: 2rem;
        }}

        .mb-0 {{
            margin-bottom: 0;
        }}

        .text-white {{
            color: white;
        }}
    </style>
</head>

<body>
    <div class=""email-container"">
        <div class=""header"">
            <i class=""fas fa-tooth text-white""></i>
            <p class=""ms-3 fs-1 mb-0 font-volkhov"">Tooth Hurty Dental</p>
        </div>
        <div class=""content"">
            <p>Dear {patient.FirstName} {patient.LastName},</p>

            <p>Your appointment at <strong>Tooth Hurty Dental</strong> has been successfully scheduled. Here are your appointment details:</p>

            <div class=""appointment-details"">
                <p><strong>Date:</strong> {Appointment.AppointmentDate:MMMM dd, yyyy}</p>
                <p><strong>Time:</strong> {Appointment.AppointmentDate:hh:mm tt}</p>
                <p><strong>Dentist:</strong> Dr. {dentist.LastName}</p>
                <p><strong>Treatment:</strong> {treatment?.Name}</p>
                <p><strong>Description:</strong> {treatment?.Description}</p>
                <p><strong>Location:</strong> Dental Clinic, Letterkenny</p>
            </div>

            <p>Please arrive 10 minutes early to complete any necessary paperwork. If you need to reschedule, kindly call us at your earliest convenience.</p>

            <p>We look forward to seeing you soon and helping you maintain a healthy smile!</p>

            <p>Warm regards,<br>
                <strong>Tooth Hurty Dental Team</strong>
            </p>
        </div>
        <div class=""footer"">
            <p>Tooth Hurty Dental | Dental Clinic, Letterkenny | Phone: 083-2239325 | Email: DentalClinic@gmail.com</p>
        </div>
    </div>
</body>

</html>";

                await _emailService.SendEmailAsync(patient.Email, subject, message);
            }

            TempData["SuccessMessage"] = "Appointment created successfully.";
            return RedirectToPage("/Admin2/Appointments/Index");
        }

        private async Task LoadDataAsync()
        {
            _logger.LogInformation("Loading data");
            Dentists = (await _unitOfWork.Dentists.GetAllAsync()).ToList();
            Patients = (await _unitOfWork.Patients.GetAllAsync()).ToList();
            Treatments = (await _unitOfWork.Treatments.GetAllAsync()).ToList();
        }

        private async Task LoadTimeSlotsAsync()
        {
            _logger.LogInformation("Loading time slots for DentistId: {DentistId}, Date: {Date}", Appointment.DentistId, Appointment.AppointmentDate);
            var timeSlots = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(
                Appointment.DentistId,
                Appointment.AppointmentDate
            );

            var availableTimeSlots = TimeSlots.GetAvailableTimeSlots(Appointment.AppointmentDate);
            TimeSlotsWithAvailability = timeSlots
                .Where(ts => availableTimeSlots.Contains(ts.TimeSlot))
                .Select(ts => (Appointment.AppointmentDate, ts.TimeSlot, ts.IsBooked))
                .ToList();
        }

        private bool ValidateAppointment()
        {
            if (Appointment.DentistId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a dentist.";
                _logger.LogWarning("Validation failed: No dentist selected");
                return false;
            }

            if (Appointment.PatientId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a patient.";
                _logger.LogWarning("Validation failed: No patient selected");
                return false;
            }

            if (string.IsNullOrEmpty(SelectedTime))
            {
                TempData["ErrorMessage"] = "Please select a time.";
                _logger.LogWarning("Validation failed: No time selected");
                return false;
            }

            if (SelectedTreatmentId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a treatment.";
                _logger.LogWarning("Validation failed: No treatment selected");
                return false;
            }

            return true;
        }

        public async Task<IActionResult> OnGetSearchPatientsAsync(string query)
        {
            _logger.LogInformation("OnGetSearchPatientsAsync called with query: {Query}", query);
            if (string.IsNullOrEmpty(query))
            {
                return new JsonResult(new List<Patient>());
            }

            var allPatients = (await _unitOfWork.Patients.GetAllAsync()).ToList();

            var filteredPatients = allPatients
                .Where(p => (p.FirstName != null && p.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                            (p.LastName != null && p.LastName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                            (p.PPS != null && p.PPS.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Select(p => new { p.PatientId, p.FirstName, p.LastName, p.PPS })
                .ToList();

            return new JsonResult(filteredPatients);
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}