using Dental.Model;
using Dental.Service;
using Dental_Surgery.Pages.PageViewModels;
using Dental_Surgery.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Surgery.Pages.Bills
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public int SelectedPatientId { get; set; }

        [BindProperty]
        public int SelectedAppointmentId { get; set; }
        public SelectList AppointmentList { get; set; }
        public BillViewModel Bill { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadAppointmentsAsync();

            if (SelectedPatientId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a patient.";
                return Page();
            }

            var patient = await _unitOfWork.Patients.GetByIdAsync(SelectedPatientId);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return Page();
            }

            var appointment = await _unitOfWork.Appointments.GetByIdAsync(SelectedAppointmentId);
            if (appointment == null || appointment.PatientId != SelectedPatientId)
            {
                TempData["ErrorMessage"] = "Appointment not found for the selected patient.";
                return Page();
            }

            var treatment = await _unitOfWork.Treatments.GetByIdAsync(appointment.TreatmentId ?? 0);
            if (treatment == null)
            {
                TempData["ErrorMessage"] = "Treatment not found.";
                return Page();
            }

            // Count unattended appointments
            var unattendedAppointments = (await _unitOfWork.Appointments.GetAllAsync())
                .Where(a => a.PatientId == SelectedPatientId && !a.attend)
                .ToList();

            decimal unattendedCharges = unattendedAppointments.Count * 20m; // Charge per unattended appointment

            Bill = new BillViewModel
            {
                PatientName = $"{patient.FirstName} {patient.LastName}",
                PatientPhone = patient.ContactNumber,
                PatientAddress = patient.Address,
                TreatmentName = treatment.Name,
                TreatmentCost = treatment.Cost,
                UnattendedAppointmentsCount = unattendedAppointments.Count,
                UnattendedAppointmentCharge = unattendedCharges,
                TotalCost = treatment.Cost + unattendedCharges,
                AppointmentDate = appointment.AppointmentDate
            };

            // Mark all unattended appointments as attended
            foreach (var uAppointment in unattendedAppointments)
            {
                uAppointment.attend = true;
                _unitOfWork.Appointments.Update(uAppointment);
            }
            await _unitOfWork.SaveAsync();

            // Call the DocxEditor utility to edit the .docx file
            string docxFilePath = "wwwroot/Bill/dental-invoice-test.docx";
            string fileName = $"{patient.FirstName}_{patient.LastName}_invoice";
            string docxFileCopy = $"wwwroot/Bill/{fileName}.docx";

            // Edit the DOCX file
            DocxEditor.EditDocx(docxFilePath, Bill, docxFileCopy);

            // Pass the PDF file path to the view
            ViewData["PdfFilePath"] = $"/Bill/{fileName}.pdf";

            return Page();
        }

        private async Task LoadAppointmentsAsync()
        {
            if (SelectedPatientId > 0)
            {
                var appointments = (await _unitOfWork.Appointments.GetAllAsync())
                    .Where(a => a.PatientId == SelectedPatientId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToList();

                AppointmentList = new SelectList(appointments, "AppointmentId", "AppointmentDate", appointments.FirstOrDefault()?.AppointmentId);

                if (appointments.Any() && SelectedAppointmentId == 0)
                {
                    SelectedAppointmentId = appointments.First().AppointmentId;
                }
            }
        }
		public async Task<IActionResult> OnGetSearchPatientsAsync(string query)
		{
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

}

