using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamev3Manager : MonoBehaviour
{
    public int width = 25;
    public int height = 15;
    public int dirtcount = 0;
    public bool endEpisode = false;
    //2d array repersenting the map
    public int[,] map;
    public GameObject[,] tileMap;
    public GameObject[,] dirtMap;
    public GameObject tilePrefab;
    public GameObject dirtPrefab;
    public GameObject agentPrefab;
    public int startx, startz;

    
    private void Awake() {  //initilize map & tilemap before start
        map = new int[width,height];
        tileMap = new GameObject[width,height];
        dirtMap = new GameObject[width,height];
        startx = (int)transform.position.x;
        startz = (int)transform.position.z;
        allTileInit();
    }

    public void allTileInit(){ //turn map to all tile
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                map[i,j] = 0;
                tileMap[i,j] = Instantiate(tilePrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                dirtMap[i,j] = Instantiate(dirtPrefab, transform.position + new Vector3(i,-5,j), Quaternion.identity);
            }
        }
    }

    public int placeDirt(int x, int z){ //place dirt on specific location, used by agent

        if (x < 0 || x >= width || z < 0 || z >= height) return 0;
        if (map[x,z] == 0){
            map[x,z] = 1;
            tileMap[x,z].transform.position = transform.position + new Vector3(x, -5, z);
            dirtMap[x,z].transform.position = transform.position + new Vector3(x, 1, z);
            dirtcount++;
            return 1;
        }
        return 0;
    }
    

    public void resetGrid(){ //reset grid
        //destory all tiles
        dirtcount = 0;
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                tileMap[i,j].transform.position = transform.position + new Vector3(i,1,j);
                dirtMap[i,j].transform.position = transform.position + new Vector3(i,-5,j);
                map[i,j] = 0;
            }
        }

        
    }

    
    
}
