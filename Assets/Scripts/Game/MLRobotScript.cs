using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class MLRobotScript : Agent
{
    private Vector3 startPosition, startRotation;

    private GridDisplayer gridDisplayer;
    private int length, width;


    private int currentx, currentz;
    private int lastx, lastz;
    public int episodeCount = 0;
    private int badMoveCount = 0;
    private int lastAction;

    public void Awake(){
        gridDisplayer = FindObjectOfType<GridDisplayer>();
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        length = gridDisplayer.length;
        width = gridDisplayer.width;
   
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.z);
        sensor.AddObservation(length - transform.position.x);
        sensor.AddObservation(width - transform.position.z);
        sensor.AddObservation(gridDisplayer.distanceToGoalX(transform.position.x));
        sensor.AddObservation(gridDisplayer.distanceToGoalZ(transform.position.z));
    }
    

    public override void OnActionReceived(ActionBuffers vectorAction){

        int action = vectorAction.DiscreteActions[0];
        if (action == lastAction){
            AddReward(20f);
        }
        switch (action){
            case 0:
                transform.position += new Vector3 (0, 0, 1)/5;
                lastAction = 0;
                resetIfFallen();
                break;
            case 1:
                transform.position += new Vector3 (1, 0, 0)/5;
                lastAction = 1;
                resetIfFallen();
                break;
            case 2:
                transform.position += new Vector3 (0, 0, -1)/5;
                lastAction = 2;
                resetIfFallen();
                break;
            case 3:
                transform.position += new Vector3 (-1, 0, 0)/5;
                lastAction = 3;
                resetIfFallen();
                break;
            case 4:
                break;
        }
    }
    public void FixedUpdate(){
        //round to nearest int
        lastx = currentx;
        lastz = currentz;
        currentx = (int)Mathf.Round(transform.position.x);
        currentz = (int)Mathf.Round(transform.position.z);

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 4; // Default action is no movement

        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[0] = 0; // Move forward
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[0] = 1; // Move right
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActions[0] = 2; // Move backward
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActions[0] = 3; // Move left
        }
    }
   
    public void reset(){

        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        gridDisplayer.reset(episodeCount);
        episodeCount++;
        
    }
    public void resetIfFallen(){

        bool f = gridDisplayer.updateMap(currentx, currentz);
        if (episodeCount == 0){
            reset();
        }
        

        if (f == true){
            AddReward(50f);
            badMoveCount = 0;
        }
        else{
            badMoveCount++;
        }
        if (badMoveCount > 5){
            AddReward(-1000f);
            stopMoving();
        }

        //add reward for moving closer to goal
        if (gridDisplayer.distanceToGoal(transform.position.x, transform.position.z) < gridDisplayer.distanceToGoal(lastx, lastz)){
            AddReward(40f);
        }
        
    
        

        if (gridDisplayer.distanceToGoal(transform.position.x, transform.position.z) < 0.4) {
            Debug.Log("hit goal");
            AddReward(5000f);
            stopMoving();
               
        }
        else
        if (transform.position.z < 0){
            stopMoving();
        }
        else if (transform.position.z > width-0.5){
            stopMoving();
        }
        else if (transform.position.x < 0){
            stopMoving();
        }
        else if (transform.position.x > length-0.5){
            stopMoving();
        }
    }
    private void stopMoving(){
        gridDisplayer.DeactivateRobot(); //removed for TRAINING
        Debug.Log("episode count: " + episodeCount);
        gridDisplayer.layObjects();
        //gridDisplayer.reset(episodeCount); //remove for NOT TRAINING
        AddReward(-1000f);
        //Debug.Log(GetCumulativeReward());
        EndEpisode();

    }





    // a: left, b: forward, c: right. Detects blocks from the edge



}


