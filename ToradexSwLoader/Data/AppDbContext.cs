using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DetailedDevice> DetailedDevices { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Hardware> Hardwares { get; set; }
        public DbSet<Fleet> Fleets { get; set; }
        public DbSet<GlobalFilterSettings> GlobalFilters { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserPetrotec> Users { get; set; }
        public DbSet<ProductPackage> ProductPackages { get; set; }
        public DbSet<PackageHardware> PackageHardwares { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; } 
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<DeviceProduct> DeviceProducts { get; set; }

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
                .HasIndex(h => h.Name)
                .IsUnique();

            modelBuilder.Entity<ProductPackage>()
                .HasKey(pp => new { pp.ProductId, pp.PackageId });

            modelBuilder.Entity<ProductPackage>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.ProductPackages)
                .HasForeignKey(pp => pp.ProductId);

            modelBuilder.Entity<ProductPackage>()
                .HasOne(pp => pp.Package)
                .WithMany(p => p.ProductPackages)
                .HasForeignKey(pp => pp.PackageId);
                
            modelBuilder.Entity<DeviceProduct>()
                .HasKey(dp => new { dp.DeviceId, dp.ProductId });

            modelBuilder.Entity<DeviceProduct>()
                .HasOne(dp => dp.Device)
                .WithMany(d => d.DeviceProducts)
                .HasForeignKey(dp => dp.DeviceId);

            modelBuilder.Entity<DeviceProduct>()
                .HasOne(dp => dp.Product)
                .WithMany(p => p.DeviceProducts)
                .HasForeignKey(dp => dp.ProductId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
