using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainPanel;
    [SerializeField]  private GameObject CreditsPanel;
    [SerializeField]  private GameObject SongPanel;

    void Start()
    {
        MainPanel.SetActive(true);
        CreditsPanel.SetActive(false);
        SongPanel.SetActive(false);
    }
}
