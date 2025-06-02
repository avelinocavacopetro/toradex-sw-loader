using ToradexSwLoader.Data;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class FleetService
    {
        private readonly AppDbContext _appDbContext;
        private readonly TorizonService _torizonService;

        public FleetService(AppDbContext appDbContext, TorizonService torizonService)
        {
            _appDbContext = appDbContext;
            _torizonService = torizonService;
        }

        public async Task<bool> ImportFleetsFromApiAsync(string apiUrl)
        {
            bool authOk = await _torizonService.AuthenticateAsync();
            if (!authOk) return false;

            var fleets = await _torizonService.GetItemsAsync<Fleet>(apiUrl);
            if (fleets == null) return false;

            foreach (var fleet in fleets)
            {
                if (string.IsNullOrWhiteSpace(fleet.FleetName))
                {
                    throw new Exception($"Fleet com Id {fleet.Id} tem FleetName nulo ou vazio!");
                }

                var fleetDb = await _appDbContext.Fleets.FindAsync(fleet.Id);
                if (fleetDb == null)
                {
                    _appDbContext.Fleets.Add(fleet);
                }
                else
                {
                    // Atualiza campos
                    fleetDb.FleetName = fleet.FleetName;
                    fleetDb.CreatedAt = fleet.CreatedAt;
                    fleetDb.FleetType = fleet.FleetType;
                    fleetDb.Expression = fleet.Expression;

                    _appDbContext.Fleets.Update(fleetDb);
                }
            }

            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
