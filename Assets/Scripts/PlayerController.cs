using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private RoomManager roomManager;       // room creationn�l instantiatelem a playert, �gy nem ment�dik el a roommanager, look into it
    private Vector3 movement;
    private Room currentRoom;
    private CameraFollow cameraFollow;

    void Start()
    {
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

    void MoveToConnectedRoom(Vector2Int direction)
    {
        if (currentRoom != null)
        {
            GameObject playerObject = Player.Instance.GetPlayerObject();
            Vector3 playerPosition = playerObject.transform.position;

            playerObject.transform.position = CalculateNewPlayerPosition(direction, playerPosition);

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

    Vector2Int DetermineDirectionOfDoor(GameObject door)
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
