using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class CurrencyManager : MonoBehaviour
{

    public static int startingCurrency = 1000;
    public static int health = 100;
    public Text currencyText;
    public Text healthText;
    
    // Start is called before the first frame update
    public void reset(){
        startingCurrency = 1000;
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        currencyText.text = "$" + startingCurrency.ToString();
        healthText.text = "HP:" + health.ToString();
        if(health <= 0){
            reset();
            SceneManager.LoadScene(4);

        }
    }
}
