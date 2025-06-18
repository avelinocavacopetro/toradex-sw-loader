using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Hardware> Hardwares { get; set; }
        public DbSet<PackageHardware> PackageHardwares { get; set; }
        public DbSet<Fleet> Fleets { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; } 
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<GlobalFilterSettings> GlobalFilters { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserPetrotec> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PackageHardware>()
                .HasKey(ph => new { ph.PackageId, ph.HardwareId });

            modelBuilder.Entity<PackageHardware>()
                .HasOne(ph => ph.Package)
                .WithMany(p => p.PackageHardwares)
                .HasForeignKey(ph => ph.PackageId);

            modelBuilder.Entity<PackageHardware>()
                .HasOne(ph => ph.Hardware)
                .WithMany(h => h.PackageHardwares)
                .HasForeignKey(ph => ph.HardwareId);

            modelBuilder.Entity<Hardware>()
                .HasIndex(h => h.HardwareName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
