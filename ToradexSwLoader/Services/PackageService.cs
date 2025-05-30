using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Data;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class PackageService
    {
        private readonly AppDbContext _appDbContext;
        private readonly TorizonService _torizonService;

        public PackageService(AppDbContext appDbContext, TorizonService torizonService)
        {
            _appDbContext = appDbContext;
            _torizonService = torizonService;
        }

        public async Task<bool> ImportPackagesFromApiAsync(string apiUrl)
        {
            bool authOk = await _torizonService.AuthenticateAsync();
            if (!authOk) return false;

            var packages = await _torizonService.GetItemsAsync<Package>(apiUrl);
            if (packages == null) return false;

            foreach (var package in packages)
            {
                var packageDb = await _appDbContext.Packages
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

                    _appDbContext.Packages.Add(packageDb);
                }
                else
                {
                    packageDb.PackageName = package.PackageName;
                    packageDb.PackageVersion = package.PackageVersion;

                    _appDbContext.PackageHardwares.RemoveRange(packageDb.PackageHardwares);
                    packageDb.PackageHardwares.Clear();
                }

                foreach (var hwId in package.HardwareIds)
                {
                    var hardware = await _appDbContext.Hardwares
                        .FirstOrDefaultAsync(h => h.HardwareName == hwId);

                    if (hardware == null)
                    {
                        hardware = new Hardware
                        {
                            HardwareName = hwId
                        };
                        _appDbContext.Hardwares.Add(hardware);
                        await _appDbContext.SaveChangesAsync();
                    }

                    var packageHardware = new PackageHardware
                    {
                        Package = packageDb,
                        Hardware = hardware
                    };

                    packageDb.PackageHardwares.Add(packageHardware);
                }
                await _appDbContext.SaveChangesAsync();
            }

            return true;
        }
    }
}
