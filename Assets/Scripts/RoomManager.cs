using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private GameObject playerObject;
    private Vector2 doorSize = new Vector2(1.0f, 1.0f);
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 10;

    int roomWidth = 20;
    int roomHeight = 12;

    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    private CameraFollow cameraFollow;

    private List<GameObject> roomObjects = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;

    private void Start()
    {
        roomGrid = new int [gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            Debug.LogError("CameraFollow script not found on the main camera.");
        }

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        if(roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        else if(roomCount < minRooms)
        {
            Debug.Log("RoomCount was less than the minimum amount of rooms. Trying again...");

            Destroy(playerObject);
            playerObject = null;

            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
            generationComplete = true;
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;

        roomGrid[x, y] = 1;
        roomCount++;
        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);

        if(playerObject == null)
        {
            InstantiatePlayer(roomIndex);
        }

        cameraFollow.SetRoomToFollow(initialRoom.transform);
    }

    private void InstantiatePlayer(Vector2Int roomIndex)
    {
        Vector3 playerPosition = GetPositionFromGridIndex(roomIndex);
        playerPosition.z = 0;

        playerObject = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        playerObject.name = "Player";
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if(roomCount >= maxRooms)
            return false;
        
        if(Random.value < 0.5f && roomIndex != Vector2Int.zero)
            return false;

        if(CountAdjacentRooms(roomIndex) > 1)
            return false;       // snake-like rooms

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y);

        return true;
    }

    // clear all rooms and try again
    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);

        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void DisableAllDoors()
    {
        foreach (var roomObject in roomObjects)
        {
            Room roomScript = roomObject.GetComponent<Room>();
            if (roomScript != null)
            {
                roomScript.DisableAllDoors();
            }
        }
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        // neighbours
        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // determine which doors to open based on the direction

        if(x > 0 && roomGrid[x- 1, y] != 0)
        {
            // neighbouring room to the left
            AdjustWallCollidersForDoor(Vector2Int.left, doorSize);
            newRoomScript.OpenDoor(Vector2Int.left);

            AdjustWallCollidersForDoor(Vector2Int.right, doorSize);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }

        if(x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            // neighbouring room to the right
            AdjustWallCollidersForDoor(Vector2Int.right, doorSize);
            newRoomScript.OpenDoor(Vector2Int.right);

            AdjustWallCollidersForDoor(Vector2Int.left, doorSize);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }

        if(y > 0 && roomGrid[x, y - 1] != 0)
        {
            // neighbouring room below
            AdjustWallCollidersForDoor(Vector2Int.down, doorSize);
            newRoomScript.OpenDoor(Vector2Int.down);

            AdjustWallCollidersForDoor(Vector2Int.up, doorSize);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }

        if(y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            // neighbouring room above
            AdjustWallCollidersForDoor(Vector2Int.up, doorSize);
            newRoomScript.OpenDoor(Vector2Int.up);

            AdjustWallCollidersForDoor(Vector2Int.down, doorSize);
            topRoomScript.OpenDoor(Vector2Int.down);
        }
    }

    void AdjustWallCollidersForDoor(Vector2Int doorPosition, Vector2 doorSize) // does not seem to be necessary currently (tba)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(doorPosition, doorSize, 0f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Walls"))
            {
                BoxCollider2D wallCollider = collider.GetComponent<BoxCollider2D>();

                if (wallCollider != null)
                {
                    wallCollider.enabled = false;
                }
            }
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);

        if(roomObject != null)
            return roomObject.GetComponent<Room>();

        return null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        int count = 0;

        if( x > 0 && roomGrid[x - 1, y] != 0) count++; // left neighbour
        if( x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; // right neighbour
        if( y > 0 && roomGrid[x, y - 1] != 0) count++; // bottom neighbour
        if( y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; // top neighbour

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), 
            roomHeight * (gridY - gridSizeY / 2));
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }
}
