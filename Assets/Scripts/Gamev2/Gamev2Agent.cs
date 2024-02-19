using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Gamev2Agent : Agent
{
    public Gamev2Manager gridManager;
    public int length,width;
    public int goalx, goalz;


    void Awake(){
        
        gridManager = FindObjectOfType<Gamev2Manager>();
        length = 25;
        width = 15;
        
    }
    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.z);
        sensor.AddObservation(length-1 - transform.position.x);
        sensor.AddObservation(width-1 - transform.position.z);
        sensor.AddObservation(gridManager.distanceToGoal(transform.position.x, transform.position.z));
    }
    public override void OnEpisodeBegin(){
        transform.position = new Vector3(0, 1, gridManager.robotz);
    }

    public override void OnActionReceived(ActionBuffers actions){
        int action = actions.DiscreteActions[0];
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
        }
        if (gridManager.distanceToGoal(transform.position.x, transform.position.z) < 0.4) {
            goalx = gridManager.goalx;
            goalz = gridManager.goalz;

            gridManager.updateMap(goalx, goalz);
            Debug.Log("hit goal");
            AddReward(100f);
            //gridManager.reset();//remove for RUNNING
            gridManager.DeactivateRobot();//remove for TRAINING
            gridManager.layObjects();
            EndEpisode();   
        }

        if(gridManager.updateMap((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z)) == true){
            AddReward(5f);
        }
        if(gridManager.endEpisode == true){
            AddReward(-50f);
            gridManager.reset();
            EndEpisode();
        }
        killWhenOutOfBounds();
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
    }

    public void killWhenOutOfBounds(){
        if (transform.position.x < 0 || transform.position.x > length-1 || transform.position.z < 0 || transform.position.z > width-1){
            AddReward(-100f);
            gridManager.reset();
            EndEpisode();
            
        }
    }

}
