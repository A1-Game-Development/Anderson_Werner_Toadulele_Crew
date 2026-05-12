using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float smoothTime = 0.12f;

    private static PlayerController instance;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 rawInput;
    private Vector2 smoothedInput;
    private Vector2 inputVelocity;

    private Vector2 lastDirection = Vector2.down;

    [HideInInspector] public NPCBase currentNPC;


    public static Vector2 savedLastDirection = Vector2.down;
    public static Vector2 entryDirection = Vector2.zero;
    public static string spawnOverride = "";

    private static bool hasSpawnedOnce = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        Debug.Log("[PlayerController] Awake complete");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[PlayerController] Enabled + sceneLoaded subscribed");
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("[PlayerController] Disabled + sceneLoaded unsubscribed");
    }

    // -----------------------------
    // INPUT
    // -----------------------------
    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
        Debug.Log("[PlayerController] Move input: " + rawInput);
    }

    // -----------------------------
    // INTERACT (OPTION A)
    // -----------------------------
    public void OnInteract(InputValue value)
    {
        Debug.Log("[PlayerController] OnInteract triggered");

        if (currentNPC != null)
        {
            Debug.Log("[PlayerController] NPC found ? sending interact");
            currentNPC.HandleInteract();
        }
        else
        {
            Debug.Log("[PlayerController] No NPC in range");
        }
    }

        // -----------------------------
    // EXIT DIRECTION
    // -----------------------------
    public void SetExitDirection()
    {
        entryDirection = lastDirection;
        Debug.Log("[PlayerController] SetExitDirection: " + entryDirection);
    }

    // -----------------------------
    // FORCE SPAWN RESET
    // -----------------------------
    public void ForceSpawnReset()
    {
        entryDirection = Vector2.zero;
        spawnOverride = "";
        hasSpawnedOnce = false;

        Debug.Log("[PlayerController] ForceSpawnReset called");
    }

    void Update()
    {
        smoothedInput = Vector2.SmoothDamp(
            smoothedInput,
            rawInput,
            ref inputVelocity,
            smoothTime
        );

        Vector2 moveInput = rawInput;

        if (moveInput.magnitude < 0.2f)
            moveInput = Vector2.zero;

        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;
            savedLastDirection = moveInput;
        }

        animator.SetFloat("Speed", smoothedInput.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = smoothedInput * movementSpeed;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[PlayerController] Scene loaded: " + scene.name);

        Transform spawn = GetSpawnPoint();

        if (spawn != null)
        {
            transform.position = spawn.position;
            Debug.Log("[PlayerController] Spawned at: " + spawn.name);
        }
        else
        {
            Debug.LogWarning("[PlayerController] Spawn point missing!");
        }
    }

    Transform GetSpawnPoint()
    {
        string spawnName = "Spawn_Default";
        GameObject obj = GameObject.Find(spawnName);

        if (obj == null)
        {
            Debug.LogWarning("[PlayerController] Cannot find spawn: " + spawnName);
            return null;
        }

        return obj.transform;
    }

    void LateUpdate()
    {
        animator.SetFloat("LastMoveX", lastDirection.x);
        animator.SetFloat("LastMoveY", lastDirection.y);
    }
}