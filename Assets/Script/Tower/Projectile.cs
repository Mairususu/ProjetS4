using UnityEngine;

namespace TowerDefense
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 5f;

        private Transform target;
        private Vector3   lastKnownPosition;
        private float     speed;
        private float     damage;
        private bool      initialized;
        private bool      impacted;

        public void Initialize(Transform target, float speed, float damage)
        {
            this.target            = target;
            this.lastKnownPosition = target.position;
            this.speed             = speed;
            this.damage            = damage;
            initialized            = true;

            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (!initialized || impacted) return; 

            if (target != null)
                lastKnownPosition = target.position;
    
            Vector3 destination = target != null ? target.position : lastKnownPosition;

            float distanceBefore = Vector3.Distance(transform.position, destination);
            float step = speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, destination, step);

            Vector3 dir = (destination - transform.position);
            if (dir.sqrMagnitude > 0.001f)
                transform.forward = dir.normalized;

            float distanceAfter = Vector3.Distance(transform.position, destination);
            bool passed = step >= distanceBefore; 

            if (distanceAfter < 0.2f || passed)
            {
                if (target != null) OnImpact(target.gameObject);
                else 
                {
                    impacted = true;
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (impacted) return;
            if (!other.CompareTag("Enemy")) return;
            OnImpact(other.gameObject);
        }

        private void OnImpact(GameObject hitObject = null)
        {
            if (impacted) return;
            impacted = true;

            if (hitObject != null)
            {
                EnemyHealth health = hitObject.GetComponentInParent<EnemyHealth>();
        
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
    
            Destroy(gameObject);
        }
    }
}