using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "EnemyData_New", menuName = "TowerDefense/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identité")]
        public string enemyName = "Créature";
        public AssetReferenceGameObject prefabRef;

        [Header("Propriétés")]
        [Tooltip("Points de vie de la créature")]
        public float maxHealth = 100f;

        [Tooltip("Vitesse de déplacement le long du chemin (unités/s)")]
        public float moveSpeed = 2f;

        [Header("Score & Récompense")]
        [Tooltip("Score ajouté si éliminée, retiré si elle atteint le bout")]
        public int scoreValue = 10;

        [Tooltip("Monnaie reçue à l'élimination")]
        public int reward = 5;
    }
}
