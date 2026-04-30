using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TowerDefense
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform enemyContainer;

        public void Spawn(EnemyData data)
        {
            StartCoroutine(SpawnAsync(data));
        }

        private IEnumerator SpawnAsync(EnemyData data)
        {
            Vector3 spawnPos = PathDefinition.Instance.StartPoint;
            AsyncOperationHandle<GameObject> handle =
                Addressables.InstantiateAsync(data.prefabRef, spawnPos, Quaternion.identity, enemyContainer);

            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[EnemySpawner] Échec instanciation : {data.enemyName}");
                yield break;
            }

            GameObject go = handle.Result;

            if (go.TryGetComponent<EnemyHealth>(out var health))   health.Initialize(data);
            if (go.TryGetComponent<PathFollower>(out var follower)) follower.Initialize(data.moveSpeed);

            go.tag = "Enemy";
            WaveManager.Instance.TrackEnemy(go);

            bool released = false;

            health.OnDestroyed += () =>
            {
                if (released) return;
                released = true;

                if (handle.IsValid())
                    Addressables.ReleaseInstance(handle);
            };
        }
    }
}