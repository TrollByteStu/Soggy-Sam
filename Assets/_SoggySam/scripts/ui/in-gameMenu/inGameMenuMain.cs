using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class inGameMenuMain : MonoBehaviour
{

    public GameObject mainPanel;
    public bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        mainPanel.SetActive(false);
    }

    void OnEscape()
    {
        if ( paused)
        { // already paused, so resume gameplay
            mainPanel.SetActive(false);
            Time.timeScale = 1;
            paused = false;
        } else { // not paused, so we pause
            mainPanel.SetActive(true);
            Time.timeScale = 0;
            paused = true;
        }
    }

    public void inGameMenu_backToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void inGameMenu_quit()
    {
        Application.Quit();
    }
}
