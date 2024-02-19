using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnScript : MonoBehaviour
{
    public int width;
    public void reposition()
    {
        Vector3 move = new Vector3(0, 0, Random.Range(0, width));
        transform.position += move;
        //transform.position = new Vector3(0, 0, Random.Range(0, width));
    }
}
