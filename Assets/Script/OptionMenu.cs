using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu: MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    private bool isPaused;
    
    void Start()
    {
        isPaused = true;
        ShowPauseMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPauseMenu();
        }
    }

    public void ShowPauseMenu()
    {
        if (isPaused == true)
        {
            Time.timeScale = 1;
            PauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            PauseMenu.SetActive(true);
        }
        isPaused = !isPaused;
    }
}
