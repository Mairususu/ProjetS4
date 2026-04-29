using UnityEngine;

namespace TowerDefense
{
    public class Projectile : MonoBehaviour
    {
        [Tooltip("Durée de vie maximale en secondes (évite les projectiles perdus)")]
        [SerializeField] private float lifetime = 5f;

        private Transform target;
        private float     speed;
        private float     damage;
        private bool      initialized;

        public void Initialize(Transform target, float speed, float damage)
        {
            this.target  = target;
            this.speed   = speed;
            this.damage  = damage;
            initialized  = true;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (!initialized) return;

            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir      = (target.position - transform.position).normalized;
            float   step     = speed * Time.deltaTime;
            transform.position  = Vector3.MoveTowards(transform.position, target.position, step);
            transform.forward   = dir;

            // Impact si proche
            if (Vector3.Distance(transform.position, target.position) < 0.15f)
                OnImpact();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            OnImpact(other.gameObject);
        }

        private void OnImpact(GameObject hitObject = null)
        {
            GameObject enemy = hitObject ?? target?.gameObject;
            if (enemy != null && enemy.TryGetComponent<EnemyHealth>(out var health))
                health.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
