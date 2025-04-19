using JTS_ServiCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace JTS_ServiCenter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ClockEvent> ClockEvents { get; set; }
    }
}
