using System;

namespace JTS_ServiCenter.Models
{
    public class ClockEvent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
    }
}
