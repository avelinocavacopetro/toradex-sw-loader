using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class FinalProductStateService
    {
        private List<FinalProduct> _finalProducts = new();
        public IReadOnlyList<FinalProduct> FinalProducts => _finalProducts;
        private DateTime _lastUpdate;
        public DateTime LastUpdate => _lastUpdate;

        public event Action? OnChange;

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
    }
}
