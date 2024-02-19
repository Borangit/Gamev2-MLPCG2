using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playOptions : MonoBehaviour
{
    public void PlayClassic()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayAI()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void QuitGame()
    {
        // Quits the game
        Debug.Log("Quit");
        Application.Quit();
    }

}
