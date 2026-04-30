using UnityEngine;

namespace TowerDefense
{
    public class TowerAttack : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private Transform[] firePoints;

        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private AudioSource audioSource;
        
        private float fireRate;
        private int   projectileCount;
        private float projectileSpeed;
        private float damage;

        public float fireCooldown;
        private TowerBase towerBase;

        private void Awake()
        {
            towerBase = GetComponent<TowerBase>();
        }

        private void Update()
        {
            // Lazy init si Awake a tourné avant TowerBase
            if (towerBase == null)
                towerBase = GetComponent<TowerBase>();

            if (towerBase == null) return; // still null → sécurité
            if (fireRate <= 0f)   return;

            fireCooldown -= Time.deltaTime;
            if (fireCooldown > 0f) return;

            Transform target = towerBase.GetCurrentTarget();
            if (target == null) return;

            Fire(target);
            fireCooldown = fireRate;
        }
        
        public void Configure(TowerLevel cfg, bool resetCooldown = false)
        {
            fireRate        = cfg.fireRate;
            projectileCount = cfg.projectileCount;
            projectileSpeed = cfg.projectileSpeed;
            damage          = cfg.damage;

            if (resetCooldown)
                fireCooldown = 0f;
        }

        private void Fire(Transform target)
        {
            if (target == null) return; 
            if (firePoints == null || firePoints.Length == 0) return;
            if (projectilePrefab == null) return;

            for (int i = 0; i < projectileCount; i++)
            {
                Transform fp = firePoints[i % firePoints.Length];
                if (fp == null) continue;

                GameObject go = Instantiate(projectilePrefab, fp.position, fp.rotation);
        
                if (go.TryGetComponent<Projectile>(out var proj))
                {
                    proj.Initialize(target, projectileSpeed, damage);
                }
            }
        }
    }
}
