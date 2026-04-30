using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TowerDefense;

public class EnemyPathfindingTests
{
    private GameObject enemyGo;
    private PathFollower follower;
    private EnemyData testData;

    [SetUp]
    public void SetUp()
    {

        enemyGo = new GameObject("EnemyPathTester");
        follower = enemyGo.AddComponent<PathFollower>();


        testData = ScriptableObject.CreateInstance<EnemyData>();
        testData.moveSpeed = 5f; 
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyGo);
        Object.DestroyImmediate(testData);
    }

    [UnityTest]
    public IEnumerator EnemyFollowsPathway_MovesTowardsNextPoint()
    {
       
        Vector3 startPos = PathDefinition.Instance.StartPoint;
        enemyGo.transform.position = startPos;

        follower.Initialize(testData.moveSpeed);

    
        Vector3 initialPos = enemyGo.transform.position;


        yield return new WaitForSeconds(0.1f);


        Assert.AreNotEqual(initialPos, enemyGo.transform.position, "L'ennemi ne s'est pas déplacé du tout.");


        float distanceTravelled = Vector3.Distance(startPos, enemyGo.transform.position);
        Assert.Greater(distanceTravelled, 0.1f, "L'ennemi ne se déplace pas assez vite ou dans la mauvaise direction.");
    }
}