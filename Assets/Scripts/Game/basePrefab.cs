using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseScript : MonoBehaviour
{
    public int width;
    public int health;
    public void reposition()
    {
        Vector3 move = new Vector3(0, 0, Random.Range(0, width));
        transform.position += move;
    }
}
