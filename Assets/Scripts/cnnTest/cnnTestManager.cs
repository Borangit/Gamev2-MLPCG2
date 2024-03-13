using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cnnTestManager : MonoBehaviour
{
    public int width = 25;
    public int height = 15;
    public int dirtcount = 0;
    public bool endEpisode = false;
    //2d array repersenting the map
    public int[,] map;
    public GameObject[,] tileMap;
    public GameObject tilePrefab;
    public GameObject dirtPrefab;
    public GameObject agentPrefab;
    public int state;

    private void Awake() {
        //summon Agent
        //Instantiate(agentPrefab, transform.position + new Vector3(0,2,0), Quaternion.identity);

        //dirtInit();
        map = new int[width,height];
        tileMap = new GameObject[width,height];
    }
    //initilize with all tile with local position
    public void allTileInit(){

        state = Random.Range(0,2);

        for(int i = 0; i < 12; i++){
            for(int j = 0; j < height; j++){
                //99% chance of dirt
                if(state == 1){
                    map[i,j] = 1;
                    tileMap[i,j] = Instantiate(dirtPrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                }
                else{
                    map[i,j] = 0;
                    tileMap[i,j] = Instantiate(tilePrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                }

            }
        }
        for (int i = 12; i < width; i++){
            for(int j = 0; j < height; j++){
                if (state == 1){
                    map[i,j] = 0;
                    tileMap[i,j] = Instantiate(tilePrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                }
                else{
                    map[i,j] = 1;
                    tileMap[i,j] = Instantiate(dirtPrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                }
            }
        }
    }

    public void resetGrid(){
        //destory all tiles
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                Destroy(tileMap[i,j]);
            }
        }

        allTileInit();
    }
    public int getState(){
        return state;
    }
    
    
}
