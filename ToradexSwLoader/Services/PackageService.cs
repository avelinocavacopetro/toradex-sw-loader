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
                var packageDb = await _appDbContext.Packages.FindAsync(package.PackageId);
                if (packageDb == null)
                {
                    _appDbContext.Packages.Add(package);
                }
                else
                {
                    packageDb.PackageName = package.PackageName;
                    packageDb.PackageVersion = package.PackageVersion;

                    _appDbContext.Packages.Update(packageDb);
                }
            }

            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
