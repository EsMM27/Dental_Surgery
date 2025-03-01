﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;

namespace Dental_Surgery.Pages.Receptionist
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dental.Model;
    using Dental.Service;

    namespace Dental_Surgery.Pages.Receptionist
    {
        public class IndexModel : PageModel
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<IndexModel> _logger;

            public IndexModel(IUnitOfWork unitOfWork, ILogger<IndexModel> logger)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
            }

            [BindProperty]
            public Appointment Appointment { get; set; } = new Appointment { AppointmentDate = DateTime.Today };

            [BindProperty]
            public string SelectedTime { get; set; }

            public List<Dentist> Dentists { get; set; } = new();
            public List<(string TimeSlot, bool IsBooked)> TimeSlotsWithAvailability { get; set; } = new();
            public List<Patient> Patients { get; set; } = new();
            public List<Treatment> Treatments { get; set; } = new();

            public async Task<IActionResult> OnGetAsync()
            {
                await LoadDataAsync();

                if (Appointment.DentistId > 0)
                {
                    TimeSlotsWithAvailability = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(
                        Appointment.DentistId,
                        Appointment.AppointmentDate
                    );
                }

                return Page();
            }

            public async Task<IActionResult> OnPostFetchAvailabilityAsync()
            {
                await LoadDataAsync();

                if (Appointment.DentistId <= 0)
                {
                    TempData["ErrorMessage"] = "Please select a dentist.";
                    return Page();
                }

                TimeSlotsWithAvailability = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(
                    Appointment.DentistId,
                    Appointment.AppointmentDate
                );

                return Page();
            }

            public async Task<IActionResult> OnPostCreateAppointmentAsync()
            {
                if (!ValidateAppointment())
                {
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

                await _unitOfWork.Appointments.AddAsync(Appointment);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMessage"] = "Appointment created successfully.";
                return Page();
            }

            private async Task LoadDataAsync()
            {
                Dentists = (List<Dentist>)await _unitOfWork.Dentists.GetAllAsync();
                Patients = (List<Patient>)await _unitOfWork.Patients.GetAllAsync();
                Treatments = (List<Treatment>)await _unitOfWork.Treatments.GetAllAsync();
            }

            private bool ValidateAppointment()
            {
                if (Appointment.DentistId <= 0)
                {
                    TempData["ErrorMessage"] = "Please select a dentist.";
                    return false;
                }

                if (Appointment.PatientId <= 0)
                {
                    TempData["ErrorMessage"] = "Please select a patient.";
                    return false;
                }

                if (Appointment.TreatmentId <= 0)
                {
                    TempData["ErrorMessage"] = "Please select a treatment.";
                    return false;
                }

                if (string.IsNullOrEmpty(SelectedTime))
                {
                    TempData["ErrorMessage"] = "Please select a time.";
                    return false;
                }

                return true;
            }
        }
    }
}