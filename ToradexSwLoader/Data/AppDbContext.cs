using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<DetailedDevice> DetailedDevices { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Fleet> Fleets { get; set; }
        public DbSet<GlobalFilterSettings> GlobalFilters { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserPetrotec> Users { get; set; }
        public DbSet<StackPackage> StackPackages { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; } 
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<DeviceProduct> DeviceProducts { get; set; }
        public DbSet<FinalProduct> FinalProducts { get; set; }
        public DbSet<Stack> Stacks { get; set; }
        public DbSet<Pattern> Patterns { get; set; }
        public DbSet<ProductStack> ProductStacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StackPackage>()
                .HasKey(pp => new { pp.StackId, pp.PackageId });

            modelBuilder.Entity<StackPackage>()
                .HasOne(pp => pp.Stack)
                .WithMany(p => p.StackPackages)
                .HasForeignKey(pp => pp.StackId);

            modelBuilder.Entity<StackPackage>()
                .HasOne(pp => pp.Package)
                .WithMany(p => p.StackPackages)
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

            modelBuilder.Entity<ProductStack>()
                .HasKey(ps => new { ps.ProductId, ps.StackId });

            modelBuilder.Entity<ProductStack>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductStacks)
                .HasForeignKey(ps => ps.ProductId);

            modelBuilder.Entity<ProductStack>()
                .HasOne(ps => ps.Stack)
                .WithMany(s => s.ProductStacks)
                .HasForeignKey(ps => ps.StackId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
