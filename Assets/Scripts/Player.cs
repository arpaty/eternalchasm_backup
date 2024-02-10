using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    private PlayerController playerController;
    

    private Vector3 position;       // gameobject has pos attribute btw
    private Room currentRoom;
    private GameObject playerObject;

    private Rigidbody2D rb;

    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("Player instance created.");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // this is so the player won't bounce off of the edges of the wall
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public void SetCurrentRoom(Room _currentRoom)
    {
        currentRoom = _currentRoom;
    }

    public Rigidbody2D GetRigidBody()
    {
        return rb;
    }
    
    public void SetPosition(Vector3 _position)
    {
        position = _position;
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public void SetPlayerController(PlayerController _playerController)
    {
        playerController = _playerController;
    }

    public GameObject GetPlayerObject()
    {
        return playerObject;
    }

    public void SetPlayerObject(GameObject _playerObject)
    {
        playerObject = _playerObject;
    }
}
