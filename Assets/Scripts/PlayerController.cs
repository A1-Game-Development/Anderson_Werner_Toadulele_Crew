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
    private Vector2 animDirection = Vector2.down;

    [HideInInspector] public NPCBase currentNPC;

    // -----------------------------
    // PERSISTENCE
    // -----------------------------
    public static Vector2 savedLastDirection = Vector2.down;
    public static Vector2 entryDirection = Vector2.zero;
    public static string spawnOverride = "";

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
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // -----------------------------
    // INPUT
    // -----------------------------
    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    // -----------------------------
    // INTERACT
    // -----------------------------
    public void OnInteract(InputValue value)
    {
        if (currentNPC != null)
            currentNPC.HandleInteract();
    }

    // -----------------------------
    // EXIT DIRECTION
    // -----------------------------
    public void SetExitDirection()
    {
        entryDirection = lastDirection;
    }

    // -----------------------------
    // UPDATE
    // -----------------------------
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
            lastDirection = moveInput.normalized;
            savedLastDirection = lastDirection;
            animDirection = lastDirection;
        }

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetFloat("Speed", smoothedInput.sqrMagnitude);

        animator.SetFloat("LastMoveX", animDirection.x);
        animator.SetFloat("LastMoveY", animDirection.y);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = smoothedInput * movementSpeed;
    }

    // -----------------------------
    // SCENE LOAD
    // -----------------------------
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Transform spawn = GetSpawnPoint();

        if (spawn != null)
            transform.position = spawn.position;
        else
            Debug.LogWarning("[Spawn] No spawn found!");
    }

    // -----------------------------
    // ?? FIXED SPAWN SYSTEM
    // -----------------------------
    Transform GetSpawnPoint()
    {
        string spawnName = "Spawn_Default";

        // ---------------------------------------
        // 1. OVERRIDE SPAWN (HIGHEST PRIORITY)
        // ---------------------------------------
        if (!string.IsNullOrEmpty(spawnOverride))
        {
            spawnName = spawnOverride;
            spawnOverride = ""; // consume once
        }
        // ---------------------------------------
        // 2. DIRECTIONAL SPAWNS
        // ---------------------------------------
        else
        {
            if (entryDirection == Vector2.up)
                spawnName = "Spawn_FromDown";
            else if (entryDirection == Vector2.down)
                spawnName = "Spawn_FromUp";
            else if (entryDirection == Vector2.left)
                spawnName = "Spawn_FromRight";
            else if (entryDirection == Vector2.right)
                spawnName = "Spawn_FromLeft";
        }

        // ---------------------------------------
        // 3. FIND SPAWN OBJECT
        // ---------------------------------------
        GameObject obj = GameObject.Find(spawnName);

        if (obj == null)
        {
            Debug.LogWarning("[Spawn] Missing spawn: " + spawnName);
            return null;
        }

        Debug.Log("[Spawn] Using: " + spawnName);
        return obj.transform;
    }

    // -----------------------------
    // STABLE IDLE HANDLING
    // -----------------------------
    void LateUpdate()
    {
        if (rawInput.magnitude < 0.2f)
        {
            animator.SetFloat("LastMoveX", animDirection.x);
            animator.SetFloat("LastMoveY", animDirection.y);
        }
    }
}