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
        [Key]
        public int AppointmentNo { get; set; }
        //foreign key to link to patient table
        [ForeignKey("Patient")] //need to tell ef core that this is the foreign key
        //panics and creates a whole new column 'PatientPPS' otherwise
        public string PPS { get; set; }
        public Patient Patient { get; set; }
        //foreign key to link to doctor table
        public int DoctorId { get; set; }
        public Dentist Doctor { get; set; }
        //foreign key to link to treatment table
        public int TreatmentNo { get; set; }
        public Treatment treatment { get; set; }
        public bool Attended { get; set; }
        public double AppointmentCost { get; set; }

        public DateTime AppDate { get; set; }
    }
}
