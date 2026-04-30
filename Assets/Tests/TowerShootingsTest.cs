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
    private TowerBase   towerBase;
    private TowerAttack towerAttack;

    private GameObject projPrefab;

    [SetUp]
    public void SetUp()
    {
        enemyGo = new GameObject("EnemyDummy");
        enemyGo.transform.position = new Vector3(0f, 0f, 2f);
        enemyGo.tag = "Enemy";
        var col = enemyGo.AddComponent<SphereCollider>();
        col.isTrigger = false;
        col.radius    = 0.5f;

        enemyHealth = enemyGo.AddComponent<EnemyHealth>();
        var enemyData = ScriptableObject.CreateInstance<EnemyData>();
        enemyData.maxHealth = 100f;
        enemyData.reward    = 0;
        enemyData.scoreValue = 0;
        enemyHealth.Initialize(enemyData);
        
        projPrefab = new GameObject("ProjectilePrefab");
        var projCollider = projPrefab.AddComponent<SphereCollider>();
        projCollider.isTrigger = true;
        projCollider.radius    = 0.1f;
        var rb = projPrefab.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        projPrefab.AddComponent<Projectile>();
        projPrefab.SetActive(false);
        
        towerGo = new GameObject("TowerDummy");
        towerGo.transform.position = Vector3.zero;

        var towerData = ScriptableObject.CreateInstance<TowerData>();
        towerData.levels    = new TowerLevel[1];
        towerData.levels[0] = new TowerLevel
        {
            range           = 10f,
            fireRate        = 0.1f,
            projectileCount = 1,
            projectileSpeed = 50f,
            damage          = 25f
        };

        towerAttack = towerGo.AddComponent<TowerAttack>();
        var firePoint = new GameObject("FirePoint").transform;
        firePoint.SetParent(towerGo.transform);
        SetPrivate(towerAttack, "firePoints", new Transform[] { firePoint });
        SetPrivate(towerAttack, "projectilePrefab", projPrefab);

        towerBase      = towerGo.AddComponent<TowerBase>();
        towerBase.data = towerData;
        towerAttack.Configure(towerData.levels[0], true);
        var rotPoint = new GameObject("RotPoint").transform;
        rotPoint.SetParent(towerGo.transform);
        SetPrivate(towerBase, "rotationPoint", rotPoint);
        SetPrivate(towerBase, "currentTarget", enemyGo.transform);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyGo);
        Object.DestroyImmediate(towerGo);
        Object.DestroyImmediate(projPrefab);

        foreach (var p in Object.FindObjectsOfType<Projectile>())
            Object.DestroyImmediate(p.gameObject);
    }

    [UnityTest]
    public IEnumerator TowerShootsAndDamagesEnemy_Successfully()
    {
        projPrefab.SetActive(true); 
        towerBase.enabled = false; 
        towerBase.SetTargetForTest(enemyGo.transform);

        Assert.AreEqual(1f, enemyHealth.HealthNormalized, 0.001f);

        yield return new WaitForSeconds(0.2f);

        Assert.Less(enemyHealth.HealthNormalized, 1f, "L'ennemi n'a subi aucun dégât.");
    }
    
    [UnityTest]
    public IEnumerator TowerDoesNotShoot_WhenNoTarget()
    {
        towerAttack.enabled = false;
        towerBase.SetTargetForTest(null); 
        towerAttack.enabled = true;

        yield return new WaitForSeconds(0.3f);

        Assert.AreEqual(1f, enemyHealth.HealthNormalized, 0.001f,
            "La tour a tiré alors qu'elle n'avait plus de cible.");
    }

    private static void SetPrivate(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (field == null)
        {
            Debug.LogError($"[Test] Champ privé '{fieldName}' introuvable sur {obj.GetType().Name}");
            return;
        }

        field.SetValue(obj, value);
    }
}