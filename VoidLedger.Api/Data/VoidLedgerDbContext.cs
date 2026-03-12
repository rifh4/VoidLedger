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

        // Central EF-to-SQL mapping for runtime storage.
        // Keep business-shaping constraints here explicit so schema intent stays visible in code.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                // Monetary values use fixed decimal precision so SQL storage matches API/business expectations.
            modelBuilder.Entity<AccountEntity>().Property(a => a.Balance).HasPrecision(18, 2);

            modelBuilder.Entity<PriceEntity>().Property(h => h.Price).HasPrecision(18, 2);
            modelBuilder.Entity<PriceEntity>().Property(h => h.Name).IsRequired().HasMaxLength(64);
                // One current price row per normalized name; this table stores the latest snapshot, not full history.
            modelBuilder.Entity<PriceEntity>().HasIndex(h => h.Name).IsUnique();
            modelBuilder.Entity<PriceEntity>().Property(h => h.PreviousPrice).HasPrecision(18, 2);

            modelBuilder.Entity<HoldingEntity>().Property(h => h.Name).IsRequired().HasMaxLength(64);
                // A single account cannot have duplicate holding rows for the same normalized name.
            modelBuilder.Entity<HoldingEntity>().HasIndex(h => new { h.AccountId, h.Name }).IsUnique(); 
            modelBuilder.Entity<HoldingEntity>().HasOne<AccountEntity>().WithMany().HasForeignKey(h => h.AccountId);

            modelBuilder.Entity<ActionLogEntity>().Property(a => a.Name).HasMaxLength(64);
            modelBuilder.Entity<ActionLogEntity>().Property(a => a.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<ActionLogEntity>().Property(a => a.Total).HasPrecision(18, 2);
            modelBuilder.Entity<ActionLogEntity>().Property(a => a.Amount).HasPrecision(18, 2);
              // Recent-actions queries read by account and time, so index the append-only audit trail on that access pattern.
            modelBuilder.Entity<ActionLogEntity>().HasIndex(a => new { a.AccountId, a.AtUtc });
            modelBuilder.Entity<ActionLogEntity>().HasOne<AccountEntity>().WithMany().HasForeignKey(a => a.AccountId);

        }
    }
}