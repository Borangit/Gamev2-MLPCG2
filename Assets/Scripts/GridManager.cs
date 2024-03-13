using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    public int width = 25;
    public int height = 15;
    public int minPathLength = 30;
    public int objectChance = 30;


    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject rock2Prefab;
    public GameObject forestPrefab;
    public GameObject forest2Prefab;
    public GameObject SpawnPrefab;
    public GameObject BasePrefab;   
    public List<Vector2Int> pathCells;

    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    public TMP_InputField minPathLengthInput;
    public TMP_InputField objectChanceInput;
    private int attempts = 0;


    private PathGenerater pathGenerater;
    private CEnemyWaveManager enemyWaveManager;
    // Start is called before the first frame update

    private void Awake() {
        enemyWaveManager = FindObjectOfType<CEnemyWaveManager>();
    }
    public void start(){
        attempts = 0;
        pathGenerater = new PathGenerater(width, height);
        pathCells = pathGenerater.GeneratePath();
        int pathCellsCount = pathCells.Count;
        if (minPathLength > width + (height/2)){
            minPathLength = width + (height/2);
        }
        while (pathCellsCount < minPathLength)
        {
            attempts++;
            if (attempts > 100)
            {
                minPathLength = 0;
                break;
            }
            pathCells = pathGenerater.GeneratePath();
            pathCellsCount = pathCells.Count;
        }
        


        layPathCells(pathCells);
        layGrassCells();
        enemyWaveManager.startWave();
    }
    public void ReadHeight(){
        height = int.Parse(heightInput.text);
        if (height > 20){
            height = 20;
        }
    }
    public void ReadWidth(){
        width = int.Parse(widthInput.text);
        if (width > 30){
            width = 30;
        }
    }
    public void ReadMinPathLength(){
        minPathLength = int.Parse(minPathLengthInput.text);
        if (minPathLength > width + (height/2)){
            minPathLength = width + (height/2);
        }
    }
    public void ReadObjectChance(){
        objectChance = int.Parse(objectChanceInput.text);
    }

    private void layPathCells(List<Vector2Int> pathCells)
    {
        foreach (Vector2Int cell in pathCells)
        {
            Instantiate(dirtPrefab, new Vector3(cell.x, 0f, cell.y), Quaternion.identity);
            if (cell.x == 0){
                Instantiate(SpawnPrefab, new Vector3(cell.x, 0.1f, cell.y), Quaternion.identity);
            }
            if (cell.x == width-1)
            {
                Instantiate(BasePrefab, new Vector3(cell.x, 0.1f, cell.y), Quaternion.identity);
            }
        }
        

    }

    private void layGrassCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <height; y++)
            {
                if (pathGenerater.CellisFree(x, y))
                {
                    int random = Random.Range(0, 100);
                    if (random < objectChance)
                    {
                        int randomObject = Random.Range(0, 150);
                        if (randomObject < 30)
                        {
                            Instantiate(treePrefab, new Vector3(x, 0f, y), Quaternion.identity);
                        }
                        else if (randomObject < 60)
                        {
                            Instantiate(forestPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                        }
                        else if (randomObject < 90)
                        {
                            Instantiate(forest2Prefab, new Vector3(x, 0f, y), Quaternion.identity);
                        }
                        else if (randomObject < 120)
                        {
                            Instantiate(rockPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(rock2Prefab, new Vector3(x, 0f, y), Quaternion.identity);
                        }
                    }
                    else
                    {
                        Instantiate(grassPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                    }

                }
            }
        }
    }


    


}
