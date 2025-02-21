using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectIntPair
    {
        public GameObject gameObject;
        [Range(0, 100)]
        public int chancePercent;
    }

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

    [Header("��������� ����")]
    public int startEliteWave = 5;
    public float timeBetweenWaves = 5f;

    [Header("��������� ������")]
    public List<GameObjectIntPair> normalEnemies; // ������ �������� � �������������
    public List<GameObjectIntPair> eliteEnemies; // ������ ������� �������� � �������������
    private Transform[] spawnPoints;
    public GameObject spawnPointsParent;
    public float minDistanceFromPlayer = 10f;

    [Header("������������ ���������� �������������")]
    public float increaseStartWave = 5;
    public float healthIncreasePerWave = 1.1f; // ���������� �������� �� 10% �� �����
    public float damageIncreasePerWave = 1.05f; // ���������� ����� �� 5% �� �����
    public float speedIncreasePerWave = 1.02f; // ���������� �������� �� 2% �� �����

    private int waveNumber = 0;
    private bool isSpawning = false;
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();
    private int curEnemyCount;

    // ��������� ������������ ��� ������� �����
    private float currentHealthMultiplier = 1f;
    private float currentDamageMultiplier = 1f;
    private float currentSpeedMultiplier = 1f;

    void Start()
    {
        Transform[] childTransforms = spawnPointsParent.GetComponentsInChildren<Transform>();
        spawnPoints = new Transform[childTransforms.Length - 1];
        int i = 0;
        foreach (Transform child in childTransforms)
        {
            if (child != spawnPointsParent.transform) // ��������� ������������ ������
            {
                spawnPoints[i] = child;
                i++;
            }
        }

        SpawnWave();
    }

    public void SpawnWave()
    {
        if (isSpawning) return;
        waveNumber++;
        isSpawning = true;

        // ����������� ������������ ������������� ����� ������ �����
        if (waveNumber > increaseStartWave)
        {
            currentHealthMultiplier *= healthIncreasePerWave;
            currentDamageMultiplier *= damageIncreasePerWave;
            currentSpeedMultiplier *= speedIncreasePerWave;
        }

        int eliteCount = waveNumber >= startEliteWave ? Random.Range(waveNumber / 2 - 2, waveNumber / 2) : 0;
        if (eliteCount > spawnPoints.Length)
        {
            eliteCount = spawnPoints.Length;
        }

        curEnemyCount = Random.Range(waveNumber + 1, waveNumber + 4);
        if (curEnemyCount > spawnPoints.Length)
            curEnemyCount = spawnPoints.Length;

        int enemyCount = curEnemyCount - eliteCount;
        List<Transform> usedPoints = new List<Transform>();

        // ����� ������� ������
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy(normalEnemies);
        }

        // ����� ������� ������
        if (waveNumber >= startEliteWave)
        {
            for (int i = 0; i < eliteCount; i++)
            {
                SpawnEnemy(eliteEnemies);
            }
        }

        isSpawning = false;
    }

    void SpawnEnemy(List<GameObjectIntPair> enemyList)
    {
        if (enemyList.Count == 0 || spawnPoints.Length == 0) return;

        Transform spawnPoint = FindAvailableSpawnPoint();
        if (spawnPoint == null)
        {
            curEnemyCount--;
            return;
        }

        GameObject enemyPrefab = GetRandomEnemy(enemyList); // �������� ����� � ������ �����������
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // ��������� ������������ � ��������������� �����
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.SetStats(enemyStats.MaxHealth * currentHealthMultiplier, enemyStats.Damage * currentDamageMultiplier, enemyStats.Speed * currentSpeedMultiplier);
        }

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

    GameObject GetRandomEnemy(List<GameObjectIntPair> enemyList)
    {
        // ��������� ����� ����� ������������
        int totalProbability = 0;
        foreach (var pair in enemyList)
        {
            totalProbability += pair.chancePercent;
        }

        // ���������� ��������� ����� �� 0 �� ����� ����� ������������
        int randomValue = Random.Range(0, totalProbability);

        // �������� ����� �� ������ ���������� �����
        int cumulativeProbability = 0;
        foreach (var pair in enemyList)
        {
            cumulativeProbability += pair.chancePercent;
            if (randomValue < cumulativeProbability)
            {
                return pair.gameObject;
            }
        }

        // ���� ���-�� ����� �� ���, ���������� ������ �������
        return enemyList[0].gameObject;
    }

    public void FreeSpawnPoints()
    {
        occupiedSpawnPoints = new HashSet<Transform>();
    }

    public void DecEnemyCount()
    {
        curEnemyCount--;
        if (curEnemyCount == 0)
        {
            FreeSpawnPoints();
            UIManager.Instance.ToggleUI();
            //SpawnWave();
        }
    }
}