using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense
{
    [System.Serializable]
    public class TowerLevel
    {
        public int upgradeCost;
        public int projectileCount = 1;
        public float fireRate = 1f;
        public float projectileSpeed = 8f;
        public float damage = 10f;
        public float range = 5f;
    }

    [CreateAssetMenu(fileName = "TowerData_New", menuName = "TowerDefense/TowerData")]
    public class TowerData : ScriptableObject
    {
        [Header("Identité")]
        public string towerName   = "Tour";
        public Sprite icon;
        public AssetReferenceGameObject prefabRef;

        [Header("Coût d'achat initial")]
        public int purchaseCost = 100;

        [Header("Niveaux (index 0 = niveau de base)")]
        public TowerLevel[] levels;
        public int MaxLevel => levels != null ? levels.Length - 1 : 0;
        public TowerLevel GetLevel(int level) =>
            levels[Mathf.Clamp(level, 0, MaxLevel)];
        public int GetUpgradeCost(int currentLevel)
        {
            int next = currentLevel + 1;
            if (next > MaxLevel) return -1;
            return levels[next].upgradeCost;
        }
    }
}
