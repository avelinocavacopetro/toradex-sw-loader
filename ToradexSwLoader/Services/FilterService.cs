using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class FilterService
    {
        public event Action? OnFilterChanged;

        public string? SelectedDevice { get; set; }
        public int OnlineTime { get; set; }
        public List<Fleet> SelectedFleets { get; private set; } = new List<Fleet>();

        public void ApplyFilter(string? name, int time)
        {
            SelectedDevice = name;
            OnlineTime = time;
            OnFilterChanged?.Invoke();
        }

        public void ApplyFleetFilter(List<Fleet> fleetNames)
        {
            SelectedFleets = fleetNames;
            OnFilterChanged?.Invoke();
        }
    }
}
