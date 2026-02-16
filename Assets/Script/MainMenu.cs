using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainPanel;
    [SerializeField]  private GameObject CreditsPanel;
    [SerializeField]  private GameObject SongPanel;

    void Start()
    {
        ShowMainMenu();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowMainMenu()
    {
        MainPanel.SetActive(true);
        CreditsPanel.SetActive(false);
        SongPanel.SetActive(false);
    }
    public void ShowCredits()
    {
        MainPanel.SetActive(false);
        CreditsPanel.SetActive(true);
        SongPanel.SetActive(false);
    }

    public void ShowSong()
    {
        MainPanel.SetActive(false);
        SongPanel.SetActive(true);
        CreditsPanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
