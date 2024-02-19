using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamev2Manager : MonoBehaviour
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
    public int length, width;
    public int[,] grid;
    public GameObject spawno,baseo,roboto;
    public int goalx, goalz, robotz;
    private int lastx, lastz, llastx, llastz;

    private GameObject[,] spawnedObjects; // To keep track of spawned prefabs

    public List<Vector2Int> pathCells;
    public bool endEpisode = false;
    public int lastDirtx, lastDirtz;
    public EnemyWaveManagerV2 enemyWaveManager;



    private void Start()
    {
        enemyWaveManager = FindObjectOfType<EnemyWaveManagerV2>();
        roboto = FindObjectOfType<Gamev2Agent>().gameObject;
        roboto.SetActive(false);//remove for TRAINING
        GenerateRandomGrid(length, width);
        spawnedObjects = new GameObject[length, width]; // Initialize the array

        DisplayGrid();
        

    }
    public void ActivateRobot(){
        roboto.SetActive(true);
    }
    public void DeactivateRobot(){
        roboto.SetActive(false);
    }

    public bool updateMap(int x, int z)
    {
        
        if (x < 0 || x >= length || z < 0 || z >= width) return false;
        GameObject currentObject = spawnedObjects[x, z];
        if ( grid[x, z] == 1 || grid[x, z] == 3)
        {
            Destroy(currentObject);
            GameObject dirt = Instantiate(dirtPrefab, new Vector3(x, 0, z), Quaternion.identity);
            if (lastx != x ){
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
            grid[x, z] = 0;
            llastx = lastx;
            llastz = lastz;
            lastx = x;
            lastz = z;
            lastDirtx = x;
            lastDirtz = z;
            pathCells.Add(new Vector2Int(x, z));
            return true;

        }
        else if (grid[x, z] == 0){
            if(lastDirtx != x || lastDirtz != z){
                endEpisode = true;
            }

        }   
        return false;
    }

    public void reset(){
        enemyWaveManager.stopWave();
        endEpisode = false;
        pathCells.Clear();  
        GenerateRandomGrid(length, width);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (spawnedObjects[i, j] != null)
                {
                    Destroy(spawnedObjects[i, j]);

                }
            }
        }
        Destroy(spawno);
        Destroy(baseo);
        DisplayGrid();
        //find all object tagged turret and destroy them
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject turret in turrets)
        {
            Destroy(turret);
        }
        //find all object tagged enemy and destroy them
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        

    }

    public float distanceToGoal(float x, float z)
    {
        return Mathf.Sqrt(Mathf.Pow(goalx - x, 2) + Mathf.Pow(goalz - z, 2));
    }
    private void DisplayGrid()
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int cellValue = grid[i, j];
                Vector3 position = new Vector3(i, 0, j);
                GameObject spawnedPrefab = Instantiate(grassPrefab, position, Quaternion.identity);
                spawnedObjects[i, j] = spawnedPrefab; // Keep track of the spawned prefab

                if (cellValue == 2)
                {
                    spawno = Instantiate(spawnPrefab, new Vector3(i, 0.1f, j), Quaternion.identity);
                    lastx = i;
                    lastz = j;
                    llastx = i;
                    llastz = j;
                    //roboto.SetActive(false);  //removed for TRAINING
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
    
    private void GenerateRandomGrid(int length,int width)
    {
        grid = new int[length, width];
        int spawnrand = Random.Range(0, width);
        int baserand = Random.Range(0, width);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = 1;

                if (i == 0 && j == spawnrand)
                {
                    grid[i, j] = 2;
                    lastDirtx = i+1;
                    lastDirtz = j;
                    roboto.transform.position = new Vector3(i, 1, j);
                    
                    
                }
                if (i == length-1 && j == baserand)
                {
                    grid[i, j] = 3;
                    
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

                            Destroy(spawnedObjects[x, y]);
                        }
                        else if (randomObject < 60)
                        {
                            GameObject spawnedPrefab2 = Instantiate(forestPrefab, new Vector3(x, 0f, y), Quaternion.identity);

                            Destroy(spawnedObjects[x, y]);
                        }
                        else if (randomObject < 90)
                        {
                            GameObject spawnedPrefab2 = Instantiate(forest2Prefab, new Vector3(x, 0f, y), Quaternion.identity);

                            Destroy(spawnedObjects[x, y]);
                        }
                        else if (randomObject < 120)
                        {
                            GameObject spawnedPrefab2 = Instantiate(rockPrefab, new Vector3(x, 0f, y), Quaternion.identity);

                            Destroy(spawnedObjects[x, y]);
                        }
                        else
                        {
                            GameObject spawnedPrefab2 = Instantiate(rock2Prefab, new Vector3(x, 0f, y), Quaternion.identity);

                            Destroy(spawnedObjects[x, y]);
                        }
                        
                    }

                       

                }
            }
        }
    }
}
