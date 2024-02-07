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
            SetDoorTriggerState(topDoor, true);
        }

        if  (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
            SetDoorTriggerState(bottomDoor, true);
        }

        if  (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
            SetDoorTriggerState(leftDoor, true);
        }

        if  (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
            SetDoorTriggerState(rightDoor, true);
        }
    }

    Transform GetDoorTransform(Vector2Int direction)
    {
        Transform doorsParent = transform.Find("Doors");

        if (doorsParent == null)
        {
            Debug.LogError("Doors parent not found.");
            return null;
        }

        Transform doorTransform = null;

        if (direction == Vector2Int.left)
            doorTransform = doorsParent.Find("LeftDoor");
        else if (direction == Vector2Int.right)
            doorTransform = doorsParent.Find("RightDoor");
        else if (direction == Vector2Int.up)
            doorTransform = doorsParent.Find("TopDoor");
        else if (direction == Vector2Int.down)
            doorTransform = doorsParent.Find("BottomDoor");

        if (doorTransform == null)
        {
            Debug.LogError($"Door in direction {direction} not found.");
        }

        return doorTransform;
    }

    public void SetDoorTriggerState(GameObject doorObject, bool isTrigger)
    {
        Collider2D doorCollider = doorObject.GetComponent<Collider2D>();

        if (doorCollider != null)
        {
            doorCollider.isTrigger = isTrigger;
        }
        else
        {
            Debug.LogError("Collider2D component not found on the doorObject.");
        }
    }

    public void DisableAllDoors()
    {
        SetDoorTriggerState(topDoor, false);
        SetDoorTriggerState(bottomDoor, false);
        SetDoorTriggerState(leftDoor, false);
        SetDoorTriggerState(rightDoor, false);
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

    public GameObject TopDoor
    {
        get => topDoor;
        set => topDoor = value;
    }

    public GameObject BottomDoor
    {
        get => bottomDoor;
        set => bottomDoor = value;
    }

    public GameObject LeftDoor
    {
        get => leftDoor;
        set => leftDoor = value;
    }

    public GameObject RightDoor
    {
        get => rightDoor;
        set => rightDoor = value;
    }
}
