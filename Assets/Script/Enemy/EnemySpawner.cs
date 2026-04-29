using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TowerDefense
{ public class EnemySpawner : MonoBehaviour
    {
        [Tooltip("Parent pour garder la hiérarchie propre (optionnel)")]
        [SerializeField] private Transform enemyContainer;
        public void Spawn(EnemyData data)
        {
            StartCoroutine(SpawnAsync(data));
        }

        private IEnumerator SpawnAsync(EnemyData data)
        {
            AsyncOperationHandle<GameObject> handle =
                data.prefabRef.LoadAssetAsync<GameObject>();

            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[EnemySpawner] Échec chargement prefab : {data.enemyName}");
                yield break;
            }

            Vector3    spawnPos = PathDefinition.Instance.StartPoint;
            GameObject go       = Instantiate(handle.Result, spawnPos, Quaternion.identity, enemyContainer);

            if (go.TryGetComponent<EnemyHealth>(out var health))   health.Initialize(data);
            if (go.TryGetComponent<PathFollower>(out var follower)) follower.Initialize(data.moveSpeed);

            go.tag = "Enemy";

            go.GetComponent<EnemyHealth>().OnDestroyed += () =>
                Addressables.Release(handle);
        }
    }
}

