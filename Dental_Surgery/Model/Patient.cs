using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Model
{
    public class Patient
    {
        public int PatientId { get; set; }
        [Required]
        [RegularExpression(@"^\d{7}[A-Za-z]{1,2}$", ErrorMessage = "PPS must be 7 digits followed by 1 or 2 letters.")]
        public string PPS { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [RegularExpression(@"^\+353[1-9][0-9]{6,9}$", ErrorMessage = "Please enter a valid phone number")]
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        // Navigation property for Appointments
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
