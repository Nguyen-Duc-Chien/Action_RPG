using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("List of Room Variants")]
    public List<GameObject> roomPrefabs;

    [Header("Map Grid Configuration")]
    public int totalRoomsToSpawn;
    public float roomLength;
    public float roomWidth;

    private EnemyData _currentEnemyData;
    private int _maxEnemiesPerRoom = 3;
    
    private List<Vector2> occupiedPositions = new List<Vector2>();
    private List<RoomWalls> spawnedRooms = new List<RoomWalls>();

    void Start()
    {
        if (RunManager.Instance != null)
        {
            LevelConfig cfg = RunManager.Instance.GetCurrentConfig();
            if (cfg != null)
            {
                if (cfg.roomPrefabs != null && cfg.roomPrefabs.Count > 0)
                    roomPrefabs = cfg.roomPrefabs;

                if (cfg.totalRooms > 0)
                    totalRoomsToSpawn = cfg.totalRooms;

                _currentEnemyData    = cfg.enemyData;
                _maxEnemiesPerRoom   = cfg.maxEnemiesPerRoom;

                Debug.Log($"<color=cyan>[MapGenerator]</color> Loaded config for Level {cfg.levelIndex}: {cfg.levelName} | Rooms: {totalRoomsToSpawn} | Max enemies/room: {_maxEnemiesPerRoom}");
            }
        }

        GenerateMap();
    }

    void GenerateMap()
    {
        if (roomPrefabs == null || roomPrefabs.Count == 0)
        {
            Debug.LogError("There isn't Prefab room in MapGenerator!");
            return;
        }

        List<RoomEnemySpawner> allSpawners = new List<RoomEnemySpawner>();

        Vector2 currentPos = Vector2.zero;
        GameObject firstRoom = Instantiate(roomPrefabs[0], new Vector3(currentPos.x, currentPos.y, 0), Quaternion.identity);
        firstRoom.name = "Starting_Room";
        occupiedPositions.Add(currentPos);
        if (firstRoom.GetComponent<RoomWalls>()) spawnedRooms.Add(firstRoom.GetComponent<RoomWalls>());
        LinkSpikeToPlayer(firstRoom);

        RoomEnemySpawner startSpawner = firstRoom.GetComponent<RoomEnemySpawner>();
        if (startSpawner != null)
        {
            allSpawners.Add(startSpawner);
        }

        for (int i = 1; i < totalRoomsToSpawn; i++)
        {
            Vector2 nextPos = GetRandomNeighborPosition();

            int randomIndex = Random.Range(0, roomPrefabs.Count);

            GameObject newRoom = Instantiate(roomPrefabs[randomIndex], new Vector3(nextPos.x, nextPos.y, 0), Quaternion.identity);
            newRoom.name = "Room_" + i;
            LinkSpikeToPlayer(newRoom);

            RoomWalls roomWallComponent = newRoom.GetComponent<RoomWalls>();
            if (roomWallComponent != null)
            {
                occupiedPositions.Add(nextPos);
                spawnedRooms.Add(roomWallComponent);

                RoomEnemySpawner spawner = newRoom.GetComponent<RoomEnemySpawner>();
                if (spawner != null)
                {
                    allSpawners.Add(spawner);
                }
            }
            else
            {
                Debug.LogWarning(newRoom.name + " is missing RoomWalls!");
                Destroy(newRoom);
                i--;
            }
        }
        CheckAndFixWalls();

        int targetTotal = 0;
        if (RunManager.Instance != null)
        {
            LevelConfig cfg = RunManager.Instance.GetCurrentConfig();
            if (cfg != null) targetTotal = cfg.targetKillsToWin;
        }

        if (targetTotal > 0)
        {
            int[] spawnsPerRoom = new int[allSpawners.Count];
            List<int> availableRooms = new List<int>();
            for (int i = 0; i < allSpawners.Count; i++)
            {
                if (allSpawners[i].GetSpawnPointCount() > 0)
                    availableRooms.Add(i);
            }

            int enemiesLeft = targetTotal;
            while (enemiesLeft > 0 && availableRooms.Count > 0)
            {
                int rndIdx = Random.Range(0, availableRooms.Count);
                int roomIdx = availableRooms[rndIdx];

                spawnsPerRoom[roomIdx]++;
                enemiesLeft--;

                int maxPossible = Mathf.Min(_maxEnemiesPerRoom, allSpawners[roomIdx].GetSpawnPointCount());
                if (spawnsPerRoom[roomIdx] >= maxPossible)
                {
                    availableRooms.RemoveAt(rndIdx);
                }
            }

            for (int i = 0; i < allSpawners.Count; i++)
            {
                allSpawners[i].Initialize(_currentEnemyData, spawnsPerRoom[i]);
                if (spawnsPerRoom[i] > 0)
                {
                    allSpawners[i].ExecuteSpawning();
                }
            }

            if (enemiesLeft > 0 && LevelManager.Instance != null)
            {
                LevelManager.Instance.OverrideKillTarget(targetTotal - enemiesLeft);
            }
        }
        else
        {
            foreach (var spawner in allSpawners)
            {
                spawner.Initialize(_currentEnemyData, _maxEnemiesPerRoom);
                spawner.ExecuteSpawning();
            }
        }

        if (LevelManager.Instance != null)
        {
            Debug.Log($"[MapGenerator] Created {totalRoomsToSpawn} rooms! Total enemies: <color=yellow>{LevelManager.Instance.targetKillsToWin}</color>.");
        }
        else
        {
            Debug.Log($"Spawned {totalRoomsToSpawn} random rooms!");
        }
    }
    void LinkSpikeToPlayer(GameObject roomObject)
    {
        Tilemap[] roomTilemaps = roomObject.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tm in roomTilemaps)
        {
            if (tm.gameObject.name == "Spikes_Trap")
            {
                PlayerSpikeHandler playerSpikeHandler = FindAnyObjectByType<PlayerSpikeHandler>();
                if (playerSpikeHandler != null)
                {
                    playerSpikeHandler.AddSpikeTilemap(tm);
                }
                break;
            }
        }
    }
    void CheckAndFixWalls()
    {
        for (int i = 0; i < occupiedPositions.Count; i++)
        {
            Vector2 roomPos = occupiedPositions[i];

            bool hasUp = occupiedPositions.Contains(roomPos + new Vector2(0, roomLength));
            bool hasDown = occupiedPositions.Contains(roomPos + new Vector2(0, -roomLength));
            bool hasLeft = occupiedPositions.Contains(roomPos + new Vector2(-roomWidth, 0));
            bool hasRight = occupiedPositions.Contains(roomPos + new Vector2(roomWidth, 0));

            if (i < spawnedRooms.Count && spawnedRooms[i] != null)
            {
                spawnedRooms[i].SetupWalls(hasUp, hasDown, hasLeft, hasRight);
            }
        }
    }

    Vector2 GetRandomNeighborPosition()
    {
        bool validPositionFound = false;
        Vector2 checkingPos = Vector2.zero;

        while (!validPositionFound)
        {
            Vector2 randomExistingRoom = occupiedPositions[Random.Range(0, occupiedPositions.Count)];

            int randomDirection = Random.Range(0, 4);
            checkingPos = randomExistingRoom;

            switch (randomDirection)
            {
                case 0: checkingPos += new Vector2(0, roomLength); break;
                case 1: checkingPos += new Vector2(0, -roomLength); break;
                case 2: checkingPos += new Vector2(-roomWidth, 0); break;
                case 3: checkingPos += new Vector2(roomWidth, 0); break;
            }

            if (!occupiedPositions.Contains(checkingPos))
            {
                validPositionFound = true;
            }
        }
        return checkingPos;
    }
}

