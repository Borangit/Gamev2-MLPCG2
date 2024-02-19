using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance;

    public GameObject standardTurretPrefab;



    void Awake(){
        if (instance != null){
            Debug.LogError("More than one BuildManager in scene!");
            return;
        }
        instance = this;
    }

    



    

    public GameObject GetTurretToBuild(){
        return standardTurretPrefab;
    }
}
