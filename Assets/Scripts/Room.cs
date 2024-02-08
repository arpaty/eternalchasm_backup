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

    internal Vector3 GetDoorPosition(Vector2Int direction)
    {
        Transform doorsParent = transform.Find("Doors");

        if (doorsParent == null)
        {
            Debug.LogError("Doors parent not found.");
            return Vector3.zero;
        }

        Vector3 doorPosition = Vector3.zero;

        if (direction == Vector2Int.left)
            doorPosition = doorsParent.Find("LeftDoor").position;
        else if (direction == Vector2Int.right)
            doorPosition = doorsParent.Find("RightDoor").position;
        else if (direction == Vector2Int.up)
            doorPosition = doorsParent.Find("TopDoor").position;
        else if (direction == Vector2Int.down)
            doorPosition = doorsParent.Find("BottomDoor").position;

        return doorPosition;
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
    public Vector2Int GetConnectedRoomIndexFrom(Vector2Int direction)
    {
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
