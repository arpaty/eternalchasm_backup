using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private GameObject playerObject;
    private PlayerController playerController;
    private Vector2 doorSize = new Vector2(1.0f, 1.0f);
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] private int maxRooms = 20;
    [SerializeField] private int minRooms = 16;

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
            //Debug.Log($"Roomindex: {roomIndex}");
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
            PlayerController.Instance.SetCurrentRoom(null);

            RegenerateRooms();
        }
        else if (generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
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

        if(playerObject == null && initialRoom != null)
        {
            setUpPlayer(roomIndex, initialRoom);
        }

        cameraFollow.SetRoomToFollow(initialRoom.transform);
    }

    private void setUpPlayer(Vector2Int roomIndex, GameObject initialRoom)
    {
        Room currentRoomComponent = initialRoom.GetComponent<Room>();
        Vector3 playerPosition = GetPositionFromGridIndex(roomIndex);
        playerPosition.z = 0;

        playerObject = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        playerObject.name = "Player";

        if(PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController.Instance is null. Make sure it's properly initialized before calling setUpPlayer.");
            return;
        }

        PlayerController.Instance.SetCurrentRoom(currentRoomComponent);

        Debug.Log($"The room the player is in: {PlayerController.Instance.GetCurrentRoom()}"); // n�zd meg ha regeneratelni kell a roomokat, mert akkor lehet �jra kell createlni tudod
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

         if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return false;

        if(roomCount >= maxRooms)
            return false;
        
        if (roomGrid[x, y] == 1)
            return false;
        
        if(Random.value < 0.5f && roomIndex != Vector2Int.zero)
            return false;

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
            newRoomScript.OpenDoor(Vector2Int.left);

            leftRoomScript.OpenDoor(Vector2Int.right);
        }

        if(x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            // neighbouring room to the right
            newRoomScript.OpenDoor(Vector2Int.right);

            rightRoomScript.OpenDoor(Vector2Int.left);
        }

        if(y > 0 && roomGrid[x, y - 1] != 0)
        {
            // neighbouring room below
            newRoomScript.OpenDoor(Vector2Int.down);

            bottomRoomScript.OpenDoor(Vector2Int.up);
        }

        if(y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            // neighbouring room above
            newRoomScript.OpenDoor(Vector2Int.up);

            topRoomScript.OpenDoor(Vector2Int.down);
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);

        if(roomObject != null)
            return roomObject.GetComponent<Room>();

        return null;
    }

    internal Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
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
