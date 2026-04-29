using System;
using UnityEngine;
using UnityEngine.AI;

namespace TowerDefense
{
    public class EnemyHealth : MonoBehaviour
    {
        public EnemyData Data { get; private set; }
        public event Action OnDestroyed; 
        private bool          isDead;       
        private EnemyAnimator enemyAnimator;
        private NavMeshAgent  agent;    
        private float currentHealth;

        public void Initialize(EnemyData data)
        {
            Data          = data;
            currentHealth = data.maxHealth;
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0f) Die();
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            if (agent != null) agent.isStopped = true;

            GameEvents.RaiseEnemyKilled(Data);
            OnDestroyed?.Invoke(); 

            if (enemyAnimator != null)
                enemyAnimator.PlayDeath(() => OnDestroyed?.Invoke());
            else
                OnDestroyed?.Invoke();

        }

        public float HealthNormalized =>
            Data != null ? Mathf.Clamp01(currentHealth / Data.maxHealth) : 0f;
    }
}
