using Dental.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAccess.Repo
{
    public interface IAppointmentRepo : IRepository<Appointment, int>
    {
        Task<List<(string TimeSlot, bool IsBooked)>> GetTimeSlotsWithAvailabilityAsync(int dentistId, DateTime appointmentDate);
        public void Update(Appointment appointment);

    }
}
