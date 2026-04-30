using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TowerDefense; 

public class TowerShootingTests
{
    private GameObject enemyGo;
    private EnemyHealth enemyHealth;

    private GameObject towerGo;
    private TowerBase towerBase;
    private TowerAttack towerAttack;

    private GameObject projPrefab;

    [SetUp]
    public void SetUp()
    {
        // ==========================================
        // 1. Création de la cible (Ennemi)
        // ==========================================
        enemyGo = new GameObject("EnemyDummy");
        enemyGo.transform.position = new Vector3(0, 0, 2f); 
        enemyGo.tag = "Enemy";

        enemyHealth = enemyGo.AddComponent<EnemyHealth>();

        // Simulation des données ennemies
        var enemyData = ScriptableObject.CreateInstance<EnemyData>();
        enemyData.maxHealth = 100f;
        enemyHealth.Initialize(enemyData);

        enemyGo.AddComponent<PathFollower>();

        // ==========================================
        // 2. Création du Projectile (Prefab)
        // ==========================================
        projPrefab = new GameObject("ProjectilePrefab");
        projPrefab.AddComponent<Projectile>();
        projPrefab.SetActive(false);

        // ==========================================
        // 3. Création de la Tour
        // ==========================================
        towerGo = new GameObject("TowerDummy");
        towerGo.transform.position = Vector3.zero;

        towerBase = towerGo.AddComponent<TowerBase>();
        towerAttack = towerGo.GetComponent<TowerAttack>();

      
        var towerData = ScriptableObject.CreateInstance<TowerData>();
        towerData.levels = new TowerLevel[1];
        towerData.levels[0] = new TowerLevel
        {
            range = 5f,
            fireRate = 10f,        
            projectileCount = 1,
            projectileSpeed = 50f, 
            damage = 25f           
        };
        towerBase.data = towerData;

        // ==========================================
        // 4. Injection des références privées
        // ==========================================
    
        var rotPoint = new GameObject("RotPoint").transform;
        rotPoint.parent = towerGo.transform;
        typeof(TowerBase).GetField("rotationPoint", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(towerBase, rotPoint);

        var firePoint = new GameObject("FirePoint").transform;
        firePoint.parent = towerGo.transform;
        typeof(TowerAttack).GetField("firePoints", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(towerAttack, new Transform[] { firePoint });

        typeof(TowerAttack).GetField("projectilePrefab", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(towerAttack, projPrefab);
    }

    [TearDown]
    public void TearDown()
    {
        // Nettoyage après le test
        Object.DestroyImmediate(enemyGo);
        Object.DestroyImmediate(towerGo);
        Object.DestroyImmediate(projPrefab);
    }

    [UnityTest]
    public IEnumerator TowerShootsAndDamagesEnemy_Successfully()
    {
        Assert.AreEqual(1f, enemyHealth.HealthNormalized, "L'ennemi devrait avoir 100% de sa vie au début du test.");

        yield return new WaitForSeconds(0.2f);

        Assert.Less(enemyHealth.HealthNormalized, 1f, "L'ennemi n'a subi aucun dégât, la tour n'a pas tiré ou le projectile a raté.");
        Assert.AreEqual(0.75f, enemyHealth.HealthNormalized, 0.01f, "Les dégâts infligés ne correspondent pas à la statistique 'damage' configurée sur la tour.");
    }
}