using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Data;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class PackageService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly TorizonService _torizonService;

        public PackageService(IDbContextFactory<AppDbContext> dbContextFactory, TorizonService torizonService)
        {
            _dbContextFactory = dbContextFactory;
            _torizonService = torizonService;
        }

        public async Task<bool> ImportPackagesFromApiAsync(string apiUrl)
        {
            bool authOk = await _torizonService.AuthenticateAsync();
            if (!authOk) return false;

            var packages = await _torizonService.GetItemsAsync<Package>(apiUrl);
            if (packages == null) return false;

            using var context = _dbContextFactory.CreateDbContext();

            foreach (var package in packages)
            {
                var packageDb = await context.Packages
                    .FirstOrDefaultAsync(p => p.Id == package.Id);

                if (packageDb == null)
                {
                    packageDb = new Package
                    {
                        Id = package.Id,
                        Name = package.Name,
                        Version = package.Version,
                        Uri = package.Uri
                    };

                    context.Packages.Add(packageDb);
                }
                else
                {
                    packageDb.Name = package.Name;
                    packageDb.Version = package.Version;
                }

                await context.SaveChangesAsync();
            }

            return true;
        }
    }
}
