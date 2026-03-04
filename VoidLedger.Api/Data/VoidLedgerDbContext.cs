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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountEntity>().Property(a => a.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<PriceEntity>().Property(h => h.Price).HasPrecision(18, 2);
            modelBuilder.Entity<PriceEntity>().Property(h => h.Name).IsRequired().HasMaxLength(64);
            modelBuilder.Entity<HoldingEntity>().Property(h => h.Name).IsRequired().HasMaxLength(64);
            modelBuilder.Entity<HoldingEntity>()
                .HasIndex(h => new { h.AccountId, h.Name })
                .IsUnique();
            modelBuilder.Entity<PriceEntity>().HasIndex(h => h.Name).IsUnique();
            modelBuilder.Entity<HoldingEntity>().HasOne<AccountEntity>().WithMany().HasForeignKey(h => h.AccountId);

        }
    }
}