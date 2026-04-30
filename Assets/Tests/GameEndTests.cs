using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TowerDefense;

public class GameEndTests
{
    private GameObject gameManagerGo;
    private GameManager gameManager;
    private StubWaveManager waveManager; 

    public class StubWaveManager : MonoBehaviour
    {
        public bool moreWaves = false;
        public bool HasMoreWaves() => moreWaves;
        public void StartNextWave() { }
    }

    [SetUp]
    public void SetUp()
    {
        gameManagerGo = new GameObject("GameManager");
        gameManager = gameManagerGo.AddComponent<GameManager>();

    
        waveManager = gameManagerGo.AddComponent<StubWaveManager>();

       
        typeof(GameManager)
            .GetField("waveManager", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(gameManager, waveManager);

        gameManagerGo.AddComponent<EconomyManager>();

        gameManagerGo.AddComponent<PlacementManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameManagerGo);
    }

    [Test]
    public void GameState_TransitionsToVictory_WhenNoMoreWaves()
    {
    
        waveManager.moreWaves = false;

        gameManager.OnWaveFinished();

        Assert.AreEqual(GameState.Victory, gameManager.CurrentState,
            "Le jeu devrait être en état de Victoire quand il n'y a plus de vagues.");
    }

    [Test]
    public void GameState_TransitionsToGameOver_Directly()
    {
        
        gameManager.ChangeState(GameState.GameOver);

        Assert.AreEqual(GameState.GameOver, gameManager.CurrentState,
            "Le GameManager n'a pas correctement basculé sur l'état GameOver.");
    }

    [UnityTest]
    public IEnumerator GameEvents_RaiseEvent_OnStateChange()
    {
        bool eventRaised = false;
        GameState lastState = GameState.MainMenu;

        GameEvents.OnGameStateChanged += (state) => {
            eventRaised = true;
            lastState = state;
        }; 

        gameManager.ChangeState(GameState.Victory);
        yield return null;

        Assert.IsTrue(eventRaised, "L'événement OnGameStateChanged n'a pas été soulevé.");
        Assert.AreEqual(GameState.Victory, lastState, "L'événement n'a pas transmis le bon état.");
    }
}