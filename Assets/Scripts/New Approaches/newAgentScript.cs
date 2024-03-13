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
    private int actionNumber, tileCount, fallOffMapCount, connectedComponentCount, LSCount;
    private int mapIndexx, mapIndexz, randomx, randomz;



    // private int lastCCN = 0;
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
    public void setActive(){
        gameObject.SetActive(true);
        EndEpisode();
    }
    public override void OnEpisodeBegin(){
        //spawn agent at random position
        randomx = Random.Range(0, width);
        randomz = Random.Range(0, height);
        transform.position = new Vector3(startx + randomx, 1, startz + randomz);
        actionNumber = 0;
        fallOffMapCount = 0;
        tileCount = 0;
        connectedComponentCount = connectedComponent();
        LSCount = FindLongestShortestPath(this, gridManager.map);

    }
    public override void OnActionReceived(ActionBuffers actions){
        int action = actions.DiscreteActions[0];
        actionNumber++;
        if(actionNumber > 1500){
            AddReward(-10f);
            gridManager.endEpisode = true;


        }
        if(tileCount > 80){
            AddReward(1f);
            gridManager.endEpisode = true;
        }
        if (connectedComponent() < connectedComponentCount){
            connectedComponentCount = connectedComponent();
            AddReward(1f);
        }



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
                    tileCount++;
                    AddReward(0.1f);
                }
 


                break;
        }
        if (gridManager.endEpisode == true){// if change percentage is over 25% end episode
            evaluateMap();
            Debug.Log(" action: " + actionNumber + " reward: " + GetCumulativeReward());
            // EndEpisode();
            gameObject.SetActive(false);

            gridManager.endEpisode = false;

            // gridManager.resetGrid();
        }
    }
    public static int FindLongestShortestPath(newAgentScript agent, int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        int longestShortestPath = 0;



        for (int startRow = 0; startRow < rows; startRow++)
        {
            for (int startCol = 0; startCol < cols; startCol++)
            {
                if (map[startRow, startCol] == 1) // Start from an empty tile
                {
                    for (int endRow = 0; endRow < rows; endRow++)
                    {
                        for (int endCol = 0; endCol < cols; endCol++)
                        {
                            if (map[endRow, endCol] == 1 && (startRow != endRow || startCol != endCol)) // End at a different empty tile
                            {

                                int pathLength = BreadthFirstSearch(agent, map, startRow, startCol, endRow, endCol);
                                if (pathLength > longestShortestPath)
                                {
                                    longestShortestPath = pathLength;



                                    

                                }
                            }
                        }
                    }
                }
            }
        }

        return longestShortestPath;
    }
    private static int BreadthFirstSearch(newAgentScript agent, int[,] map, int startRow, int startCol, int endRow, int endCol)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        bool[,] visited = new bool[rows, cols];
        Queue<(int row, int col, int distance)> queue = new Queue<(int, int, int)>();
        queue.Enqueue((startRow, startCol, 0));
        visited[startRow, startCol] = true;

        int[] dr = { -1, 1, 0, 0 }; // Directions: up, down, left, right
        int[] dc = { 0, 0, -1, 1 };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.row == endRow && current.col == endCol)
            {
                return current.distance; // Found the shortest path
            }

            for (int i = 0; i < 4; i++) // Explore all 4 directions
            {
                int newRow = current.row + dr[i];
                int newCol = current.col + dc[i];

                // Check if it's within bounds, not visited, and not a solid tile
                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && !visited[newRow, newCol] && map[newRow, newCol] == 1)
                {
                    visited[newRow, newCol] = true;
                    queue.Enqueue((newRow, newCol, current.distance + 1));

                    
                    
                }
            }
        }

        return -1; // Path not found
    }
    public void evaluateMap(){
        int longestShortestPath = 0;
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
            }
        }

        longestShortestPath = FindLongestShortestPath(this, gridManager.map);
        //spawn spawn and goal prefabs


        if (longestShortestPath > (LSCount + 30) && connectedComponent() == 1)
        {
            AddReward(100f);
            AddReward(connectedComponentCount * 1f);
        }
        


       

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
            AddReward(-0.1f);
            fallOffMapCount++;
        }
    }
    public int connectedComponent(){
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
            }
        }
        return connectedComponentCount;
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
