using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDisplayer : MonoBehaviour
{
    public GameObject dirtPrefab;
    public GameObject cornerPrefab;
    public GameObject grassPrefab;
    public GameObject spawnPrefab;
    public GameObject basePrefab;
    public GameObject robotPrefab;
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject rock2Prefab;
    public GameObject forestPrefab;
    public GameObject forest2Prefab;

    public int objectChance = 30;
    public GridGenerator gridGenerator;
    public int length, width;
    public int goalx, goalz, robotz;
    private int lastx, lastz, llastx, llastz;
    public List<Vector2Int> pathCells;



    private GameObject[,] spawnedObjects; // To keep track of spawned prefabs
    private GameObject[,] spawnedObjects2; // To keep track of spawned tree/rocks
    public GameObject spawno,baseo,roboto;

    public float distanceToGoal(float x, float z)
    {
        return Mathf.Sqrt(Mathf.Pow(goalx - x, 2) + Mathf.Pow(goalz - z, 2));
    }
    public float distanceToGoalX(float x)
    {
        return goalx - x;
    }
    public float distanceToGoalZ(float z)
    {
        return goalz - z;
    }


    private void Start()
    {
        gridGenerator = new GridGenerator(length, width);
        spawnedObjects = new GameObject[length, width]; // Initialize the array
        spawnedObjects2 = new GameObject[length, width]; // Initialize the array
        DisplayGrid();
    }

    public bool updateMap(int x, int z)
    {   if (x < 0 || x >= length || z < 0 || z >= width) return false;
        
        GameObject currentObject = spawnedObjects[x, z];
        if (currentObject != null && currentObject.CompareTag("Grass"))
        {
            Destroy(currentObject);
            GameObject dirt = Instantiate(dirtPrefab, new Vector3(x, 0, z), Quaternion.identity);
            if (lastx != x){
                //rotate dirt by 90 degrees
                dirt.transform.Rotate(0, 90, 0);
            }
            //Debug.Log("llastx: " + llastx + " llastz: " + llastz + " lastx: " + lastx + " lastz: " + lastz + " x: " + x + " z: " + z);
            if (llastx != x && llastz != z){
                Destroy(spawnedObjects[lastx, lastz]);
                GameObject corner = Instantiate(cornerPrefab, new Vector3(lastx, 0, lastz), Quaternion.identity);
                spawnedObjects[lastx, lastz] = corner;
                if ((llastx == lastx + 1 && z == lastz - 1) || (x == lastx + 1 && llastz == lastz - 1)){
                    corner.transform.Rotate(0, 90, 0);
                }
                else if ((llastx == lastx - 1 && z == lastz - 1) || (x == lastx - 1 && llastz == lastz - 1)){
                    corner.transform.Rotate(0, 180, 0);
                }
                else if ((llastx == lastx - 1 && z == lastz + 1) || (x == lastx - 1 && llastz == lastz + 1)){
                    corner.transform.Rotate(0, 270, 0);
                }
            }   
            spawnedObjects[x, z] = dirt;
            gridGenerator.grid[x, z] = 0; // Update the grid value to dirt
            llastx = lastx;
            llastz = lastz;
            lastx = x;
            lastz = z;
            pathCells.Add(new Vector2Int(x, z));

            return true;

        }


        return false;
    }
    public void reset(int x)
    {
        for (int i=0; spawnedObjects2.GetLength(0) > i; i++)
        {
            for (int j=0; spawnedObjects2.GetLength(1) > j; j++)
            {
                if (spawnedObjects2[i, j] != null)
                {
                    GameObject currentObject = spawnedObjects2[i, j];
                    Destroy(currentObject);
                }
            }
        }
        if (x != 0){
            goalx = length-1;
            goalz = Random.Range(0, width);
            robotz = Random.Range(0, width);
            spawno.transform.position = new Vector3(0, 0.1f, robotz);
            baseo.transform.position = new Vector3(length-1, 0.1f, goalz);
            roboto.transform.position = new Vector3(1, 0.2f, robotz);
            //set llastx and llastz to spawno position
            llastx = 0;
            llastz = robotz;
            lastx = 0;
            lastz = robotz;
        }
        else {
            spawno.transform.position = new Vector3(0, 0.1f, robotz);
            baseo.transform.position = new Vector3(length-1, 0.1f, goalz);
            roboto.transform.position = new Vector3(1, 0.2f, robotz);
            //set llastx and llastz to spawno position
            llastx = 0;
            llastz = robotz;
            lastx = 0;
            lastz = robotz;
        }
        
        for (int i = 0; i < gridGenerator.grid.GetLength(0); i++)
        {
            for (int j = 0; j < gridGenerator.grid.GetLength(1); j++)
            {
                // Replace the prefab at each cell with grass
                ReplacePrefabWithGrass(i, j);
            }
        }
        pathCells.Clear();
        roboto.SetActive(true);
        
        
    }

    private void ReplacePrefabWithGrass(int x, int z)
    {
        GameObject currentObject = spawnedObjects[x, z];
        if (currentObject != null)
        {
            Destroy(currentObject);
            GameObject grass = Instantiate(grassPrefab, new Vector3(x, 0, z), Quaternion.identity);
            spawnedObjects[x, z] = grass;
            gridGenerator.grid[x, z] = 1; // Update the grid value to grass
        }
    }
    public void ActivateRobot(){
        roboto.SetActive(true);
    }
    public void DeactivateRobot(){
        Debug.Log("Deactivating robot");
        roboto.SetActive(false);
        roboto.transform.position = new Vector3(-100, -100, -100);
    }


    void DisplayGrid()
    {
        for (int i = 0; i < gridGenerator.grid.GetLength(0); i++)
        {
            for (int j = 0; j < gridGenerator.grid.GetLength(1); j++)
            {
                int cellValue = gridGenerator.grid[i, j];
                Vector3 position = new Vector3(i, 0, j);
                GameObject prefabToInstantiate = grassPrefab;
                GameObject spawnedPrefab = Instantiate(prefabToInstantiate, position, Quaternion.identity);
                spawnedObjects[i, j] = spawnedPrefab; // Keep track of the spawned prefab

                if (cellValue == 2)
                {
                    spawno = Instantiate(spawnPrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                    roboto = Instantiate(robotPrefab, new Vector3(i+1, 0.2f, j), Quaternion.identity);
                    roboto.SetActive(false);  //removed for TRAINING
                    robotz = j;


                }
                if (cellValue == 3)
                {
                    baseo = Instantiate(basePrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                    goalx = i;
                    goalz = j;
                }
                
            }
        }
    }

    
    public void layObjects()
    {
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y <width; y++)
            {
                if (!pathCells.Contains(new Vector2Int(x, y)))
                {
                    int random = Random.Range(0, 100);
                    if (random < objectChance)
                    {
                        int randomObject = Random.Range(0, 150);
                        if (randomObject < 30)
                        {
                            GameObject spawnedPrefab2 = Instantiate(treePrefab, new Vector3(x, 0f, y), Quaternion.identity);
                            spawnedObjects2[x, y] = spawnedPrefab2;
                        }
                        else if (randomObject < 60)
                        {
                            GameObject spawnedPrefab2 = Instantiate(forestPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                            spawnedObjects2[x, y] = spawnedPrefab2;
                        }
                        else if (randomObject < 90)
                        {
                            GameObject spawnedPrefab2 = Instantiate(forest2Prefab, new Vector3(x, 0f, y), Quaternion.identity);
                            spawnedObjects2[x, y] = spawnedPrefab2;
                        }
                        else if (randomObject < 120)
                        {
                            GameObject spawnedPrefab2 = Instantiate(rockPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                            spawnedObjects2[x, y] = spawnedPrefab2;
                        }
                        else
                        {
                            GameObject spawnedPrefab2 = Instantiate(rock2Prefab, new Vector3(x, 0f, y), Quaternion.identity);
                            spawnedObjects2[x, y] = spawnedPrefab2;
                        }
                        
                    }
                    else{
                        spawnedObjects2[x, y] = null;
                    }
                       

                }
            }
        }
    }
}