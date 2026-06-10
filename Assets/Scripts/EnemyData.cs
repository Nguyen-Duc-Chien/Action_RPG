using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnRate
{
    public GameObject enemyPrefab;
    [Range(0, 100)] public float spawnChance;
}

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Roguelike/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public List<EnemySpawnRate> enemyPool;
}