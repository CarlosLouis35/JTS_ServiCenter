using System;
using System.Linq;
using JTS_ServiCenter.Data; // Added namespace
using JTS_ServiCenter.Models; // Added namespace

namespace JTS_ServiCenter
{
    public class Clocker
    {
        public bool IsClockedIn { get; private set; }
        public DateTime? ClockInTime { get; private set; }
        // Removed redundant ClockOutTime property

        private readonly AppDbContext _context;
        public int UserId { get; set; }

        public Clocker(AppDbContext context)
        {
            _context = context;
            // Check initial state from DB (optional but good practice)
            var lastEvent = _context.ClockEvents
                .Where(e => e.UserId == this.UserId)
                .OrderByDescending(e => e.ClockInTime)
                .FirstOrDefault();
            if (lastEvent != null && lastEvent.ClockOutTime == null)
            {
                IsClockedIn = true;
                ClockInTime = lastEvent.ClockInTime;
            }
        }

        // Removed inner class definitions for ClockEvent and AppDbContext

        public void ClockIn()
        {
            if (IsClockedIn)
            {
                throw new InvalidOperationException("Already clocked in.");
            }

            IsClockedIn = true;
            ClockInTime = DateTime.Now;
            // ClockOutTime property is not needed here

            var clockEvent = new ClockEvent
            {
                UserId = this.UserId,
                ClockInTime = DateTime.Now, // Use the same timestamp
                ClockOutTime = null // Explicitly null
            };
            _context.ClockEvents.Add(clockEvent);
            _context.SaveChanges();
        }

        public void ClockOut()
        {
            if (!IsClockedIn)
            {
                throw new InvalidOperationException("Not clocked in.");
            }

            var clockOutTimestamp = DateTime.Now;
            // Find the last open clock-in event for this user
            var lastClockInEvent = _context.ClockEvents
                                        .Where(e => e.UserId == this.UserId && e.ClockOutTime == null)
                                        .OrderByDescending(e => e.ClockInTime)
                                        .FirstOrDefault();

            if (lastClockInEvent != null)
            {
                lastClockInEvent.ClockOutTime = clockOutTimestamp;
                _context.SaveChanges();
                IsClockedIn = false;
                ClockInTime = null; // Reset ClockInTime as well
            }
            else
            {
                // Handle case where no open clock-in event is found (data inconsistency?)
                throw new InvalidOperationException("Cannot clock out: No open clock-in record found.");
            }
        }

        public TimeSpan? CalculateTotalTimeWorked(int userId)
        {
            // Ensure consistent timezone handling if applicable
            var totalSeconds = _context.ClockEvents
                .Where(e => e.UserId == userId && e.ClockOutTime.HasValue)
                .ToList() // Fetch data before calculation
                .Sum(e => (e.ClockOutTime.Value - e.ClockInTime).TotalSeconds);

            if (totalSeconds == 0)
            {
                return null;
            }
            return TimeSpan.FromSeconds(totalSeconds);
        }
    }
}
