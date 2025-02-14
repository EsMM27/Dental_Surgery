using System.ComponentModel.DataAnnotations;

namespace Dental.Model
{
    public class Dentist
    {
            [Key]
            public int Id { get; set; }
            public string AwardingBody { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Specialisation { get; set; }
            public List<Appointment> Appointments { get; set; }
            public string Image { get; internal set; }
    }
}
