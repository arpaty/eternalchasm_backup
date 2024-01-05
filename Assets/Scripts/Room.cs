using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    public Vector2Int RoomIndex { get; set; }

    public void OpenDoor(Vector2Int direction) 
    {
        if  (direction == Vector2Int.up)
        {
            topDoor.SetActive(true);
        }

        if  (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
        }

        if  (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
        }

        if  (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
        }
    }

    // This method returns the connected room index based on the direction
    public Vector2Int GetConnectedRoomIndex(Vector2Int direction)
    {
        // Implement logic to determine the connected room index based on the direction
        // This could involve looking up a predefined mapping or using some algorithm
        // In a simple example, we'll just return the current room index plus the direction.

        return RoomIndex + direction;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Player entered the room trigger zone
            // Implement additional logic if needed
            Debug.Log("Player entered the room!");
        }
    }
}
