using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CurrencyManager : MonoBehaviour
{

    public static int startingCurrency = 1000;
    public static int health = 100;
    public Text currencyText;
    public Text healthText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currencyText.text = "$" + startingCurrency.ToString();
        healthText.text = "HP:" + health.ToString();
    }
}
