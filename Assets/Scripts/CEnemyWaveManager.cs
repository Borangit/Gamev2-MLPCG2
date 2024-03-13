using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CEnemyWaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1f;  // Interval between each enemy spawn
    public float waveInterval = 10f;  // Interval between each wave
    public int enemyPerWave;
    public int waveN;
    public List<Vector2Int> pathCells;
    public int hitpoints; // Default hitpoints value, you can adjust this in the inspector
    public GameObject explosionEffect;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private Dictionary<GameObject, int> enemyPathIndices = new Dictionary<GameObject, int>();
    private float spawnTimer;
    private float waveTimer;
    private GridManager gridManager;

    private int waveCount = 0;
    public bool startWaveOnStart = false;
    

    void Start()
    {
        spawnTimer = spawnInterval;  // Initialize the spawn timer to start spawning immediately
        waveTimer = waveInterval;  // Initialize the wave timer to start spawning immediately
    }
    public void startWave(){
        gridManager = FindObjectOfType<GridManager>();
        pathCells = gridManager.pathCells;
        startWaveOnStart = true;
    }

    void Update()
    {
        if (startWaveOnStart == false) {
            return;
        }
        // Handle enemy spawning\
        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && waveCount < enemyPerWave && waveTimer >= waveInterval && waveN > 0)
        {
            SpawnEnemy();
            spawnTimer = 0f;  // Reset spawn timer
            waveCount++;
        }
        if (waveCount >= enemyPerWave)
        {
            waveCount = 0;
            waveTimer = 0f;
            waveN--;
            
        }

        // Move all active enemies along the path
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = activeEnemies[i];
            if (enemy == null) 
            {
                activeEnemies.RemoveAt(i);
                continue;
            }

            int nextPathCellIndex = enemyPathIndices[enemy];
            if (nextPathCellIndex < pathCells.Count)
            {
                enemy.transform.position = Vector3.MoveTowards(
                    enemy.transform.position, 
                    new Vector3(pathCells[nextPathCellIndex].x, 0f, pathCells[nextPathCellIndex].y), 
                    Time.deltaTime * 3f
                );
                if (enemy.transform.position == new Vector3(pathCells[nextPathCellIndex].x, 0f, pathCells[nextPathCellIndex].y))
                {
                    enemyPathIndices[enemy]++;
                }
            }
            if (enemy.transform.position.x == gridManager.width-1)
            {
                Destroy(enemy);
                activeEnemies.RemoveAt(i);
                CurrencyManager.health -= 10;
            }
        }
        if (waveN == 0 && activeEnemies.Count == 0){
            SceneManager.LoadScene(5);
        }
    }

    public void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(pathCells[0].x, 0f, pathCells[0].y), Quaternion.identity);
        enemy.GetComponent<Enemy>().setHitpoints(hitpoints);
        enemy.GetComponent<Enemy>().setExplosionEffect(explosionEffect);
        activeEnemies.Add(enemy);
        enemyPathIndices.Add(enemy, 1);
    }

    public void setPathCells(List<Vector2Int> newPathCells)
    {
        pathCells = newPathCells;
    }

    public void setEnemyCount(int newEnemyCount)
    {
        enemyPerWave = newEnemyCount;
    }
}