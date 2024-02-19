using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newApproachGridManager : MonoBehaviour
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

    private void Awake() {
        //summon Agent
        //Instantiate(agentPrefab, transform.position + new Vector3(0,2,0), Quaternion.identity);
        allTileInit();
        //dirtInit();
    }
    //initilize with all tile with local position
    public void allTileInit(){
        map = new int[width,height];
        tileMap = new GameObject[width,height];
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                //99% chance of dirt
                if(Random.Range(0,100) > 101){
                    map[i,j] = 1;
                    tileMap[i,j] = Instantiate(dirtPrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                }
                else{
                    map[i,j] = 0;
                    tileMap[i,j] = Instantiate(tilePrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                }

            }
        }
    }
    void Update(){
        dirtcount = 0;
        //loop through map and update tileMap
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                if(map[i,j] == 0){
                    if (tileMap[i,j].tag == "Dirt"){
                        Destroy(tileMap[i,j]);
                        tileMap[i,j] = Instantiate(tilePrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                    }
                }
                else if(map[i,j] == 1){
                    if (tileMap[i,j].tag == "Grass"){
                        Destroy(tileMap[i,j]);
                        tileMap[i,j] = Instantiate(dirtPrefab, transform.position + new Vector3(i,1,j), Quaternion.identity);
                    }
                    dirtcount++;
                    
                }
            }
        }
        if (dirtcount > (height*width)/6 || endEpisode == true){  // if change percentage is over 25% end episode
            endEpisode = true;
            //resetGrid();
        }

    }
    public void resetGrid(){
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                if(Random.Range(0,100) > 101){
                    map[i,j] = 1;
                }
                else{
                    map[i,j] = 0;
                }
            }
        }
        
    }
    
    
}
