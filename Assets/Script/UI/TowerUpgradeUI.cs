using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class TowerUpgradeUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text towerNameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text upgradeCostText;
        [SerializeField] private Button   upgradeButton;
        [SerializeField] private Button   closeButton;

        private TowerBase currentTower;

        private void Awake()
        {
            upgradeButton?.onClick.AddListener(OnUpgradeClicked);
            closeButton?.onClick.AddListener(() => UIManager.Instance.HideUpgradePanel());
        }

        public void Show(TowerBase tower)
        {
            currentTower = tower;
            Refresh();
        }

        private void Refresh()
        {
            if (currentTower == null) return;

            TowerData  data = currentTower.data;
            int        lvl  = currentTower.CurrentLevel;
            TowerLevel cfg  = data.GetLevel(lvl);
            int        cost = data.GetUpgradeCost(lvl);

            if (towerNameText  != null) towerNameText.text  = data.towerName;
            if (levelText      != null) levelText.text      = $"Niveau {lvl + 1} / {data.MaxLevel + 1}";

            if (statsText != null)
                statsText.text =
                    $"DMG: {cfg.damage}\n" +
                    $"Cadence: {cfg.fireRate:F1}s\n" +
                    $"Vitesse proj: {cfg.projectileSpeed}\n" +
                    $"Projectiles: {cfg.projectileCount}\n" +
                    $"Portée: {cfg.range}";

            bool isMax = currentTower.IsMaxLevel();

            if (upgradeCostText != null)
                upgradeCostText.text = isMax ? "MAX" : $"Améliorer : {cost} ¤";

            if (upgradeButton != null)
                upgradeButton.interactable = !isMax && EconomyManager.Instance.CanAfford(cost);
        }

        private void OnUpgradeClicked()
        {
            if (currentTower == null) return;
            if (currentTower.TryUpgrade()) Refresh();
        }

        // Refresh si la monnaie change pendant que le panel est ouvert
        private void OnEnable()  => GameEvents.OnCurrencyChanged += _ => Refresh();
        private void OnDisable() => GameEvents.OnCurrencyChanged -= _ => Refresh();
    }
}
