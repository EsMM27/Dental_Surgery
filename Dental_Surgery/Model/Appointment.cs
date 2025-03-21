using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Model
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        [StringLength(250, ErrorMessage = "Notes cannot exceed 250 characters")]
        public string? Notes { get; set; }
        public bool attend { get; set; }

        // Foreign keys
        public int DentistId { get; set; }
        public int PatientId { get; set; }
        public int? TreatmentId { get; set; }

        // Navigation properties
        public Dentist? Dentist { get; set; }
        public Patient? Patient { get; set; }
        public Treatment? Treatment { get; set; }
    }
}
