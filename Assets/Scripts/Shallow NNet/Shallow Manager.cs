using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shallowManager : MonoBehaviour
{
    public GameObject dirtPrefab;
    public GameObject cornerPrefab;
    public GameObject grassPrefab;
    public GameObject spawnPrefab;
    public GameObject basePrefab;
    public GameObject robotPrefab;

    
    public shallowGenerator gridGenerator;
    public int length, width;
    public int goalx, goalz, robotz;
    private int lastx, lastz, llastx, llastz;


    private GameObject[,] spawnedObjects; // To keep track of spawned prefabs
    private GameObject spawno,baseo,roboto;

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
        gridGenerator = new shallowGenerator(length, width);
        spawnedObjects = new GameObject[length, width]; // Initialize the array
        DisplayGrid();
    }

    public bool fixedUpdates(int x, int z)
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
            return true;

        }
        llastx = lastx;
        llastz = lastz;
        lastx = x;
        lastz = z;
        return false;
    }
    public void reset()
    {



        spawno.transform.position = new Vector3(0, 0.1f, robotz);
        baseo.transform.position = new Vector3(goalx, 0.1f, goalz);
        roboto.transform.position = new Vector3(1, 0.2f, robotz);
        //set llastx and llastz to spawno position
        llastx = 0;
        llastz = robotz;
        lastx = 0;
        lastz = robotz;
        for (int i = 0; i < gridGenerator.grid.GetLength(0); i++)
        {
            for (int j = 0; j < gridGenerator.grid.GetLength(1); j++)
            {
                // Replace the prefab at each cell with grass
                ReplacePrefabWithGrass(i, j);
            }
        }
        
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

    void DisplayGrid()
    {
        for (int i = 0; i < gridGenerator.grid.GetLength(0); i++)
        {
            for (int j = 0; j < gridGenerator.grid.GetLength(1); j++)
            {
                int cellValue = gridGenerator.grid[i, j];
                Vector3 position = new Vector3(i, 0, j);
                GameObject prefabToInstantiate = null;

                switch (cellValue)
                {
                    case 0:
                        prefabToInstantiate = dirtPrefab;
                        break;
                    case 1:
                        prefabToInstantiate = grassPrefab;
                        break;
                    case 2:
                    case 3:
                        prefabToInstantiate = dirtPrefab;
                        break;
                }

                if (prefabToInstantiate != null)
                {
                    GameObject spawnedPrefab = Instantiate(prefabToInstantiate, position, Quaternion.identity);
                    spawnedObjects[i, j] = spawnedPrefab; // Keep track of the spawned prefab

                    if (cellValue == 2)
                    {
                        spawno = Instantiate(spawnPrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                        roboto = Instantiate(robotPrefab, new Vector3(i+1, 0.2f, j), Quaternion.identity);
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
    }
}