using UnityEngine;

namespace TowerDefense
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Multiplicateur")]
        [Tooltip("Valeur maximale absolue du multiplicateur")]
        [SerializeField] private int maxMultiplier = 5;
        [Tooltip("Éliminations consécutives nécessaires pour +1 multiplicateur")]
        [SerializeField] private int killStreakThreshold = 5;
        [Tooltip("Fuites consécutives nécessaires pour -1 multiplicateur")]
        [SerializeField] private int leakStreakThreshold = 3;

        public int Score       { get; private set; }
        public int Multiplier  { get; private set; } = 1;

        private int killStreak; // nb éliminations depuis dernier reset/palier
        private int leakStreak; // nb fuites depuis dernier reset/palier

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnEnemyKilled      += HandleEnemyKilled;
            GameEvents.OnEnemyReachedEnd  += HandleEnemyReachedEnd;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyKilled      -= HandleEnemyKilled;
            GameEvents.OnEnemyReachedEnd  -= HandleEnemyReachedEnd;
        }

        private void HandleEnemyKilled(EnemyData data)
        {
            AddScore(data.scoreValue);

            EconomyManager.Instance.Earn(data.reward);

            if (Multiplier < 0)
            {
                Multiplier = 1;
                killStreak = 0;
                GameEvents.RaiseMultiplierChanged(Multiplier);
            }

            leakStreak = 0;

            killStreak++;
            if (killStreak >= killStreakThreshold)
            {
                killStreak = 0;
                ChangeMultiplier(1);
            }
        }

        private void HandleEnemyReachedEnd(EnemyData data)
        {
            AddScore(-data.scoreValue);

            if (Multiplier > 0)
            {
                Multiplier = -1;
                leakStreak = 0;
                GameEvents.RaiseMultiplierChanged(Multiplier);
            }

            killStreak = 0;

            leakStreak++;
            if (leakStreak >= leakStreakThreshold)
            {
                leakStreak = 0;
                ChangeMultiplier(-1);
            }
        }

        private void AddScore(int baseValue)
        {
            Score += baseValue * Multiplier;
            GameEvents.RaiseScoreChanged(Score);
        }

        private void ChangeMultiplier(int delta)
        {
            Multiplier = Mathf.Clamp(Multiplier + delta, -maxMultiplier, maxMultiplier);
            GameEvents.RaiseMultiplierChanged(Multiplier);
        }
    }
}
