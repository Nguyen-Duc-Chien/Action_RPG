using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("List of Room Variants")]
    public List<GameObject> roomPrefabs;

    [Header("Map Grid Configuration")]
    public int totalRoomsToSpawn;
    public float roomLength;
    public float roomWidth;
    
    private List<Vector2> occupiedPositions = new List<Vector2>();
    private List<RoomWalls> spawnedRooms = new List<RoomWalls>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        if (roomPrefabs == null || roomPrefabs.Count == 0)
        {
            Debug.LogError("There isn't Prefab room in MapGenerator!");
            return;
        }

        // 1. Spawn 1st room at (0,0)
        Vector2 currentPos = Vector2.zero;
        GameObject firstRoom = Instantiate(roomPrefabs[0], new Vector3(currentPos.x, currentPos.y, 0), Quaternion.identity);
        firstRoom.name = "Starting_Room";
        occupiedPositions.Add(currentPos);
        if (firstRoom.GetComponent<RoomWalls>()) spawnedRooms.Add(firstRoom.GetComponent<RoomWalls>());

        // 2. Generate the rest of the rooms
        for (int i = 1; i < totalRoomsToSpawn; i++)
        {
            Vector2 nextPos = GetRandomNeighborPosition();

            // Pick a random room prefab from the list (excluding the first one which is used for the starting room)
            int randomIndex = Random.Range(0, roomPrefabs.Count);

            GameObject newRoom = Instantiate(roomPrefabs[randomIndex], new Vector3(nextPos.x, nextPos.y, 0), Quaternion.identity);
            newRoom.name = "Room_" + i;

            RoomWalls roomWallComponent = newRoom.GetComponent<RoomWalls>();
            if (roomWallComponent != null)
            {
                occupiedPositions.Add(nextPos);
                spawnedRooms.Add(roomWallComponent);
            }
            else
            {
                Debug.LogWarning(newRoom.name + " is missing RoomWalls!");
                Destroy(newRoom);
                i--;
            }
        }
        CheckAndFixWalls();
        Debug.Log("Spawned " + totalRoomsToSpawn + " random rooms!");
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

    // Find a random valid neighboring position to spawn the next room
    Vector2 GetRandomNeighborPosition()
    {
        bool validPositionFound = false;
        Vector2 checkingPos = Vector2.zero;

        while (!validPositionFound)
        {
            // Pick a random existing room from the list of occupied positions
            Vector2 randomExistingRoom = occupiedPositions[Random.Range(0, occupiedPositions.Count)];

            // Choose a random direction (up, down, left, right) to check for the next room position
            int randomDirection = Random.Range(0, 4);
            checkingPos = randomExistingRoom;

            switch (randomDirection)
            {
                case 0: checkingPos += new Vector2(0, roomLength); break;
                case 1: checkingPos += new Vector2(0, -roomLength); break;
                case 2: checkingPos += new Vector2(-roomWidth, 0); break;
                case 3: checkingPos += new Vector2(roomWidth, 0); break;
            }

            // If the checking position is not already occupied, we can use it for the next room
            if (!occupiedPositions.Contains(checkingPos))
            {
                validPositionFound = true;
            }
        }
        return checkingPos;
    }
}

