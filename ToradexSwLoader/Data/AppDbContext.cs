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

    }
}
