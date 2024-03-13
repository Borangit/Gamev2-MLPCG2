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
    public GameObject roboto, spawno, baseo;
    public List<Vector2Int> pathCells, tempPathCells;

    public GameObject spawnPrefab;
    public GameObject basePrefab;

    public int goalx, goalz, spawnx, spawnz;


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
                if(Random.Range(0,100) > 93){
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
        // if (dirtcount > (height*width)/6 || endEpisode == true){  // if change percentage is over 25% end episode
        //     endEpisode = true;
        //     //resetGrid();
        // }

    }
    public void resetGrid(){
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                if(Random.Range(0,100) > 93){
                    map[i,j] = 1;
                }
                else{
                    map[i,j] = 0;
                }
            }
        }
        //destory all turret
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject turret in turrets)
        {
            Destroy(turret);
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        Destroy(spawno);
        Destroy(baseo);
        roboto.SetActive(true);
        
    }
    public int distanceToGoal(float x, float z)
    {
        return Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(width - x, 2) + Mathf.Pow(height - z, 2)));
    }
    public List<Vector2Int> getPathCells(){
        FindLongestShortestPath(map);
        spawno = Instantiate(spawnPrefab, new Vector3(spawnx, 1f, spawnz), Quaternion.identity);
        baseo = Instantiate(basePrefab, new Vector3(goalx, 1f, goalz), Quaternion.identity);
        setPathCells();
        print(pathCells.Count);
        return pathCells;
    }
    public void setPathCells()
    {
        // Initialize or clear the pathCells list
        pathCells = new List<Vector2Int>();

        // Initialize the visited map to keep track of visited cells
        bool[,] visited = new bool[width, height];
        
        // Initialize the queue for BFS. The queue will store a tuple of the current position and the path taken to reach there.
        Queue<(Vector2Int position, List<Vector2Int> path)> queue = new Queue<(Vector2Int, List<Vector2Int>)>();

        // Start with the spawn position and an empty path
        queue.Enqueue((new Vector2Int(spawnx, spawnz), new List<Vector2Int> { new Vector2Int(spawnx, spawnz) }));
        visited[spawnx, spawnz] = true;

        // Directions to move on the grid: up, down, left, right
        Vector2Int[] directions = {
            new Vector2Int(-1, 0), // up
            new Vector2Int(1, 0),  // down
            new Vector2Int(0, -1), // left
            new Vector2Int(0, 1)   // right
        };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            Vector2Int currentPosition = current.position;
            List<Vector2Int> currentPath = current.path;

            // If the goal is reached, update pathCells and break
            if (currentPosition.x == goalx && currentPosition.y == goalz)
            {
                pathCells = new List<Vector2Int>(currentPath);
                return;
            }

            // Explore neighbors
            foreach (var direction in directions)
            {
                Vector2Int nextPosition = currentPosition + direction;
                // Check bounds and if the cell is walkable (map[i, j] == 1) and not visited
                if (nextPosition.x >= 0 && nextPosition.x < width && nextPosition.y >= 0 && nextPosition.y < height &&
                    map[nextPosition.x, nextPosition.y] == 1 && !visited[nextPosition.x, nextPosition.y])
                {
                    visited[nextPosition.x, nextPosition.y] = true;

                    // Create a new path including this step
                    List<Vector2Int> newPath = new List<Vector2Int>(currentPath) { nextPosition };

                    // Enqueue the new position and path
                    queue.Enqueue((nextPosition, newPath));
                }
            }
        }

        // If the loop ends without finding a path, no path exists.
        Debug.LogError("No path found from spawn to goal.");
    }

    public int FindLongestShortestPath(int[,] map)
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
                                tempPathCells.Clear();
                                int pathLength = BreadthFirstSearch(map, startRow, startCol, endRow, endCol);
                                if (pathLength > longestShortestPath)
                                {
                                    longestShortestPath = pathLength;
                                    spawnx = startRow;
                                    spawnz = startCol;
                                    goalx = endRow;
                                    goalz = endCol;


                                }
                            }
                        }
                    }
                }
            }
        }

        return longestShortestPath;
    }
    private int BreadthFirstSearch(int[,] map, int startRow, int startCol, int endRow, int endCol)
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
                    tempPathCells.Add(new Vector2Int(newRow, newCol));
                    
                    
                }
            }
        }

        return -1; // Path not found
    }


    
    
}
