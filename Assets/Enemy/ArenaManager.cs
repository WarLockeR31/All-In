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

    [Header("Настройки волн")]
    public int startEliteWave = 5;
    public float timeBetweenWaves = 5f;

    [Header("Настройки спавна")]
    public List<GameObjectIntPair> normalEnemies; // Список префабов с вероятностями
    public List<GameObjectIntPair> eliteEnemies; // Список элитных префабов с вероятностями
    private Transform[] spawnPoints;
    public GameObject spawnPointsParent;
    public float minDistanceFromPlayer = 10f;

    [Header("Коэффициенты увеличения характеристик")]
    public float increaseStartWave = 5;
    public float healthIncreasePerWave = 1.1f; // Увеличение здоровья на 10% за волну
    public float damageIncreasePerWave = 1.05f; // Увеличение урона на 5% за волну
    public float speedIncreasePerWave = 1.02f; // Увеличение скорости на 2% за волну

    private int waveNumber = 0;
    private bool isSpawning = false;
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();
    private int curEnemyCount;

    // Приватные коэффициенты для текущей волны
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
            if (child != spawnPointsParent.transform) // Исключаем родительский объект
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

        // Увеличиваем коэффициенты характеристик после каждой волны
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

        // Спавн обычных врагов
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy(normalEnemies);
        }

        // Спавн элитных врагов
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

        GameObject enemyPrefab = GetRandomEnemy(enemyList); // Выбираем врага с учетом вероятности
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Применяем коэффициенты к характеристикам врага
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
        // Вычисляем общую сумму вероятностей
        int totalProbability = 0;
        foreach (var pair in enemyList)
        {
            totalProbability += pair.chancePercent;
        }

        // Генерируем случайное число от 0 до общей суммы вероятностей
        int randomValue = Random.Range(0, totalProbability);

        // Выбираем врага на основе случайного числа
        int cumulativeProbability = 0;
        foreach (var pair in enemyList)
        {
            cumulativeProbability += pair.chancePercent;
            if (randomValue < cumulativeProbability)
            {
                return pair.gameObject;
            }
        }

        // Если что-то пошло не так, возвращаем первый элемент
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