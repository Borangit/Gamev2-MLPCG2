using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playOptions : MonoBehaviour
{
    public void PlayClassic()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(2);
    }
    public void PlayAI()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(3);
    }
    public void PlayAI2()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(7);
    }
    public void PlayShallow(){
        SceneManager.LoadScene(6);
    }
    public void QuitGame()
    {
        // Quits the game
        Debug.Log("Quit");
        Application.Quit();
    }
    public void TitleScreenC()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(0);
    }
    public void TitleScreenAI()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(0);
    }
    public void TitleScreenS()
    {
        // Loads the next scene in the build index
        SceneManager.LoadScene(1);
    }

}
