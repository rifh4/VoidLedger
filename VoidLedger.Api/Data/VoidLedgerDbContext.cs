using Microsoft.EntityFrameworkCore;
using VoidLedger.Api.Data.Entities;

namespace VoidLedger.Api.Data
{
    public class VoidLedgerDbContext : DbContext
    {
        public VoidLedgerDbContext(DbContextOptions<VoidLedgerDbContext> options) : base(options){}

        public DbSet<AccountEntity> Accounts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountEntity>().Property(a => a.Balance).HasPrecision(18, 2);
        }
    }
}