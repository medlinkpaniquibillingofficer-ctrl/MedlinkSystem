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

        public DbSet<Patient>           Patients            { get; set; }
        public DbSet<PHRequirement>     PHRequirements      { get; set; }
        public DbSet<InventoryItemModel>     InventoryItems      { get; set; }
        public DbSet<StockTransactionModel>  StockTransactions   { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockTransactionModel>()
                .Property(t => t.TransactionType)
                .HasConversion<string>();
        }
    }
}