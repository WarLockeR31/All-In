using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    #region Singleton
    public static ArenaManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Настройки волн")]
    public int startEliteWave = 5;
    public float timeBetweenWaves = 5f;

    [Header("Настройки спавна")]
    public GameObject[] normalEnemies;
    public GameObject[] eliteEnemies;
    private Transform[] spawnPoints;
    public GameObject spawnPointsParent;
    public float minDistanceFromPlayer = 10f;

    private int waveNumber = 0;
    private bool isSpawning = false;
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();
    private int curEnemyCount;

    void Start()
    {
        Transform[] childTransforms = spawnPointsParent.GetComponentsInChildren<Transform>();
        spawnPoints = new Transform[childTransforms.Length - 1];
        int i = 0;
        foreach (Transform child in childTransforms)
        {
            if (child != spawnPointsParent.transform) // Исключаем родительский объект
            {
                spawnPoints[i] = child;
                i++;
            }
        }

        SpawnWave();
    }

    void SpawnWave()
    {
        if (isSpawning) return;
        isSpawning = true;

        int eliteCount = waveNumber >= startEliteWave ? Random.Range(1, waveNumber / 2) : 0;

        curEnemyCount = Random.Range(waveNumber + 2, waveNumber + 5);
        int enemyCount = curEnemyCount - eliteCount;
        List<Transform> usedPoints = new List<Transform>();

        //Enemies
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy(normalEnemies);
        }
        
        //Elite Enemies
        if (waveNumber >= startEliteWave)
        {
            for (int i = 0; i < eliteCount; i++)
            {
                SpawnEnemy(eliteEnemies);
            }
        }

        waveNumber++;
        isSpawning = false;
    }

    void SpawnEnemy(GameObject[] enemyArray)
    {
        if (enemyArray.Length == 0 || spawnPoints.Length == 0) return;

        Transform spawnPoint = FindAvailableSpawnPoint();
        if (spawnPoint == null) return;

        GameObject enemyPrefab = enemyArray[Random.Range(0, enemyArray.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        occupiedSpawnPoints.Add(spawnPoint);
    }

    Transform FindAvailableSpawnPoint()
    {
        List<Transform> availablePoints = new List<Transform>();
        foreach (Transform point in spawnPoints)
        {
            if (!occupiedSpawnPoints.Contains(point) &&
                Vector3.Distance(point.position, Player.Instance.transform.position) >= minDistanceFromPlayer)
            {
                availablePoints.Add(point);
            }
        }

        return availablePoints.Count > 0
            ? availablePoints[Random.Range(0, availablePoints.Count)]
            : null;
    }

    public void FreeSpawnPoints()
    {
        occupiedSpawnPoints.Clear();
    }

    public void DecEnemyCount()
    {
        curEnemyCount--;
        if (curEnemyCount == 0)
        {
            FreeSpawnPoints();
            SpawnWave();
        }
    }
}