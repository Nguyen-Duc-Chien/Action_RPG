using System.Collections.Generic;
using UnityEngine;

public class RoomEnemySpawner : MonoBehaviour
{
    [Header("Data Config")]
    [SerializeField] private EnemyData enemyData;

    [Header("Spawn Rules")]
    [SerializeField] private int maxEnemiesInThisRoom = 3;

    // Runtime pool — ưu tiên dùng list truyền từ MapGenerator
    private List<EnemySpawnRate> _runtimePool;

    /// <summary>
    /// Khởi tạo bằng inline enemy pool (từ LevelConfig).
    /// </summary>
    public void Initialize(List<EnemySpawnRate> pool, int maxEnemies)
    {
        if (pool != null && pool.Count > 0) _runtimePool = pool;
        if (maxEnemies >= 0) maxEnemiesInThisRoom = maxEnemies;
    }

    /// <summary>
    /// Fallback: khởi tạo bằng EnemyData SO (backward compatibility).
    /// </summary>
    public void Initialize(EnemyData data, int maxEnemies)
    {
        if (data != null) enemyData = data;
        if (maxEnemies >= 0) maxEnemiesInThisRoom = maxEnemies;
    }

    private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> activeEnemies = new List<GameObject>();

    private bool isPlayerInside = false;
    private bool hasSpawnedFirstTime = false;
    private bool isCleared = false;

    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeated += OnEnemyGlobalDie;
    }

    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeated -= OnEnemyGlobalDie;
    }

    /// <summary>
    /// Trả về enemy pool hiệu lực: ưu tiên runtime pool, fallback về enemyData SO.
    /// </summary>
    private List<EnemySpawnRate> GetPool()
    {
        if (_runtimePool != null && _runtimePool.Count > 0)
            return _runtimePool;

        if (enemyData != null && enemyData.enemyPool != null)
            return enemyData.enemyPool;

        return null;
    }

    public void ExecuteSpawning()
    {
        var pool = GetPool();
        if (pool == null || pool.Count == 0) return;

        if (spawnPoints.Count == 0)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.CompareTag("EnemySpawnPoint")) spawnPoints.Add(child);
            }
        }

        if (spawnPoints.Count == 0) return;

        isCleared = false;
        hasSpawnedFirstTime = true;
        activeEnemies.Clear();

        ShuffleSpawnPoints();

        int spawnLimit = Mathf.Min(maxEnemiesInThisRoom, spawnPoints.Count);
        for (int i = 0; i < spawnLimit; i++)
        {
            GameObject enemyPrefab = GetRandomEnemy(pool);
            if (enemyPrefab != null)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity, transform);
                activeEnemies.Add(enemy);

                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.RegisterSpawnedEnemy();
                }
            }
        }
    }

    private void OnEnemyGlobalDie(int exp)
    {
        if (!isPlayerInside || isCleared) return;

        activeEnemies.RemoveAll(enemy => enemy == null);

        if (activeEnemies.Count == 0)
        {
            isCleared = true;
            Debug.Log($"{gameObject.name} cleared this wave!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;

            if (!hasSpawnedFirstTime)
            {
                ExecuteSpawning();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private GameObject GetRandomEnemy(List<EnemySpawnRate> pool)
    {
        float totalWeight = 0;
        foreach (var enemy in pool) totalWeight += enemy.spawnChance;
        float randomValue = Random.Range(0, totalWeight);
        float currentWeightSum = 0;
        foreach (var enemy in pool)
        {
            currentWeightSum += enemy.spawnChance;
            if (randomValue <= currentWeightSum) return enemy.enemyPrefab;
        }
        return null;
    }

    public int GetSpawnPointCount()
    {
        if (spawnPoints.Count == 0)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.CompareTag("EnemySpawnPoint")) spawnPoints.Add(child);
            }
        }
        return spawnPoints.Count;
    }

    private void ShuffleSpawnPoints()
    {
        for (int i = spawnPoints.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            Transform temp = spawnPoints[i];
            spawnPoints[i] = spawnPoints[rnd];
            spawnPoints[rnd] = temp;
        }
    }
}