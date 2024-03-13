using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        // Quits the game
        Debug.Log("Quit");
        Application.Quit();
    }
    public void TitleScreen()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(0);
    }

}
