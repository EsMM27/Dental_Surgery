namespace Dental_Surgery.Pages.PageViewModels
{
    public class BillViewModel
    {
        public string PatientName { get; set; }
        public string TreatmentName { get; set; }
        public decimal TreatmentCost { get; set; }
        public int UnattendedAppointmentsCount { get; set; }
        public decimal UnattendedAppointmentCharge { get; set; }
        public decimal TotalCost { get; set; }
        public string PatientPhone { get; set; }
        public string PatientAddress { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
