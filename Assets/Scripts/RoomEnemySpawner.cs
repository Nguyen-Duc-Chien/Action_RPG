using System.Collections.Generic;
using UnityEngine;

public class RoomEnemySpawner : MonoBehaviour
{
    [Header("Data Config")]
    [SerializeField] private EnemyData enemyData;

    [Header("Spawn Rules")]
    [SerializeField] private int maxEnemiesInThisRoom = 3;
    [Range(0f, 100f)][SerializeField] private float respawnChance = 50f;

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

    public void ExecuteSpawning()
    {
        if (enemyData == null || enemyData.enemyPool.Count == 0) return;

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
            GameObject enemyPrefab = GetRandomEnemy();
            if (enemyPrefab != null)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity, transform);
                activeEnemies.Add(enemy);
            }
        }

        Debug.Log($"{gameObject.name}: Spawned {activeEnemies.Count} enemies!");
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
            else if (isCleared)
            {
                if (Random.Range(0f, 100f) <= respawnChance)
                {
                    Debug.Log($"{gameObject.name}: Respawned new wave for farming exp!");
                    ExecuteSpawning();
                }
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

    private GameObject GetRandomEnemy()
    {
        float totalWeight = 0;
        foreach (var enemy in enemyData.enemyPool) totalWeight += enemy.spawnChance;
        float randomValue = Random.Range(0, totalWeight);
        float currentWeightSum = 0;
        foreach (var enemy in enemyData.enemyPool)
        {
            currentWeightSum += enemy.spawnChance;
            if (randomValue <= currentWeightSum) return enemy.enemyPrefab;
        }
        return null;
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