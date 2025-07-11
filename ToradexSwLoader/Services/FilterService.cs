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

        public int OnlineTime { get; set; }
        public int RefreshTime { get; set; }
        public string? SelectedPackage { get; set; }
        public string? Version { get; set; }
        public List<Fleet> SelectedFleets { get; private set; } = new List<Fleet>();
        public List<Product> SelectedProducts { get; private set; } = new List<Product>();
        public List<Device> SelectedDevices { get; private set; } = new List<Device>();
        public List<Stack> SelectedStacks { get; private set; } = new List<Stack>();
        public List<Pattern> SelectedPatterns { get; private set; } = new List<Pattern>();

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
                OnlineTime = filter.OnlineTime;
                RefreshTime = filter.RefreshTime;
                SelectedPackage = filter.SelectedPackage;
                Version = filter.Version;

                SelectedFleets = string.IsNullOrWhiteSpace(filter.SelectedFleetsJson)
                                 ? new List<Fleet>()
                                 : JsonSerializer.Deserialize<List<Fleet>>(filter.SelectedFleetsJson) ?? new List<Fleet>();

                SelectedProducts = string.IsNullOrWhiteSpace(filter.SelectedProductsJson)
                                 ? new List<Product>()
                                 : JsonSerializer.Deserialize<List<Product>>(filter.SelectedProductsJson) ?? new List<Product>();

                SelectedDevices = string.IsNullOrWhiteSpace(filter.SelectedDevicesJson)
                                 ? new List<Device>()
                                 : JsonSerializer.Deserialize<List<Device>>(filter.SelectedDevicesJson) ?? new List<Device>();
                SelectedStacks = string.IsNullOrWhiteSpace(filter.SelectedStacksJson)
                                 ? new List<Stack>()
                                 : JsonSerializer.Deserialize<List<Stack>>(filter.SelectedStacksJson) ?? new List<Stack>();
                SelectedPatterns = string.IsNullOrWhiteSpace(filter.SelectedPatternsJson)
                                 ? new List<Pattern>()
                                 : JsonSerializer.Deserialize<List<Pattern>>(filter.SelectedPatternsJson) ?? new List<Pattern>();
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

            filter.OnlineTime = OnlineTime;
            filter.RefreshTime = RefreshTime;
            filter.SelectedPackage = SelectedPackage;
            filter.Version = Version;
            filter.SelectedFleetsJson = JsonSerializer.Serialize(SelectedFleets);
            filter.SelectedProductsJson = JsonSerializer.Serialize(SelectedProducts);
            filter.SelectedDevicesJson = JsonSerializer.Serialize(SelectedDevices);
            filter.SelectedStacksJson = JsonSerializer.Serialize(SelectedStacks);
            filter.SelectedPatternsJson = JsonSerializer.Serialize(SelectedPatterns);
            filter.LastUpdated = DateTime.Now;

            await context.SaveChangesAsync();
            OnFilterChanged?.Invoke();
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

        public async Task ApplyProductsFilter(List<Product> productsNames)
        {
            SelectedProducts = productsNames;
            await SaveFilterAsync();
        }

        public async Task ApplyDevicesFilter(List<Device> devicesNames)
        {
            SelectedDevices = devicesNames;
            await SaveFilterAsync();
        }

        public async Task ApplyStacksFilter(List<Stack> stacksNames)
        {
            SelectedStacks = stacksNames;
            await SaveFilterAsync();
        }
        public async Task ApplyPatternsFilter(List<Pattern> patternsNames)
        {
            SelectedPatterns = patternsNames;
            await SaveFilterAsync();
        }

        public async Task ApplyTimeFilter(int newTime)
        {
            RefreshTime = newTime;
            await SaveFilterAsync();
        }
    }
}
