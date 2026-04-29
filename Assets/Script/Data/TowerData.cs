using UnityEngine;

namespace TowerDefense
{
    [System.Serializable]
    public class TowerLevel
    {
        [Tooltip("Coût d'amélioration pour atteindre CE niveau")]
        public int upgradeCost;

        [Tooltip("Nombre de projectiles tirés simultanément")]
        public int projectileCount = 1;

        [Tooltip("Délai entre deux tirs (secondes)")]
        public float fireRate = 1f;

        [Tooltip("Vitesse des projectiles (unités/s)")]
        public float projectileSpeed = 8f;

        [Tooltip("Dégâts par projectile")]
        public float damage = 10f;

        [Tooltip("Portée de détection des ennemis")]
        public float range = 5f;
    }

    [CreateAssetMenu(fileName = "TowerData_New", menuName = "TowerDefense/TowerData")]
    public class TowerData : ScriptableObject
    {
        [Header("Identité")]
        public string towerName   = "Tour";
        public Sprite icon;
        public GameObject prefab;

        [Header("Coût d'achat initial")]
        public int purchaseCost = 100;

        [Header("Niveaux (index 0 = niveau de base)")]
        public TowerLevel[] levels;

        // ── Helpers ────────────────────────────────────────────

        public int MaxLevel => levels != null ? levels.Length - 1 : 0;

        /// <summary>Retourne la config du niveau demandé (clampé).</summary>
        public TowerLevel GetLevel(int level) =>
            levels[Mathf.Clamp(level, 0, MaxLevel)];

        /// <summary>Coût d'amélioration vers le prochain niveau. -1 si max.</summary>
        public int GetUpgradeCost(int currentLevel)
        {
            int next = currentLevel + 1;
            if (next > MaxLevel) return -1;
            return levels[next].upgradeCost;
        }
    }
}
