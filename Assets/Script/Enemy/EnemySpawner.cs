using UnityEngine;
namespace TowerDefense
{ public class EnemySpawner : MonoBehaviour
    {
        [Tooltip("Parent pour garder la hiérarchie propre (optionnel)")]
        [SerializeField] private Transform enemyContainer;
        public GameObject Spawn(EnemyData data)
        {
            if (data?.prefab == null)
            {
                Debug.LogWarning($"[EnemySpawner] Prefab manquant pour {data?.name}");
                return null;
            }

            Vector3    spawnPos = PathDefinition.Instance.StartPoint;
            GameObject go       = Instantiate(data.prefab, spawnPos, Quaternion.identity, enemyContainer);

            if (go.TryGetComponent<EnemyHealth>(out var health)) health.Initialize(data);
            if (go.TryGetComponent<PathFollower>(out var follower)) follower.Initialize(data.moveSpeed);

            go.tag = "Enemy";

            return go;
        }
    }
}

