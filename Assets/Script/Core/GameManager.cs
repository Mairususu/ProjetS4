using UnityEngine;

namespace TowerDefense
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Managers")]
        [SerializeField] private WaveManager    waveManager;
        [SerializeField] private EconomyManager economyManager;
        [SerializeField] private ScoreManager   scoreManager;
        [SerializeField] private UIManager      uiManager;
        [SerializeField] private PlacementManager placementManager;

        [Header("Configuration")]
        [Tooltip("Monnaie de départ du joueur")]
        [SerializeField] private int startingCurrency = 150;

        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            economyManager.Initialize(startingCurrency);
            ChangeState(GameState.Placement);
        }


        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            GameEvents.RaiseGameStateChanged(newState);

            switch (newState)
            {
                case GameState.Placement:
                    placementManager.BeginPlacementPhase();
                    break;

                case GameState.Defense:
                    placementManager.EndPlacementPhase();
                    waveManager.StartNextWave();
                    break;

                case GameState.Victory:
                case GameState.GameOver:
                    break;
            }
        }

        public void LaunchWave()
        {
            if (CurrentState != GameState.Placement) return;
            ChangeState(GameState.Defense);
        }

        public void OnWaveFinished()
        {
            if (waveManager.HasMoreWaves())
                ChangeState(GameState.Placement);
            else
                ChangeState(GameState.Victory);
        }
    }
}
