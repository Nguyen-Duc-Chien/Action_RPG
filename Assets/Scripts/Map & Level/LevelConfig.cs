using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_XX", menuName = "Roguelike/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Info")]
    public int levelIndex;      
    public string levelName;    

    [Header("Map Settings")]
    public List<GameObject> roomPrefabs;    
    public int totalRooms = 5;              

    [Header("Enemy Settings")]
    public EnemyData enemyData;             
    public int maxEnemiesPerRoom = 3;       

    [Header("Kill Target")]
    public int targetKillsToWin = 0;

    [Header("Boss")]
    public bool hasBoss = false;
}
