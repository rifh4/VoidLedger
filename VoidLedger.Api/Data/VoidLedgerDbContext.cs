using Microsoft.EntityFrameworkCore;
using VoidLedger.Api.Data.Entities;

namespace VoidLedger.Api.Data
{
    public class VoidLedgerDbContext : DbContext
    {
        public VoidLedgerDbContext(DbContextOptions<VoidLedgerDbContext> options) : base(options){}

        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<HoldingEntity> Holdings { get; set; }
        public DbSet<PriceEntity> Prices { get; set; }
        public DbSet<ActionLogEntity> ActionLogs => Set<ActionLogEntity>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use fixed decimal precision for monetary values.
            modelBuilder.Entity<AccountEntity>().Property(a => a.Balance).HasPrecision(18, 2);

            modelBuilder.Entity<PriceEntity>().Property(h => h.Price).HasPrecision(18, 2);
            modelBuilder.Entity<PriceEntity>().Property(h => h.Name).IsRequired().HasMaxLength(64);
            // One current price row per normalized name. This table stores the latest snapshot, not full history.
            modelBuilder.Entity<PriceEntity>().HasIndex(h => h.Name).IsUnique();
            modelBuilder.Entity<PriceEntity>().Property(h => h.PreviousPrice).HasPrecision(18, 2);

            modelBuilder.Entity<HoldingEntity>().Property(h => h.Name).IsRequired().HasMaxLength(64);
            // One account cannot have duplicate holdings for the same normalized name.
            modelBuilder.Entity<HoldingEntity>().HasIndex(h => new { h.AccountId, h.Name }).IsUnique(); 
            modelBuilder.Entity<HoldingEntity>().HasOne<AccountEntity>().WithMany().HasForeignKey(h => h.AccountId);

            modelBuilder.Entity<ActionLogEntity>().Property(a => a.Name).HasMaxLength(64);
            modelBuilder.Entity<ActionLogEntity>().Property(a => a.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<ActionLogEntity>().Property(a => a.Total).HasPrecision(18, 2);
            modelBuilder.Entity<ActionLogEntity>().Property(a => a.Amount).HasPrecision(18, 2);
            // Index action logs by account and time for recent-action queries.
            modelBuilder.Entity<ActionLogEntity>().HasIndex(a => new { a.AccountId, a.AtUtc });
            modelBuilder.Entity<ActionLogEntity>().HasOne<AccountEntity>().WithMany().HasForeignKey(a => a.AccountId);

        }
    }
}