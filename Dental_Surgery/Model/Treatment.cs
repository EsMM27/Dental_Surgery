using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Model
{
    public class Treatment
    {
        public int TreatmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }

        // Navigation property for Appointments
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
