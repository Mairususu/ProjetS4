using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TowerDefense; 

public class BuyTower
{
    private GameObject economyGo;
    private EconomyManager economyManager;

    private GameObject slotGo;
    private TowerSlot towerSlot;

    private TowerData testTowerData;

    [SetUp]
    public void SetUp()
    {
        economyGo = new GameObject("EconomyManager");
        economyManager = economyGo.AddComponent<EconomyManager>();
        if (economyManager != null) {
            economyManager.Initialize(200); 
        } else {
            Assert.Fail("Impossible d'ajouter le composant EconomyManager à l'objet de test.");
        }

        slotGo = new GameObject("TowerSlot");
        towerSlot = slotGo.AddComponent<TowerSlot>();
        testTowerData = ScriptableObject.CreateInstance<TowerData>();
        testTowerData.purchaseCost = 150;
        testTowerData.levels = new TowerLevel[] 
        { 
            new TowerLevel { 
                upgradeCost = 100, 
                range = 5f, 
                damage = 10f,
                fireRate = 1f,
                projectileSpeed = 10f
            } 
        };
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(economyGo);
        Object.DestroyImmediate(slotGo);
        Object.DestroyImmediate(testTowerData);
    }

    [Test]
    public void BuyTowerSimplePasses()
    {
        bool canBuy = economyManager.TrySpend(testTowerData.purchaseCost);
        Assert.IsTrue(canBuy, "L'achat aurait d� �tre autoris� avec suffisamment de fonds.");
        Assert.AreEqual(50, economyManager.Currency, "La somme d�duite n'est pas correcte (200 - 150 = 50).");
    }

    [UnityTest]
    public IEnumerator BuyTowerWithEnumeratorPasses()
    {
        bool canBuy = economyManager.TrySpend(testTowerData.purchaseCost);
        Assert.IsTrue(canBuy);
        towerSlot.PlaceTower(testTowerData);
        yield return new WaitForSeconds(0.1f); 
        Assert.IsFalse(towerSlot.IsAvailable, "Le slot devrait être indisponible après le placement.");
    }
}