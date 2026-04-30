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
        [SerializeField] private EnemyHealthBar  healthBar;
        private NavMeshAgent  agent;    
        private float currentHealth;

        public void Initialize(EnemyData data)
        {
            Data          = data;
            currentHealth = data.maxHealth;
            IsDead        = false;  
        }

        // Dans EnemyHealth.cs

        private void Awake()
        {
            if (healthBar == null)
                healthBar = GetComponentInChildren<EnemyHealthBar>();
            enemyAnimator = GetComponent<EnemyAnimator>();
            agent = GetComponent<NavMeshAgent>();
        }

        public void TakeDamage(float amount)
        {
            if (isDead || Data == null) return;
            currentHealth -= amount;
            if (healthBar != null)
            {
                healthBar.TakeDamage(currentHealth, Data.maxHealth);
            }

            if (currentHealth <= 0f) Die();
        }
        public bool IsDead { get; private set; }
        private void Die()
        {
            Debug.Log($"[EnemyHealth] Die() appelé, IsDead avant = {IsDead}");
            if (IsDead) return;
            IsDead = true;
            Debug.Log($"[EnemyHealth] IsDead mis à true");
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
