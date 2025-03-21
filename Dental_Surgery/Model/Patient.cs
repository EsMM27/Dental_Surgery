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
        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[A-Za-z\s'-]{2,30}$", ErrorMessage = "First name can only contain letters, spaces, apostrophes, and hyphens")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[A-Za-z\s'-]{2,30}$", ErrorMessage = "Last name can only contain letters, spaces, apostrophes, and hyphens")]
        public string LastName { get; set; }
        [RegularExpression(@"^0(7\d{9}|8\d{8})$", ErrorMessage = "Please enter a valid Irish/UK contact number")]
        public string ContactNumber { get; set; }
        [RegularExpression(@"^0(7\d{9}|8\d{8})$", ErrorMessage = "Please enter a valid contact number")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Address is required")]
        [RegularExpression(@"^[A-Za-z0-9\s.,'\/#\-]{5,100}$", ErrorMessage = "Address contains invalid characters")]
        public string Address { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        // Navigation property for Appointments
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
