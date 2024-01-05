using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothTime = 0.3f; // Smoothing factor for camera movement

    private Transform roomToFollow; // Change the type to Transform

    private Vector2 velocity = Vector2.zero;

    void LateUpdate()
    {
        if (roomToFollow != null)
        {
            Vector2 targetPosition = new Vector2(roomToFollow.position.x, roomToFollow.position.y);
            transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        }
    }

    // Method to set the room to follow during runtime
    public void SetRoomToFollow(Transform roomTransform)
    {
        roomToFollow = roomTransform;
    }
}
