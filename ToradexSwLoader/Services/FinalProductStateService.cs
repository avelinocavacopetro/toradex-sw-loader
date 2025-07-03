using ToradexSwLoader.Models;
using System.Timers;

namespace ToradexSwLoader.Services
{
    public class FinalProductStateService
    {
        private List<FinalProduct> _finalProducts = new();
        public IReadOnlyList<FinalProduct> FinalProducts => _finalProducts;
        private DateTime _lastUpdate;
        public DateTime LastUpdate => _lastUpdate;
        private readonly IServiceProvider _serviceProvider;

        public event Action? OnChange;

        private readonly System.Timers.Timer _refreshTimer;

        public FinalProductStateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _lastUpdate = DateTime.Now;

            var initialInterval = GetValidInterval(GetRefreshTimeFromScopedFilter());

            _refreshTimer = new System.Timers.Timer(initialInterval);
            _refreshTimer.Elapsed += (sender, e) =>
            {
                _lastUpdate = DateTime.Now;
                OnChange?.Invoke();
            };
            _refreshTimer.Start();
        }

        private int GetRefreshTimeFromScopedFilter()
        {
            using var scope = _serviceProvider.CreateScope();
            var filterService = scope.ServiceProvider.GetRequiredService<FilterService>();
            return filterService.RefreshTime;
        }

        private double GetValidInterval(int refreshTimeSeconds)
        {
            if (refreshTimeSeconds < 10 || refreshTimeSeconds > 60)
                return 10_000;
            return refreshTimeSeconds * 1000;
        }

        public void SetFinalProducts(List<FinalProduct> newList)
        {
            if (!AreListsEqual(_finalProducts, newList))
            {
                _finalProducts = newList;
                _lastUpdate = DateTime.Now;
                OnChange?.Invoke();
            }
        }

        private bool AreListsEqual(List<FinalProduct> oldList, List<FinalProduct> newList)
        {
            if (oldList.Count != newList.Count) return false;

            for (int i = 0; i < oldList.Count; i++)
            {
                if (oldList[i].Id != newList[i].Id || oldList[i].Status != newList[i].Status)
                    return false;
            }
            return true;
        }

        public void Dispose()
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
        }
    }
}
