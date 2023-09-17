using BloodDonation.Types.Entity;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Types.Data
{
    public class BloodDonationDbContext : DbContext
    {
        public DbSet<Hospital> Hospital { get; set; }
        public DbSet<NeedForBlood> NeedForBlood { get; set; }
        public DbSet<User> User { get; set; }

        public BloodDonationDbContext(DbContextOptions<BloodDonationDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}