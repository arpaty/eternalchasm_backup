using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private RoomManager roomManager;       // room creationn�l instantiatelem a playert, �gy nem ment�dik el a roommanager, look into it
    private Vector3 movement;
    private Room currentRoom;
    private CameraFollow cameraFollow;
    private Vector2Int cumulativeDirection = Vector2Int.zero;

    void Start()
    {
        currentRoom = Player.Instance.GetCurrentRoom();
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            Debug.LogError("CameraFollow script not found on the main camera.");
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        movement = new Vector3(horizontal, vertical, 0f);
    }

    void FixedUpdate()
    {
        Rigidbody2D rb = Player.Instance.GetRigidBody();

        rb.MovePosition(transform.position + movement * Player.Instance.GetMoveSpeed() * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // check if the character collides with something
        //Debug.Log("Collided with: " + collision.gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Doors"))
        {
            // Handle door interaction
            Debug.Log("Player entered the door!");
            Vector2Int direction = DetermineDirectionOfDoor(other.gameObject);
            MoveToConnectedRoom(direction);
        }
        else if (other.CompareTag("Walls"))
        {
            // Handle wall interaction
            //Debug.Log("Player hit a wall!");
        }
    }

    void OnDirectionInput(Vector2Int direction)
    {
        cumulativeDirection += direction;
    }


    void MoveToConnectedRoom(Vector2Int direction)
    {
        if (Player.Instance.GetCurrentRoom() != null)
        {
            //Debug.Log($"Currentroom {currentRoom}");
            GameObject playerObject = Player.Instance.GetPlayerObject();
            Vector3 playerPosition = playerObject.transform.position;

            Debug.Log($"Direction {direction}");

            playerObject.transform.position = CalculateNewPlayerPositione(direction, playerPosition);

            Vector2Int adjacentRoomIndex = currentRoom.GetConnectedRoomIndexFrom(direction);
            Room adjacentRoom = roomManager.GetRoomScriptAt(adjacentRoomIndex);
            SetCurrentRoom(adjacentRoom);

            cameraFollow.SetRoomToFollow(currentRoom.transform);
        }
    }

    Vector3 CalculateNewPlayerPosition(Vector2Int direction, Vector3 newPlayerPosition)
    {
        if (direction == Vector2Int.left)
            newPlayerPosition += new Vector3(-5, 0, 0);
        else if (direction == Vector2Int.right)
            newPlayerPosition += new Vector3(5, 0, 0);
        else if (direction == Vector2Int.up)
            newPlayerPosition += new Vector3(0, 5, 0);
        else if (direction == Vector2Int.down)
            newPlayerPosition += new Vector3(0, -5, 0);

        return newPlayerPosition;
    }

    Vector3 CalculateNewPlayerPositione(Vector2Int direction, Vector3 newPlayerPosition)
    {
        float moveSpeed = 5f;

        Vector3 movement = Vector3.zero;
        if (direction.x != 0)
            movement += new Vector3(direction.x * moveSpeed, 0, 0);
        if (direction.y != 0)
            movement += new Vector3(0, direction.y * moveSpeed, 0);
        if(direction.x != 0 && direction.y != 0)
            movement += new Vector3(direction.x * moveSpeed, direction.y * moveSpeed, 0);

        newPlayerPosition += movement;

        return newPlayerPosition;
    }

    Vector2Int DetermineDirectionOfDoor(GameObject door)        // seems to be bugging out when entering the side of the door, not in the center (gives wrong directional input)
    {
        Vector3 doorPosition = door.transform.position;
        Vector3 playerPosition = transform.position;

        // difference in position
        Vector3 positionDifference = doorPosition - playerPosition;

        float xDifference = Mathf.Abs(positionDifference.x);
        float yDifference = Mathf.Abs(positionDifference.y);

        Vector2Int directionOfDoor;

        // check which axis has a larger difference
        if (xDifference > yDifference)
        {
            // more horizontally aligned
            if (positionDifference.x > 0)
            {
                directionOfDoor = Vector2Int.right;
            }
            else
            {
                directionOfDoor = Vector2Int.left;
            }
        }
        else
        {
            // more vertically aligned
            if (positionDifference.y > 0)
            {
                directionOfDoor = Vector2Int.up;
            }
            else
            {
                directionOfDoor = Vector2Int.down;
            }
        }

        // debug
        //Debug.Log($"The door is: {directionOfDoor}");

        return directionOfDoor;
    }


    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;
    }

    internal void SetRoomManager(RoomManager _roomManager)
    {
        roomManager = _roomManager;
    }
}
