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
        public List<Entity> SelectedEntities { get; private set; } = new List<Entity>();

        public FilterService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task LoadFilterAsync()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

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
                                 : JsonSerializer.Deserialize<List<Fleet>>(filter.SelectedFleetsJson, options) ?? new List<Fleet>();

                SelectedProducts = string.IsNullOrWhiteSpace(filter.SelectedProductsJson)
                                 ? new List<Product>()
                                 : JsonSerializer.Deserialize<List<Product>>(filter.SelectedProductsJson, options) ?? new List<Product>();

                SelectedDevices = string.IsNullOrWhiteSpace(filter.SelectedDevicesJson)
                                 ? new List<Device>()
                                 : JsonSerializer.Deserialize<List<Device>>(filter.SelectedDevicesJson, options) ?? new List<Device>();
                SelectedStacks = string.IsNullOrWhiteSpace(filter.SelectedStacksJson)
                                 ? new List<Stack>()
                                 : JsonSerializer.Deserialize<List<Stack>>(filter.SelectedStacksJson, options) ?? new List<Stack>();
                SelectedPatterns = string.IsNullOrWhiteSpace(filter.SelectedPatternsJson)
                                 ? new List<Pattern>()
                                 : JsonSerializer.Deserialize<List<Pattern>>(filter.SelectedPatternsJson, options) ?? new List<Pattern>();
                SelectedEntities = string.IsNullOrWhiteSpace(filter.SelectedEntitiesJson)
                                 ? new List<Entity>()
                                 : JsonSerializer.Deserialize<List<Entity>>(filter.SelectedEntitiesJson, options) ?? new List<Entity>();
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

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = false
            };

            filter.OnlineTime = OnlineTime;
            filter.RefreshTime = RefreshTime;
            filter.SelectedPackage = SelectedPackage;
            filter.Version = Version;
            filter.SelectedFleetsJson = JsonSerializer.Serialize(SelectedFleets, options);
            filter.SelectedProductsJson = JsonSerializer.Serialize(SelectedProducts, options);
            filter.SelectedDevicesJson = JsonSerializer.Serialize(SelectedDevices, options);
            filter.SelectedStacksJson = JsonSerializer.Serialize(SelectedStacks, options);
            filter.SelectedPatternsJson = JsonSerializer.Serialize(SelectedPatterns, options);
            filter.SelectedEntitiesJson = JsonSerializer.Serialize(SelectedEntities, options);
            filter.LastUpdated = DateTime.Now;

            await context.SaveChangesAsync();
            OnFilterChanged?.Invoke();
        }

        private async Task ApplyFilterAsync<T>(Action<List<T>> setFilter, List<T> value)
        {
            setFilter(value);
            await SaveFilterAsync();
        }

        public async Task ApplyPackageFilter(string? name, string? version)
        {
            SelectedPackage = name;
            Version = version;
            await SaveFilterAsync();
        }

        public Task ApplyFleetFilter(List<Fleet> fleets) =>
            ApplyFilterAsync(f => SelectedFleets = f, fleets);

        public Task ApplyProductsFilter(List<Product> products) =>
            ApplyFilterAsync(p => SelectedProducts = p, products);

        public Task ApplyDevicesFilter(List<Device> devices) =>
            ApplyFilterAsync(d => SelectedDevices = d, devices);

        public Task ApplyStacksFilter(List<Stack> stacks) =>
            ApplyFilterAsync(s => SelectedStacks = s, stacks);

        public Task ApplyPatternsFilter(List<Pattern> patterns) =>
            ApplyFilterAsync(p => SelectedPatterns = p, patterns);

        public Task ApplyEntitiesFilter(List<Entity> entities) =>
            ApplyFilterAsync(e => SelectedEntities = e, entities);

        public async Task ApplyTimeFilter(int newTime)
        {
            RefreshTime = newTime;
            await SaveFilterAsync();
        }
    }
}
