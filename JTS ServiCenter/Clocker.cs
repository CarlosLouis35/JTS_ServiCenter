csharp
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JTS_ServiCenter
{
    public class Clocker
    {
        public bool IsClockedIn { get; private set; }
        public DateTime? ClockInTime { get; private set; }

        private readonly AppDbContext _context;
        public int UserId { get; set; }
        public Clocker(AppDbContext context)
        {
            _context = context;
        }

        public class ClockEvent
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public DateTime ClockInTime { get; set; }
            public DateTime? ClockOutTime { get; set; }
        }

        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<ClockEvent> ClockEvents { get; set; }
        }


        public DateTime? ClockOutTime { get; private set; }

        public void ClockIn()
        {
            if (IsClockedIn)
            {
                throw new InvalidOperationException("Already clocked in.");
            }

            IsClockedIn = true;
            ClockInTime = DateTime.Now;
            ClockOutTime = null;
            
            var clockEvent = new ClockEvent
            {
                UserId = this.UserId,
                ClockInTime = DateTime.Now,
                
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

            IsClockedIn = false;
            ClockOutTime = DateTime.Now;


            var lastClockInEvent = _context.ClockEvents.Where(e => e.UserId == this.UserId && e.ClockOutTime == null).OrderByDescending(e => e.ClockInTime).FirstOrDefault();
            if (lastClockInEvent != null)
            {
                lastClockInEvent.ClockOutTime = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public TimeSpan? CalculateTotalTimeWorked(int userId)
        {
            var totalTime = _context.ClockEvents.Where(e => e.UserId == userId && e.ClockOutTime != null).Sum(e => (e.ClockOutTime - e.ClockInTime).Value.TotalSeconds);
            if(totalTime == 0)
            {
                return null;
            }
            TimeSpan totalTimeWorked = TimeSpan.FromSeconds(totalTime);
            return totalTimeWorked;
        }
    }

    
}