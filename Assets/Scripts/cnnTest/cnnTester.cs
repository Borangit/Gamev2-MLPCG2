using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;


public class cnnTester : Agent
{
    public int width = 25;
    public int height = 15;
    private cnnTestManager gridManager;
    private void Awake() {
        gridManager = FindObjectOfType<cnnTestManager>();
    }
      public override void OnActionReceived(ActionBuffers actions){
        int action = actions.DiscreteActions[0];

        switch (action){

            case 0:
                transform.position += new Vector3 (1, 0, 0);
                break;

            case 1:
                transform.position += new Vector3 (-1, 0, 0);
                break;
 
        }

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

    }
    public override void OnEpisodeBegin(){
        transform.position = new Vector3(12, 1, 7);
        gridManager.resetGrid();

    }
    void Update(){

        if (transform.position.z < 0 || transform.position.z > 14){
            AddReward(-1f);
            EndEpisode();

        }
        if (transform.position.x < 0){
            if (gridManager.getState() == 1){
                AddReward(1f);
            }
            print(GetCumulativeReward());
            EndEpisode();
            //print reward
            

        }
        if (transform.position.x > 24){
            if (gridManager.getState() == 0){
                AddReward(1f);
            }
            print(GetCumulativeReward());
            EndEpisode();
            //print reward  

        }
    }
}
