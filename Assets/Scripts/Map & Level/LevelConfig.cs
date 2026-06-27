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
    [Tooltip("Enemy pool riêng cho level này. Mỗi level có thể có tập quái khác nhau.")]
    public List<EnemySpawnRate> enemyPool;

    [Tooltip("Số quái tối đa spawn mỗi phòng")]
    public int maxEnemiesPerRoom = 3;       

    [Header("Kill Target")]
    public int targetKillsToWin = 0;

    [Header("Boss")]
    public bool hasBoss = false;

    // ── Backward Compatibility ──────────────────────────────────────────────
    // Giữ lại field cũ để không mất data trong Inspector.
    // MapGenerator sẽ ưu tiên dùng enemyPool inline, nếu rỗng thì fallback về enemyData SO.
    [Header("Legacy (Fallback)")]
    [Tooltip("Fallback: Dùng khi enemyPool ở trên rỗng. Sẽ bị bỏ sau khi migrate xong.")]
    public EnemyData enemyData;

    /// <summary>
    /// Trả về enemy pool hiệu lực: ưu tiên inline list, fallback về EnemyData SO.
    /// </summary>
    public List<EnemySpawnRate> GetEffectiveEnemyPool()
    {
        if (enemyPool != null && enemyPool.Count > 0)
            return enemyPool;

        if (enemyData != null && enemyData.enemyPool != null && enemyData.enemyPool.Count > 0)
            return enemyData.enemyPool;

        return new List<EnemySpawnRate>();
    }
}
