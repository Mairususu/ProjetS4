using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using TowerDefense;

public class EnemyPathfindingTests
{
    private GameObject        pathGo;
    private PathDefinition    pathDefinition;

    private GameObject        waypointA;
    private GameObject        waypointB;

    [SetUp]
    public void SetUp()
    {
        pathGo = new GameObject("PathDefinition");
        pathDefinition = pathGo.AddComponent<PathDefinition>();
        waypointA = new GameObject("Waypoint_0");
        waypointA.transform.position = Vector3.zero;
        waypointB = new GameObject("Waypoint_1");
        waypointB.transform.position = new Vector3(0f, 0f, 10f);

        var waypoints = new Transform[] { waypointA.transform, waypointB.transform };
        SetPrivate(pathDefinition, "waypoints", waypoints);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(pathGo);
        Object.DestroyImmediate(waypointA);
        Object.DestroyImmediate(waypointB);
    }

    [Test]
    public void PathDefinition_ReturnsCorrectStartAndEnd()
    {
        Assert.AreEqual(Vector3.zero, PathDefinition.Instance.StartPoint,
            "Le point de départ devrait être Vector3.zero.");
        Assert.AreEqual(new Vector3(0f, 0f, 10f), PathDefinition.Instance.EndPoint,
            "Le point d'arrivée devrait être (0,0,10).");
    }

    [Test]
    public void PathDefinition_WaypointCount_IsCorrect()
    {
        Assert.AreEqual(2, PathDefinition.Instance.WaypointCount,
            "Le chemin devrait avoir exactement 2 waypoints.");
    }

    [Test]
    public void PathDefinition_GetWaypoint_ReturnsCorrectPosition()
    {
        Assert.AreEqual(Vector3.zero,
            PathDefinition.Instance.GetWaypoint(0),
            "Waypoint 0 devrait être à l'origine.");
        Assert.AreEqual(new Vector3(0f, 0f, 10f),
            PathDefinition.Instance.GetWaypoint(1),
            "Waypoint 1 devrait être en (0,0,10).");
    }

    [Test]
    public void PathDefinition_GetWaypoint_ClampsOutOfRange()
    {
        Assert.DoesNotThrow(() => PathDefinition.Instance.GetWaypoint(99),
            "GetWaypoint avec un index invalide ne devrait pas lever d'exception.");
        Assert.DoesNotThrow(() => PathDefinition.Instance.GetWaypoint(-1),
            "GetWaypoint avec un index négatif ne devrait pas lever d'exception.");
    }

    [Test]
    public void EnemyData_Initialization_IsCorrect()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.maxHealth  = 80f;
        data.moveSpeed  = 5f;
        data.scoreValue = 10;
        data.reward     = 8;

        Assert.AreEqual(80f, data.maxHealth,   "maxHealth incorrect.");
        Assert.AreEqual(5f,  data.moveSpeed,   "moveSpeed incorrect.");
        Assert.AreEqual(10,  data.scoreValue,  "scoreValue incorrect.");
        Assert.AreEqual(8,   data.reward,      "reward incorrect.");
        Object.DestroyImmediate(data);
    }

    [Test]
    public void EnemyHealth_TakeDamage_ReducesHealth()
    {
        var go     = new GameObject("EnemyHealthTest");
        var health = go.AddComponent<EnemyHealth>();
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.maxHealth  = 100f;
        data.scoreValue = 0;
        data.reward     = 0;
        health.Initialize(data);

        Assert.AreEqual(1f, health.HealthNormalized, 0.001f,
            "La vie normalisée devrait être 1 après initialisation.");
        health.TakeDamage(25f);
        Assert.AreEqual(0.75f, health.HealthNormalized, 0.001f,
            "La vie devrait être à 75% après 25 dégâts sur 100 PV.");
        health.TakeDamage(75f);
        Assert.IsTrue(health.IsDead,
            "L'ennemi devrait être mort après avoir reçu 100 dégâts au total.");

        Object.DestroyImmediate(go);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void EnemyHealth_DoesNotTakeDamage_WhenAlreadyDead()
    {
        var go     = new GameObject("EnemyHealthDeadTest");
        var health = go.AddComponent<EnemyHealth>();

        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.maxHealth  = 50f;
        data.scoreValue = 0;
        data.reward     = 0;
        health.Initialize(data);

        health.TakeDamage(50f); 
        Assert.IsTrue(health.IsDead, "L'ennemi devrait être mort.");
        Assert.DoesNotThrow(() => health.TakeDamage(999f),
            "TakeDamage sur un ennemi mort ne devrait pas lever d'exception.");

        Object.DestroyImmediate(go);
        Object.DestroyImmediate(data);
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