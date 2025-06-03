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
                    .Include(p => p.PackageHardwares)
                    .ThenInclude(ph => ph.Hardware)
                    .FirstOrDefaultAsync(p => p.PackageId == package.PackageId);

                if (packageDb == null)
                {
                    packageDb = new Package
                    {
                        PackageId = package.PackageId,
                        PackageName = package.PackageName,
                        PackageVersion = package.PackageVersion,
                        PackageHardwares = new List<PackageHardware>()
                    };

                    context.Packages.Add(packageDb);
                }
                else
                {
                    packageDb.PackageName = package.PackageName;
                    packageDb.PackageVersion = package.PackageVersion;

                    context.PackageHardwares.RemoveRange(packageDb.PackageHardwares);
                    packageDb.PackageHardwares.Clear();
                }

                foreach (var hwId in package.HardwareIds)
                {
                    var hardware = await context.Hardwares
                        .FirstOrDefaultAsync(h => h.HardwareName == hwId);

                    if (hardware == null)
                    {
                        hardware = new Hardware
                        {
                            HardwareName = hwId
                        };
                        context.Hardwares.Add(hardware);
                        await context.SaveChangesAsync();
                    }

                    var packageHardware = new PackageHardware
                    {
                        Package = packageDb,
                        Hardware = hardware
                    };

                    packageDb.PackageHardwares.Add(packageHardware);
                }
                await context.SaveChangesAsync();
            }

            return true;
        }
    }
}
