using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Room currentRoom;

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0f);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the character collides with something
        Debug.Log("Collided with: " + collision.gameObject.name);
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
