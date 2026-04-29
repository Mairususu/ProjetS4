using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class TowerShopUI : MonoBehaviour
    {
        [SerializeField] private GameObject    towerCardPrefab;
        [SerializeField] private Transform     contentParent;

        private readonly List<TowerCardUI> cards = new();

        public void RefreshShop(TowerData[] availableTowers)
        {
            // Vider les cartes existantes
            foreach (var c in cards) if (c != null) Destroy(c.gameObject);
            cards.Clear();

            if (availableTowers == null) return;

            foreach (TowerData data in availableTowers)
            {
                GameObject go   = Instantiate(towerCardPrefab, contentParent);
                TowerCardUI card = go.GetComponent<TowerCardUI>();
                if (card != null)
                {
                    card.Setup(data);
                    cards.Add(card);
                }
            }
        }
        public void RefreshAffordability()
        {
            foreach (var card in cards)
            {
                if (card == null) continue;
                bool canAfford = EconomyManager.Instance.CanAfford(card.Data.purchaseCost);
                card.SetAffordable(canAfford);
            }
        }

        private void OnEnable()  => GameEvents.OnCurrencyChanged += _ => RefreshAffordability();
        private void OnDisable() => GameEvents.OnCurrencyChanged -= _ => RefreshAffordability();
    }
}
