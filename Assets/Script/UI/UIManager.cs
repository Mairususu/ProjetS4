using UnityEngine;

namespace TowerDefense
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject placementPanel;   // shop + bouton lancer vague
        [SerializeField] private GameObject upgradePanel;     // panel amélioration tour
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Sous-composants")]
        [SerializeField] private HUDController    hudController;
        [SerializeField] private TowerShopUI      towerShopUI;
        [SerializeField] private TowerUpgradeUI   towerUpgradeUI;


        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnGameStateChanged += OnGameStateChanged;
            GameEvents.OnTowerSelected    += OnTowerSelected;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            GameEvents.OnTowerSelected    -= OnTowerSelected;
        }


        private void OnGameStateChanged(GameState state)
        {
            SetActive(placementPanel, state == GameState.Placement);
            SetActive(upgradePanel,   false);
            SetActive(victoryPanel,   state == GameState.Victory);
            SetActive(gameOverPanel,  state == GameState.GameOver);

            if (state == GameState.Placement)
                towerShopUI?.RefreshShop(PlacementManager.Instance.ShopData);
        }

        private void OnTowerSelected(TowerBase tower)
        {
            if (GameManager.Instance.CurrentState == GameState.Defense) return;
            towerUpgradeUI?.Show(tower);
            SetActive(upgradePanel, true);
        }

        public void HideUpgradePanel()
        {
            SetActive(upgradePanel, false);
        }
        private static void SetActive(GameObject go, bool active)
        {
            if (go != null) go.SetActive(active);
        }
    }
}
