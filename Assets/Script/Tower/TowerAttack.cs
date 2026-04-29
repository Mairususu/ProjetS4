using UnityEngine;

namespace TowerDefense
{
    public class TowerAttack : MonoBehaviour
    {
        [Header("Références")]
        [Tooltip("Points de départ des projectiles (un par canon)")]
        [SerializeField] private Transform[] firePoints;

        [SerializeField] private GameObject projectilePrefab;

        // Config courante (appliquée par TowerBase.ApplyLevel)
        private float fireRate;
        private int   projectileCount;
        private float projectileSpeed;
        private float damage;

        private float fireCooldown;
        private TowerBase towerBase;

        private void Awake()  => towerBase = GetComponent<TowerBase>();

        private void Update()
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown > 0f) return;

            Transform target = towerBase.GetCurrentTarget();
            if (target == null) return;

            Fire(target);
            fireCooldown = 1f / fireRate;
        }
        
        public void Configure(TowerLevel cfg)
        {
            fireRate       = cfg.fireRate;
            projectileCount = cfg.projectileCount;
            projectileSpeed = cfg.projectileSpeed;
            damage          = cfg.damage;
            fireCooldown    = 0f; 
        }

        private void Fire(Transform target)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Transform fp = firePoints[i % firePoints.Length];
                GameObject go = Instantiate(projectilePrefab, fp.position, fp.rotation);

                if (go.TryGetComponent<Projectile>(out var proj))
                    proj.Initialize(target, projectileSpeed, damage);
            }
        }
    }
}
