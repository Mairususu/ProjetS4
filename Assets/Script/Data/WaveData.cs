using System;
using UnityEngine;

namespace TowerDefense
{
    [Serializable]
    public class WaveEntry
    {
        [Tooltip("Type d'ennemi à spawner")]
        public EnemyData enemyData;

        [Tooltip("Nombre d'ennemis de ce type dans ce groupe")]
        public int count = 5;

        [Tooltip("Intervalle de spawn entre chaque ennemi (secondes)")]
        public float spawnInterval = 0.8f;

        [Tooltip("Délai d'attente après ce groupe avant le suivant")]
        public float delayAfterGroup = 2f;
    }

    [CreateAssetMenu(fileName = "WaveData_New", menuName = "TowerDefense/WaveData")]
    public class WaveData : ScriptableObject
    {
        [Tooltip("Groupes successifs d'ennemis composant la vague")]
        public WaveEntry[] entries;
    }
}
