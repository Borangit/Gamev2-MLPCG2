using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
public class Gamev3Agent : Agent
{
    public int width = 25;
    public int height = 15;
    public Gamev3Manager gridManager;
    private int neighbourDirtCount;
    public int spawnx, spawnz, goalx, goalz;
    public GameObject spawnPrefab;
    public GameObject BasePrefab;
    private int blockedMove, blockedMoveCount;

    private int mapx, mapz;
    private void Awake() {

        spawnPrefab = Instantiate(spawnPrefab, new Vector3(0, -5, 0), Quaternion.identity);
        BasePrefab = Instantiate(BasePrefab, new Vector3(24, -5, 14), Quaternion.identity);
        print("Agent Awake");

    }
    public override void OnActionReceived(ActionBuffers actions){
        int action = actions.DiscreteActions[0];
        if (blockedMoveCount > 10){
            EndEpisode();
        }
        
        switch (action){

            case 0:
                if (shouldBlockMove(0)){
                    blockedMoveCount++;
                    break;
                }
                transform.position += new Vector3 (1, 0, 0);
                blockedMove = 1;
                movedAgent();
                break;
            case 1:
                if (shouldBlockMove(1)){
                    blockedMoveCount++;
                    break;
                }
                transform.position += new Vector3 (-1, 0, 0);
                blockedMove = 0;
                movedAgent();
                break;
            case 2:
                if (shouldBlockMove(2)){
                    blockedMoveCount++;
                    break;
                }
                transform.position += new Vector3 (0, 0, 1);
                blockedMove = 3;
                movedAgent();
                break;
            case 3:
                if (shouldBlockMove(3)){
                    blockedMoveCount++;
                    break;
                }
                transform.position += new Vector3 (0, 0, -1);
                blockedMove = 2;
                movedAgent();
                break;
            // case 4:
            //     BasePrefab.transform.position = new Vector3((int)transform.position.x, 1, (int)transform.position.z);
            //     AddReward(1 * gridManager.dirtcount);

            //     EndEpisode();
            //     break;
 
        }

        

    }
    public void movedAgent(){
        if (transform.position.x < gridManager.startx || transform.position.x > width-1 + gridManager.startx || transform.position.z < gridManager.startz || transform.position.z > height-1 + gridManager.startz){

            EndEpisode();
        }
        mapx = (int)(transform.position.x - gridManager.startx);
        mapz = (int)(transform.position.z - gridManager.startz);
        gridManager.placeDirt(mapx, mapz);
        if (mapx == goalx && mapz == goalz){
            AddReward(10f);
            AddReward(0.1f * gridManager.dirtcount);
            print(GetCumulativeReward());
            EndEpisode();
        }

        if(getNeighbourDirtCount(mapx, mapz) > 1){

            EndEpisode();
        }
        // else{
        //     AddReward(1f * gridManager.dirtcount);
        // }
    }
    private bool shouldBlockMove(int currentMove){// blocks move if the agent tries to undo last move#
        return false; //comment this line to enable the block
        // switch (currentMove){
        //     case 0:
        //         mapx = (int)(transform.position.x) + 1;
        //         mapz = (int)(transform.position.z);
        //         if (getNeighbourDirtCount(mapx, mapz) > 1){
        //             AddReward(-1f);
        //             return true;
        //         }
        //         break;
        //     case 1:
        //         mapx = (int)(transform.position.x) - 1;
        //         mapz = (int)(transform.position.z);
        //         if (getNeighbourDirtCount(mapx, mapz) > 1){
        //             AddReward(-1f);
        //             return true;
        //         }
        //         break;
        //     case 2:
        //         mapx = (int)(transform.position.x);
        //         mapz = (int)(transform.position.z) + 1;
        //         if (getNeighbourDirtCount(mapx, mapz) > 1){
        //             AddReward(-1f);
        //             return true;
        //         }
        //         break;
        //     case 3:
        //         mapx = (int)(transform.position.x);
        //         mapz = (int)(transform.position.z) - 1;
        //         if (getNeighbourDirtCount(mapx, mapz) > 1){
        //             AddReward(-1f);
        //             return true;
        //         }
        //         break;
        // }
        // if (currentMove == blockedMove){
        //     AddReward(-1f);
        //     return true;
        // }
        // return false;
    }
    public override void Heuristic(in ActionBuffers actionsOut){
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = -1;

        if (Input.GetKey(KeyCode.D)){
            discreteActionsOut[0] = 0;
        }
        if (Input.GetKey(KeyCode.A)){
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.W)){
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.S)){
            discreteActionsOut[0] = 3;
        }
        // if (Input.GetKey(KeyCode.Space)){
        //     discreteActionsOut[0] = 4;
        // }

    }
    public override void OnEpisodeBegin(){
        
        blockedMoveCount = 0;
        
        goalx = Random.Range(0,width) + gridManager.startx;
        goalz = Random.Range(0,height) + gridManager.startz;
        spawnx = Random.Range(0,width) + gridManager.startx;
        spawnz = Random.Range(0,height) + gridManager.startz;
        // spawnx = 12 + gridManager.startx;
        // spawnz = 7 + gridManager.startz;

        BasePrefab.transform.position = new Vector3(goalx, 1, goalz);

        
        transform.position = new Vector3(spawnx, 1, spawnz) ;
        gridManager.resetGrid();
        spawnPrefab.transform.position = new Vector3(spawnx, 1, spawnz);


    }


    private int getNeighbourDirtCount(int i, int j){

        if (i < 0){
            i = 0;
        }
        if (i >= width){
            i = width-1;
        }
        if (j < 0){
            j = 0;
        }
        if (j >= height){
            j = height-1;
        }
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

}
