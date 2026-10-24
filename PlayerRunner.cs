
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerRunner : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 8f;
    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Slide")]
    public float slideDuration = 0.8f;
    public float slideHeight = 1f;
    public float slideTransitionSpeed = 12f;
    public float slideCameraDrop = 0.5f;

    [Header("Visual Animation")]
    public Transform visualRoot;
    public float slideVisualHeight = 0.55f;

    private CharacterController controller;
    private Vector3 velocity;

    private int currentLane = 0;

    private float normalHeight;
    private Vector3 normalCenter;

    private Transform playerCamera;
    private Vector3 normalCameraPosition;

    private Vector3 normalVisualScale;
    private Vector3 targetVisualScale;

    private bool isSliding = false;
    public bool IsSliding => isSliding;
    private float slideTimer = 0f;
    private bool isJumping = false;

    // Stores a slide request made while airborne
    private bool slideRequested = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Save the original collider dimensions
        normalHeight = controller.height;
        normalCenter = controller.center;

        // Find the player camera
        Camera cam = GetComponentInChildren<Camera>();

        if (cam != null)
        {
            playerCamera = cam.transform;
            normalCameraPosition = playerCamera.localPosition;
        }

        // Save the original visual scale
        if (visualRoot != null)
        {
            normalVisualScale = visualRoot.localScale;
            targetVisualScale = normalVisualScale;
        }
    }

    void Update()
    {
        HandleLaneChange();
        HandleJump();
        HandleSlide();
        AnimateSlide();
        MovePlayer();
    }

    void HandleLaneChange()
    {
        // Move left
        if ((Input.GetKeyDown(KeyCode.A) ||
             Input.GetKeyDown(KeyCode.LeftArrow)) &&
            currentLane > -1)
        {
            currentLane--;
        }

        // Move right
        if ((Input.GetKeyDown(KeyCode.D) ||
             Input.GetKeyDown(KeyCode.RightArrow)) &&
            currentLane < 1)
        {
            currentLane++;
        }
    }

    void HandleJump()
    {
        // Press W to jump or interrupt a slide
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isSliding)
            {
                EndSlide();
            }

            // Cancel a pending slide request
            slideRequested = false;

            if (controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );

                isJumping = true;
            }
        }

        // Reset vertical velocity when grounded
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            isJumping = false;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
    }

    void HandleSlide()
    {
        // Detect slide input
        if (Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isSliding)
                return;

            if (isJumping || !controller.isGrounded)
            {
                // Cancel upward movement and start descending
                velocity.y = Mathf.Min(velocity.y, -2f);
                isJumping = false;

                // Request a slide after landing
                slideRequested = true;
            }
            else
            {
                // Start sliding immediately on the ground
                StartSlide();
            }
        }

        // Start the requested slide after landing
        if (slideRequested && controller.isGrounded && !isSliding)
        {
            slideRequested = false;
            StartSlide();
        }

        // Count down the slide duration
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }
    }

    void StartSlide()
    {
        if (isSliding)
            return;

        isSliding = true;
        slideTimer = slideDuration;

        // Keep the collider bottom at the same height
        controller.height = Mathf.Max(
            slideHeight,
            controller.radius * 2f
        );

        controller.center = new Vector3(
            normalCenter.x,
            normalCenter.y -
                (normalHeight - controller.height) / 2f,
            normalCenter.z
        );

        // Compress the visual model smoothly
        if (visualRoot != null)
        {
            targetVisualScale = new Vector3(
                normalVisualScale.x,
                normalVisualScale.y * slideVisualHeight,
                normalVisualScale.z
            );
        }
    }

    void EndSlide()
    {
        isSliding = false;
        slideTimer = 0f;

        // Restore the original collider dimensions
        controller.height = normalHeight;
        controller.center = normalCenter;

        // Restore the visual model smoothly
        if (visualRoot != null)
        {
            targetVisualScale = normalVisualScale;
        }
    }

    void AnimateSlide()
    {
        // Smoothly animate the visual model
        if (visualRoot != null)
        {
            visualRoot.localScale = Vector3.Lerp(
                visualRoot.localScale,
                targetVisualScale,
                slideTransitionSpeed * Time.deltaTime
            );
        }

        // Smoothly lower or raise the camera
        if (playerCamera != null)
        {
            Vector3 targetCameraPosition = normalCameraPosition;

            if (isSliding)
            {
                targetCameraPosition -= new Vector3(
                    0f,
                    slideCameraDrop,
                    0f
                );
            }

            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                targetCameraPosition,
                slideTransitionSpeed * Time.deltaTime
            );
        }
    }

    void MovePlayer()
    {
        // Calculate the target lane position
        float targetX = currentLane * laneDistance;

        float newX = Mathf.MoveTowards(
            transform.position.x,
            targetX,
            laneChangeSpeed * Time.deltaTime
        );

        float horizontalMovement =
            newX - transform.position.x;

        // Move forward and apply vertical velocity
        Vector3 movement = new Vector3(
            horizontalMovement,
            velocity.y * Time.deltaTime,
            forwardSpeed * Time.deltaTime
        );

        controller.Move(movement);
    }
}
