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
        // 1. Initialiser le gestionnaire d'économie
        economyGo = new GameObject("EconomyManager");
        economyManager = economyGo.AddComponent<EconomyManager>();
        economyManager.Initialize(200); // On donne 200 de monnaie initiale au joueur

        // 2. Initialiser le slot de placement
        slotGo = new GameObject("TowerSlot");
        towerSlot = slotGo.AddComponent<TowerSlot>();

        // 3. Créer une configuration de tour factice pour le test
        testTowerData = ScriptableObject.CreateInstance<TowerData>();
        testTowerData.purchaseCost = 150;
    }

    [TearDown]
    public void TearDown()
    {
        // Nettoyage des objets pour ne pas polluer les autres tests
        Object.DestroyImmediate(economyGo);
        Object.DestroyImmediate(slotGo);
        Object.DestroyImmediate(testTowerData);
    }

    [Test]
    public void BuyTowerSimplePasses()
    {
        // Act : Le joueur tente de dépenser le coût de la tour
        bool canBuy = economyManager.TrySpend(testTowerData.purchaseCost);

        // Assert : On vérifie que la transaction réussit et que le solde est correct
        Assert.IsTrue(canBuy, "L'achat aurait dû être autorisé avec suffisamment de fonds.");
        Assert.AreEqual(50, economyManager.Currency, "La somme déduite n'est pas correcte (200 - 150 = 50).");
    }

    [UnityTest]
    public IEnumerator BuyTowerWithEnumeratorPasses()
    {
        // Arrange : On s'assure que la dépense initiale est valide
        bool canBuy = economyManager.TrySpend(testTowerData.purchaseCost);
        Assert.IsTrue(canBuy);

        // Act : On lance la construction sur le slot
        towerSlot.PlaceTower(testTowerData);

        // On attend une frame pour permettre à la coroutine PlaceTowerAsync de s'exécuter
        yield return null;

        // Assert : On vérifie que le slot s'est bien bloqué en réponse au placement
        Assert.IsFalse(towerSlot.IsAvailable, "Le slot devrait être marqué comme indisponible pendant et après la construction.");
    }
}