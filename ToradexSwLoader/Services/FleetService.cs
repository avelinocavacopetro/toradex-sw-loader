using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Data;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class FleetService
    {
        readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        readonly TorizonService _torizonService;

        public FleetService(IDbContextFactory<AppDbContext> dbContextFactory, TorizonService torizonService)
        {
            _dbContextFactory = dbContextFactory;
            _torizonService = torizonService;
        }

        public async Task<bool> ImportFleetsFromApiAsync(string apiUrl)
        {
            bool authOk = await _torizonService.AuthenticateAsync();
            if (!authOk) return false;

            var fleets = await _torizonService.GetItemsAsync<Fleet>(apiUrl);
            if (fleets == null) return false;

            using var context = _dbContextFactory.CreateDbContext();

            foreach (var fleet in fleets)
            {
                if (string.IsNullOrWhiteSpace(fleet.Name))
                {
                    throw new Exception($"Fleet com Id {fleet.Id} tem FleetName nulo ou vazio!");
                }

                var fleetDb = await context.Fleets.FindAsync(fleet.Id);
                if (fleetDb == null)
                {
                    context.Fleets.Add(fleet);
                }
                else
                {
                    fleetDb.Name = fleet.Name;
                    fleetDb.CreatedAt = fleet.CreatedAt;
                    fleetDb.FleetType = fleet.FleetType;
                    fleetDb.Expression = fleet.Expression;

                    context.Fleets.Update(fleetDb);
                }
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}