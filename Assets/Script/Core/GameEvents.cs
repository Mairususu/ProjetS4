
using System;

namespace TowerDefense
{
    public static class GameEvents
    {
        // --- État de jeu ---
        public static event Action<GameState> OnGameStateChanged;
        public static void RaiseGameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);

        // --- Vagues ---
        public static event Action<int> OnWaveStarted;         // numéro de vague
        public static void RaiseWaveStarted(int wave) => OnWaveStarted?.Invoke(wave);

        public static event Action<int> OnWaveCompleted;
        public static void RaiseWaveCompleted(int wave) => OnWaveCompleted?.Invoke(wave);

        // --- Ennemis ---
        public static event Action<EnemyData> OnEnemyKilled;   // ennemi éliminé
        public static void RaiseEnemyKilled(EnemyData data) => OnEnemyKilled?.Invoke(data);

        public static event Action<EnemyData> OnEnemyReachedEnd; // ennemi arrivé au bout
        public static void RaiseEnemyReachedEnd(EnemyData data) => OnEnemyReachedEnd?.Invoke(data);

        public static event Action<int> OnEnemyCountChanged;   // nombre de créatures restantes
        public static void RaiseEnemyCountChanged(int count) => OnEnemyCountChanged?.Invoke(count);

        // --- Économie & Score ---
        public static event Action<int> OnCurrencyChanged;
        public static void RaiseCurrencyChanged(int amount) => OnCurrencyChanged?.Invoke(amount);

        public static event Action<int> OnScoreChanged;
        public static void RaiseScoreChanged(int score) => OnScoreChanged?.Invoke(score);

        public static event Action<int> OnMultiplierChanged;
        public static void RaiseMultiplierChanged(int multiplier) => OnMultiplierChanged?.Invoke(multiplier);

        // --- Tours ---
        public static event Action<TowerBase> OnTowerPlaced;
        public static void RaiseTowerPlaced(TowerBase tower) => OnTowerPlaced?.Invoke(tower);

        public static event Action<TowerBase> OnTowerUpgraded;
        public static void RaiseTowerUpgraded(TowerBase tower) => OnTowerUpgraded?.Invoke(tower);

        public static event Action<TowerBase> OnTowerSelected;
        public static void RaiseTowerSelected(TowerBase tower) => OnTowerSelected?.Invoke(tower);
    }
}
