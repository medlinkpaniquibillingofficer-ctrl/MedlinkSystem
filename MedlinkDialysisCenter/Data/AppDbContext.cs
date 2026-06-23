using MedlinkDialysisCenter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient>               Patients            { get; set; }
        public DbSet<PHRequirement>         PHRequirements      { get; set; }
        public DbSet<InventoryItemModel>    InventoryItems      { get; set; }
        public DbSet<StockTransactionModel> StockTransactions   { get; set; }

        public DbSet<HepaTestModel>         HepaTests           { get; set; }
        public DbSet<PatientVaccineModel>   PatientVaccines     { get; set; }

        public DbSet<PhConsumptionModel>    PhConsumptions      { get; set; }

        // In OnModelCreating:




        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockTransactionModel>()
                .Property(t => t.TransactionType)
                .HasConversion<string>();

            modelBuilder.Entity<HepaTestModel>()
                .HasOne(h => h.Patient)
                .WithOne(p => p.HepaTest)        
                .HasForeignKey<HepaTestModel>(h => h.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PatientVaccineModel>()
            .HasOne(v => v.Patient)
            .WithMany(p => p.Vaccines)
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}