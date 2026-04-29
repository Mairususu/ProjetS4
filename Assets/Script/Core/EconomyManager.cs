using UnityEngine;

namespace TowerDefense
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        public int Currency { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Initialize(int startAmount)
        {
            Currency = startAmount;
            GameEvents.RaiseCurrencyChanged(Currency);
        }

        public bool TrySpend(int amount)
        {
            if (amount > Currency) return false;
            Currency -= amount;
            GameEvents.RaiseCurrencyChanged(Currency);
            return true;
        }

        public void Earn(int amount)
        {
            Currency += amount;
            GameEvents.RaiseCurrencyChanged(Currency);
        }

        public bool CanAfford(int amount) => Currency >= amount;
    }
}
