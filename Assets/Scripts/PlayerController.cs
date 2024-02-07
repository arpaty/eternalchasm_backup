using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; } // singleton

    Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private RoomManager roomManager;       // room creationn�l instantiatelem a playert, �gy nem ment�dik el a roommanager, look into it
    private Vector3 movement;
    private Room currentRoom;

    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerController>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("PlayerController instance created.");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // this is so the player won't bounce off of the edges of the wall
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
        rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);
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
            MoveToConnectedRoom(direction); // be kell fejezni m�g, nincs meg
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
            /*Debug.Log("Belemegy");
            Vector2Int connectedRoomIndex = currentRoom.GetConnectedRoomIndex(direction);

            //Vector3 newPosition = currentRoom.GetDoorPositionInConnectedRoom(direction);
            Vector3 newPosition = roomManager.GetPositionFromGridIndex(connectedRoomIndex);
            //playerObject.transform.position = newPosition;

            Debug.Log($"Player transitioning to the connected room {connectedRoomIndex}");

            currentRoom = null;*/
        }
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
}
