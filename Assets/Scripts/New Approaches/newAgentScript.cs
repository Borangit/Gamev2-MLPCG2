using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class newAgentScript : Agent
{
    // Start is called before the first frame update
    //run on click w
    public newApproachGridManager gridManager;
    public int width,height,neighbourDirtCount;
    public float startx, startz;
    private int actionNumber, tileCount, fallOffMapCount;
    private int mapIndexx, mapIndexz, randomx, randomz;
    void Awake(){
        
        width = gridManager.width;
        height = gridManager.height;
        startx = transform.position.x;
        startz = transform.position.z;
    }
    void Update(){
        // moveAgent();
        // changeTile();
        killWhenOutOfBounds();
    }
    public override void OnEpisodeBegin(){
        //spawn agent at random position
        randomx = Random.Range(0, width);
        randomz = Random.Range(0, height);
        transform.position = new Vector3(startx + randomx, 1, startz + randomz);
        actionNumber = 0;
        fallOffMapCount = 0;
        tileCount = 0;
    }
    public override void OnActionReceived(ActionBuffers actions){
        int action = actions.DiscreteActions[0];
        actionNumber++;
        // if (actionNumber > 2000){
        //     gridManager.endEpisode = true;
        //     AddReward(-5000f);

        // }
        switch (action){
            case 0:
                transform.position += new Vector3 (0, 0, 1);
                
                break;
            case 1:
                transform.position += new Vector3 (1, 0, 0);
                break;
            case 2:
                transform.position += new Vector3 (0, 0, -1);
                break;
            case 3:
                transform.position += new Vector3 (-1, 0, 0);
                break;
            case 4:
                mapIndexx = (int) (transform.position.x - startx);
                mapIndexz = (int) (transform.position.z - startz);
                if (mapIndexx < 0 || mapIndexx >= width || mapIndexz < 0 || mapIndexz >= height){ // if out of bounds break
                    break;
                }
                
                if (gridManager.map[mapIndexx,mapIndexz] == 0 ){ // change grass tile to dirt tile
                    gridManager.map[mapIndexx,mapIndexz] = 1;
                }




                break;
        }
        if (gridManager.endEpisode == true){// if change percentage is over 25% end episode
            evaluateMap();
            // AddReward(-(actionNumber - 400)*2);//penalize for taking too long, 600 is the max number of actions
            Debug.Log("action: " + actionNumber);
            //print reward
            Debug.Log("Reward: " + GetCumulativeReward());
            Debug.Log("Fall off map count: " + fallOffMapCount);
            EndEpisode();
            gridManager.endEpisode = false;
            gridManager.resetGrid();
        }
    }
    public void evaluateMap(){
        //add a reward if all the components are connected together
        int connectedComponentCount = 0;
        int[,] visited = new int[width,height];
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                visited[i,j] = 0;
            }
        }
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                
                if (gridManager.map[i,j] == 1 && visited[i,j] == 0){
                    connectedComponentCount++;
                    dfs(i,j,visited);
                }            
                if (gridManager.map[i,j] == 1){
                    tileCount++;
                    int neighboutDirtCount = getNeighbourDirtCount(i,j);    
                    if(neighbourDirtCount > 2){
                        AddReward(1f);
                    }
                    if (neighbourDirtCount == 2 ){
                        AddReward(4f);
                    }
                    if (neighbourDirtCount == 1){
                        AddReward(2f);
                    }
                    
                }
            }
        }
        AddReward(500/connectedComponentCount);
        Debug.Log("Connected Component Count: " + connectedComponentCount + " Tile Count: " + tileCount);
        // if(tileCount < 10){
        //     AddReward(-1500f);
        // }
    }
    //dfs for the above function
    private void dfs(int i, int j, int[,] visited){
        if (i < 0 || i >= width || j < 0 || j >= height){
            return;
        }
        if (gridManager.map[i,j] == 0 || visited[i,j] == 1){
            return;
        }
        visited[i,j] = 1;
        dfs(i-1,j,visited);
        dfs(i+1,j,visited);
        dfs(i,j-1,visited);
        dfs(i,j+1,visited);
    }
    private int getNeighbourDirtCount(int i, int j){
        neighbourDirtCount = 0;

        if(i-1 >= 0 && gridManager.map[i-1,j] == 1){
            neighbourDirtCount++;
        }
        if(i+1 < width && gridManager.map[i+1,j] == 1){
            neighbourDirtCount++;
        }
        if(j-1 >= 0 && gridManager.map[i,j-1] == 1){
            neighbourDirtCount++;
        }
        if(j+1 < height && gridManager.map[i,j+1] == 1){
            neighbourDirtCount++;
        }
        return neighbourDirtCount;
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = -1;
        if (Input.GetKey(KeyCode.W)){
            discreteActionsOut[0] = 0;
        }
        if (Input.GetKey(KeyCode.D)){
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S)){
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.A)){
            discreteActionsOut[0] = 3;
        }
        if (Input.GetKey(KeyCode.Space)){
            discreteActionsOut[0] = 4;
        }
    }
    private void killWhenOutOfBounds(){
        if (transform.position.x < startx || transform.position.x > startx + width-1 || transform.position.z < startz || transform.position.z > startz + height-1){
            transform.position = new Vector3(startx + (width/2), 1, startz + (height/2));
            AddReward(-5f);
            fallOffMapCount++;
        }
    }
    //move agent using wasd
    // private void moveAgent(){
        
    //     if (Input.GetKeyDown(KeyCode.W)){
    //         transform.position += new Vector3 (0, 0, 1);
    //         actionNumber++;
    //     }
    //     if (Input.GetKeyDown(KeyCode.A)){
    //         transform.position += new Vector3 (-1, 0, 0);
    //         actionNumber++;
    //     }
    //     if (Input.GetKeyDown(KeyCode.S)){
    //         transform.position += new Vector3 (0, 0, -1);
    //         actionNumber++;
    //     }
    //     if (Input.GetKeyDown(KeyCode.D)){
    //         transform.position += new Vector3 (1, 0, 0);
    //         actionNumber++;
    //     }
    // }
    // private void changeTile(){
    //     if (Input.GetKeyDown(KeyCode.Space)){
    //         //get tile position, convert to map index & convert it to int
    //         mapIndexx = (int) (transform.position.x - startx);
    //         mapIndexz = (int) (transform.position.z - startz);

    //         //change tile
    //         if (gridManager.map[mapIndexx,mapIndexz] == 1){
    //             gridManager.map[mapIndexx,mapIndexz] = 0;
    //         }
    //         else{
    //             gridManager.map[mapIndexx,mapIndexz] = 1;
                
    //         }
    //     }
    //      if (actionNumber > 1000){
    //         gridManager.endEpisode = true;
    //         AddReward(-500f);
    //     }
    //     if (gridManager.endEpisode == true){// if change percentage is over 25% end episode
    //         evaluateMap();
    //         AddReward(-(actionNumber - 600)/2);//penalize for taking too long, 600 is the max number of actions
    //         Debug.Log("action: " + actionNumber);
    //         //print reward
    //         Debug.Log("Reward: " + GetCumulativeReward());
    //         EndEpisode();
    //         gridManager.endEpisode = false;
    //         gridManager.resetGrid();
    //     }
    // }
}
