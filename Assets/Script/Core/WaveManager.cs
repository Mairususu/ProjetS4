// ============================================================
// WaveManager.cs
// Orchestre les vagues : spawning, suivi des ennemis actifs
// ============================================================
// Setup Unity :
//   • Attacher sur le GameObject "WaveManager"
//   • Glisser les WaveData ScriptableObjects dans waves[]
//   • Relier le spawner et le path dans l'Inspector
// ============================================================
using System.Collections;
using System.Collections.Generic;
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

        private readonly HashSet<GameObject> activeEnemies = new();

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

            int total = 0;
            foreach (var entry in wave.entries) total += entry.count;
            EnemiesRemaining = total;

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
                    GameObject enemy = spawner.Spawn(entry.enemyData);
                    if (enemy != null) activeEnemies.Add(enemy);
                    yield return new WaitForSeconds(entry.spawnInterval);
                }

                if (entry.delayAfterGroup > 0f)
                    yield return new WaitForSeconds(entry.delayAfterGroup);
            }
            IsSpawning = false;
        }
        
        public void RegisterEnemy(GameObject enemy) => activeEnemies.Add(enemy);

        private void OnEnemyRemoved(EnemyData _)
        {
            EnemiesRemaining = Mathf.Max(0, EnemiesRemaining - 1);
            GameEvents.RaiseEnemyCountChanged(EnemiesRemaining);
            activeEnemies.RemoveWhere(e => e == null);
            if (!IsSpawning && activeEnemies.Count == 0)
            {
                GameEvents.RaiseWaveCompleted(CurrentWaveIndex + 1);
                GameManager.Instance.OnWaveFinished();
            }
        }
    }
}
