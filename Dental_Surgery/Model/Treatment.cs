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
        [RegularExpression(@"^[A-Za-z\s'-]{2,50}$", ErrorMessage = "Please enter a valid name (letters, spaces, apostrophes, and hyphens only)")]
        public string Name { get; set; }
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string Description { get; set; }
        [Range(0.01, 10000, ErrorMessage = "Cost must be between 0.01 and 10,000")]
        public decimal Cost { get; set; }

        // Navigation property for Appointments
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
