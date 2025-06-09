namespace ToradexSwLoader.Services
{
    public class FilterService
    {
        public event Action? OnFilterChanged;

        public string? SelectedDevice { get; set; }
        public int OnlineTime { get; set; }

        public void ApplyFilter(string? name, int time)
        {
            SelectedDevice = name;
            OnlineTime = time;
            OnFilterChanged?.Invoke();
        }
    }
}
