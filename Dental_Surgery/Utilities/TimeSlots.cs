namespace Dental_Surgery.Utilities
{
    public static class TimeSlots
    {
        public static readonly List<string> AllTimeSlots = new List<string>
        {
            "09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
            "13:00", "13:30", "14:00", "14:30", "15:00", "15:30"
        };
        // checked against the current day time for example if the current time is 16:00 then the time slots after 16:00 will be removed
        public static List<string> GetAvailableTimeSlots(DateTime appointmentDate)
        {
            var currentTime = DateTime.Now;
            var availableTimeSlots = new List<string>();

            if (appointmentDate.Date == currentTime.Date)
            {
                // Filter time slots for the current day
                availableTimeSlots = AllTimeSlots
                    .Where(timeSlot => DateTime.Parse(timeSlot) > currentTime)
                    .ToList();
            }
            else
            {
                // Return all time slots for future dates
                availableTimeSlots = AllTimeSlots;
            }

            return availableTimeSlots;
        }

    }
}
