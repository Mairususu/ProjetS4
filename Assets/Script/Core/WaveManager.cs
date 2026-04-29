using System.Collections;
using UnityEngine;

namespace TowerDefense
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        [Header("Données des vagues (dans l'ordre)")]
        [SerializeField] private WaveData[] waves;

        [Header("Références")]
        [SerializeField] private EnemySpawner spawner;

        public int  CurrentWaveIndex { get; private set; } = -1;
        public int  EnemiesRemaining { get; private set; }
        public bool IsSpawning       { get; private set; }

        private int totalSpawned;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnEnemyKilled     += OnEnemyRemoved;
            GameEvents.OnEnemyReachedEnd += OnEnemyRemoved;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyKilled     -= OnEnemyRemoved;
            GameEvents.OnEnemyReachedEnd -= OnEnemyRemoved;
        }

        public bool HasMoreWaves() => CurrentWaveIndex + 1 < waves.Length;

        public void StartNextWave()
        {
            if (!HasMoreWaves()) return;

            CurrentWaveIndex++;
            WaveData wave = waves[CurrentWaveIndex];

            // Calcul du total
            int total = 0;
            foreach (var entry in wave.entries) total += entry.count;

            EnemiesRemaining = total;
            totalSpawned     = 0;
            GameEvents.RaiseWaveStarted(CurrentWaveIndex + 1);
            GameEvents.RaiseEnemyCountChanged(EnemiesRemaining);

            StartCoroutine(SpawnWave(wave));
        }

        private IEnumerator SpawnWave(WaveData wave)
        {
            IsSpawning = true;

            foreach (WaveEntry entry in wave.entries)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    spawner.Spawn(entry.enemyData);
                    totalSpawned++;
                    yield return new WaitForSeconds(entry.spawnInterval);
                }

                if (entry.delayAfterGroup > 0f)
                    yield return new WaitForSeconds(entry.delayAfterGroup);
            }

            IsSpawning = false;
            CheckWaveComplete();
        }

        public void TrackEnemy(GameObject enemy) { }

        private void OnEnemyRemoved(EnemyData _)
        {
            EnemiesRemaining = Mathf.Max(0, EnemiesRemaining - 1);
            GameEvents.RaiseEnemyCountChanged(EnemiesRemaining);

            CheckWaveComplete();
        }

        private void CheckWaveComplete()
        {
            if (IsSpawning) return;
            if (EnemiesRemaining > 0) return;

            GameEvents.RaiseWaveCompleted(CurrentWaveIndex + 1);
            GameManager.Instance.OnWaveFinished();
        }
    }
}
