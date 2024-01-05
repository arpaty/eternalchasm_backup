using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    private Room currentRoom;
    private Vector3 movement;

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the character collides with something
        Debug.Log("Collided with: " + collision.gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Doors"))
        {
            // Implement your door logic here
            Debug.Log("Player entered a door.");
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);
    }

      // Call this method when you want the player to transition to another room
    void MoveToConnectedRoom(Vector2Int direction)
    {
        if (currentRoom != null)
        {
            // Check with the current room for the connected room in the specified direction
            Vector2Int connectedRoomIndex = currentRoom.GetConnectedRoomIndex(direction);

            // Perform the transition logic (update player position, camera follow, etc.)
            // This is where you might want to access the RoomManager or some other manager script
            // to handle the room transition logic.

            // For now, we'll just print a message
            Debug.Log($"Player transitioning to the connected room {connectedRoomIndex}");

            // Clear the current room reference after transition
            currentRoom = null;
        }
    }
}
