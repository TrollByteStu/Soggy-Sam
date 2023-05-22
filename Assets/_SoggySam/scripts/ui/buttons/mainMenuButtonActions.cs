using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuButtonActions : MonoBehaviour
{
    public void mainMenu_Button_Play()
    {
        SceneManager.LoadScene(1);
    }

    public void mainMenu_Button_Quit()
    {
        Application.Quit();
    }
}
