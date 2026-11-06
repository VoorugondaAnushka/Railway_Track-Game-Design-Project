using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("Forward Movement")]
    public float forwardSpeed = 11f;
    public bool isMoving = true;

    [Header("Railway 3-Track Setup (Strictly on Rails)")]
    // 3 Tracks: Left = -3.5, Center = 0.0, Right = 3.5
    public float[] lanePositions = new float[] { -3.5f, 0f, 3.5f };
    public int currentLane = 1; // Starts on Center track
    public float laneChangeSpeed = 18f;
    public float leftLimit = -3.5f;
    public float rightLimit = 3.5f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2.4f;
    public float gravity = -26f;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;
    private float verticalVelocity = 0f;
    private float startZ;
    private int currentDistance = 0;
    private float targetX = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        startZ = transform.position.z;

        currentLane = Mathf.Clamp(currentLane, 0, lanePositions.Length - 1);
        targetX = lanePositions[currentLane];

        // Snap initial X to exact track
        Vector3 initialPos = transform.position;
        initialPos.x = targetX;
        transform.position = initialPos;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.Play("Running", 0, 0f);
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            return;
        }

        // Distance Counter along Z
        int distance = Mathf.Max(0, Mathf.FloorToInt(transform.position.z - startZ));
        if (distance != currentDistance)
        {
            currentDistance = distance;
            if (MasterLevelInfo.Instance != null)
            {
                MasterLevelInfo.Instance.UpdateDistance(currentDistance);
            }
        }

        // Discrete 3-Lane Track Switching (A/Left: switch left track, D/Right: switch right track)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0)
            {
                currentLane--;
                targetX = lanePositions[currentLane];
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < lanePositions.Length - 1)
            {
                currentLane++;
                targetX = lanePositions[currentLane];
            }
        }

        // Smoothly interpolate X towards target track lane
        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
        newX = Mathf.Clamp(newX, lanePositions[0], lanePositions[lanePositions.Length - 1]);
        float deltaX = newX - transform.position.x;

        // Vertical Movement (Jumping & Gravity)
        if (controller != null && controller.isGrounded)
        {
            verticalVelocity = -1.5f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Calculate frame movement delta
        Vector3 moveDelta = new Vector3(
            deltaX,
            verticalVelocity * Time.deltaTime,
            forwardSpeed * Time.deltaTime
        );

        if (controller != null)
        {
            controller.Move(moveDelta);
        }
        else
        {
            transform.position += moveDelta;
        }

        // Enforce strict track bounds so the player NEVER walks off tracks
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, lanePositions[0], lanePositions[lanePositions.Length - 1]);
        transform.position = pos;

        // Ensure running animation continues smoothly while moving
        if (animator != null && isMoving)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
            {
                animator.Play("Running");
            }
            if (animator.speed < 0.1f)
            {
                animator.speed = 1.35f;
            }
        }
    }

    public void StopMoving()
    {
        isMoving = false;
        if (animator != null)
        {
            animator.speed = 0f;
        }
    }
}
