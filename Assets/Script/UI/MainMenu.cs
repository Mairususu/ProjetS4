using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainPanel;
    [SerializeField]  private GameObject SongPanel;

    void Start()
    {
        ShowMainMenu();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/GameScene");
    }

    public void ShowMainMenu()
    {
        MainPanel.SetActive(true);
        SongPanel.SetActive(false);
    }

    public void ShowSong()
    {
        MainPanel.SetActive(false);
        SongPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
