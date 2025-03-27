using System.ComponentModel.DataAnnotations;

namespace Dental.Model
{
    public class Dentist
    {
        public int DentistId { get; set; }
        // assocaite with IdentityUser
        public string? UserId { get; set; }
        // assocaite with IdentityUser
        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[A-Za-z\s'-]{2,30}$", ErrorMessage = "First name can only contain letters, spaces, apostrophes, and hyphens")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[A-Za-z\s'-]{2,30}$", ErrorMessage = "Last name can only contain letters, spaces, apostrophes, and hyphens")]
        public string LastName { get; set; }

        //degree
        [Required(ErrorMessage = "Specialization is required")]
        [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Specialization must only contain letters and spaces")]
        public string Specialization { get; set; }
        //place they got degree
        public string AwardingBody { get; set; }
        [RegularExpression(@"^0(7\d{9}|8\d{8})$", ErrorMessage = "Please enter a valid contact number")]

        public string ContactNumber { get; set; }
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        //image of the doctor 
        public string? Image {  get; set; }

        // Navigation property for Appointments
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
