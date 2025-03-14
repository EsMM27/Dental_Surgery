using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;

namespace Dental_Surgery.Pages.Dentists
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(ILogger<IndexModel> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public List<Dentist> Dentists { get; set; } = new();
        public List<Appointment> DailyAppointments { get; set; } = new();
        public List<Patient> Patients { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int SelectedDentistId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime ScheduleDate { get; set; } = DateTime.Today;

        public async Task OnGetAsync()
        {
            Dentists = (await _unitOfWork.Dentists.GetAllAsync()).ToList();
            Patients = (await _unitOfWork.Patients.GetAllAsync()).ToList();

            if (SelectedDentistId > 0)
            {
                DailyAppointments = (await _unitOfWork.Appointments
                    .GetAppointmentsForDentistAsync(SelectedDentistId, ScheduleDate))
                    .OrderBy(a => a.AppointmentDate.TimeOfDay)
                    .ToList();
            }
        }
    }
}
