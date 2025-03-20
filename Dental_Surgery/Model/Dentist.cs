using System.ComponentModel.DataAnnotations;

namespace Dental.Model
{
    public class Dentist
    {
        public int DentistId { get; set; }
        // assocaite with IdentityUser
        public string? UserId { get; set; }
        // assocaite with IdentityUser
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //degree
        public string Specialization { get; set; }
        //place they got degree
        public string AwardingBody { get; set; }
        public string ContactNumber { get; set; }
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        //image of the doctor 
        public string? Image {  get; set; }

        // Navigation property for Appointments
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
