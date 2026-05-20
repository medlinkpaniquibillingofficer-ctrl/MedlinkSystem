using MedlinkDialysisCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<PHRequirement> PHRequirements { get; set; }
    }
}