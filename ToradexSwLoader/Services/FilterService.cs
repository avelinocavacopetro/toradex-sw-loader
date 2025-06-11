using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToradexSwLoader.Data;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class FilterService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public event Action? OnFilterChanged;

        public string? SelectedDevice { get; set; }
        public int OnlineTime { get; set; }
        public string? SelectedPackage { get; set; }
        public string? Version { get; set; }
        public List<Fleet> SelectedFleets { get; private set; } = new List<Fleet>();

        public FilterService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task LoadFilterAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            var filter = await context.GlobalFilters.FirstOrDefaultAsync();
            if(filter != null)
            {
                SelectedDevice = filter.SelectedDevice;
                OnlineTime = filter.OnlineTime;
                SelectedPackage = filter.SelectedPackage;
                Version = filter.Version;

                SelectedFleets = string.IsNullOrWhiteSpace(filter.SelectedFleetsJson)
                                 ? new List<Fleet>()
                                 : JsonSerializer.Deserialize<List<Fleet>>(filter.SelectedFleetsJson) ?? new List<Fleet>();
            }
        }
        
        public async Task SaveFilterAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            var filter = await context.GlobalFilters.FindAsync(1);
            if(filter == null)
            {
                filter = new GlobalFilterSettings { Id = 1 };
                context.GlobalFilters.Add(filter);
            }

            filter.SelectedDevice = SelectedDevice;
            filter.OnlineTime = OnlineTime;
            filter.SelectedPackage = SelectedPackage;
            filter.Version = Version;
            filter.SelectedFleetsJson = JsonSerializer.Serialize(SelectedFleets);
            filter.LastUpdated = DateTime.Now;

            await context.SaveChangesAsync();
            OnFilterChanged?.Invoke();
        }

        public async Task ApplyFilter(string? name, int time)
        {
            SelectedDevice = name;
            OnlineTime = time;
            await SaveFilterAsync();
        }

        public async Task ApplyPackageFilter(string? name, string? version)
        {
            SelectedPackage = name;
            Version = version;
            await SaveFilterAsync();
        }

        public async Task ApplyFleetFilter(List<Fleet> fleetNames)
        {
            SelectedFleets = fleetNames;
            await SaveFilterAsync();
        }
    }
}
