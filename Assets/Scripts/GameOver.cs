using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public void OnRestartButton()
    {
        SceneManager.LoadScene(2);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

}
