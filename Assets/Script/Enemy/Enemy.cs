using UnityEngine;
namespace TowerDefense
{
    public class EnemyHealth : MonoBehaviour
    {
        public EnemyData Data { get; private set; }

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
            GameEvents.RaiseEnemyKilled(Data);
            Destroy(gameObject);
        }

        public float HealthNormalized =>
            Data != null ? Mathf.Clamp01(currentHealth / Data.maxHealth) : 0f;
    }
}
