using Microsoft.EntityFrameworkCore;

namespace ToradexSwLoader.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
