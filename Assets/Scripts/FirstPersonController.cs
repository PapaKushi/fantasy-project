using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    // Camera Settings
    [Header("Camera Settings")]
    [SerializeField] private float walkFOV = 60f; // FOV of the camera when idling or walking.
    [SerializeField] private float sprintFOV = 120f; // FOV of camera when sprinting.
    [SerializeField] private float FOVTransitionMultiplier = 0.25f; // Speed at which FOV transitions between sprint and run.

    // Camera Bobbing Settings
    [SerializeField] private float bobAmount = 0.05f;
    [SerializeField] private float bobSpeed = 10f;

    // Movment Speed Settings
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f; // Speed while walking.
    [SerializeField] private float sprintMultiplier = 2f; // How many times faster sprint than walk.

    // Jumping Settings
    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f; // Force of the jump. Related to max height.
    [SerializeField] private float gravity = 9.8f; // How strong the downward restoring force is.

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f; // How far you can look up and down.

    // Inputs
    [Header("Input Customization")]
    [SerializeField] private string horiztonalMovementInput = "Horizontal";
    [SerializeField] private string verticalMovementInput = "Vertical";
    [SerializeField] private string mouseXInput = "Mouse X";
    [SerializeField] private string mouseYInput = "Mouse Y";
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioSource footStepSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float velocityThreshhold = 2.0f;

    private float baseCameraY;
    private float currentCameraFOV;
    private int lastPlayedIndex = -1;
    private bool isMoving = false;
    private float nextStepTime;
    private Camera mainCamera;
    private float verticalRotation;
    private Vector3 currentMovement = Vector3.zero;
    private CharacterController characterController;

    private void Start()
    {
        mainCamera = Camera.main;
        characterController = GetComponent<CharacterController>();
        currentCameraFOV = walkFOV;
        baseCameraY = mainCamera.transform.localPosition.y;
    }


    // Update is called once per frame
    void Update()
    {
        HandleMovement(); // Handle walking and sprinting.

        // Skip look input entirely while a UI menu (inventory or quest log)
        // is open, so mouse movement doesn't spin the camera while
        // managing items or reading quests.
        if (!Inventory_Toggle.IsInventoryOpen && !Quest_Log_Toggle.IsQuestLogOpen)
        {
            HandleRotation(); // Handle looking around.
        }

        HandleFootsteps(); // Handle the sounds related to walking/sprinting.
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis(verticalMovementInput);
        float horizontalInput = Input.GetAxis(horiztonalMovementInput);

        // Detect if sprint key is pressed and set appropriate speed multiplier.
        float speedMultiplier = Input.GetKey(sprintKey) ? sprintMultiplier : 1f;

        // Handle forward, backward, and side to side movement.
        float verticalSpeed = verticalInput * walkSpeed * speedMultiplier;
        float horiztonalSpeed = horizontalInput * walkSpeed * speedMultiplier;

        Vector3 horizontalMovement = new Vector3(horiztonalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        HandleGravityAndJumping();
        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);

        isMoving = verticalInput != 0 || horizontalInput != 0;

        // FOV should always ease toward its target, even after the player stops moving,
        // otherwise it can get stuck elevated if sprint is released right as movement stops.
        HandleSprintFOV();

        if (isMoving)
        {
            handleCameraBobbing();
        }
    }

    void HandleGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (Input.GetKeyDown(jumpKey))
            {
                currentMovement.y = jumpForce;
            }
        } else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        float mouseXRotation = Input.GetAxis(mouseXInput) * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= Input.GetAxis(mouseYInput) * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleSprintFOV()
    {
        if (Input.GetKey(sprintKey) && currentCameraFOV < sprintFOV)
        {
            currentCameraFOV = currentCameraFOV + FOVTransitionMultiplier;
            mainCamera.fieldOfView = currentCameraFOV;
        } else if (!Input.GetKey(sprintKey) && currentCameraFOV > walkFOV)
        {
            currentCameraFOV = currentCameraFOV - FOVTransitionMultiplier;
            mainCamera.fieldOfView = currentCameraFOV;
        }
    }

    void handleCameraBobbing()
    {
        float bobYOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        mainCamera.transform.localPosition = new Vector3(0, baseCameraY + bobYOffset, 0);
    }

    void HandleFootsteps()
    {
        float currentStepInterval = (Input.GetKey(sprintKey) ? sprintStepInterval : walkStepInterval);

        if (characterController.isGrounded && isMoving && Time.time > nextStepTime && characterController.velocity.magnitude > velocityThreshhold)
        {
            PlayFootSounds();
            nextStepTime = Time.time + currentStepInterval;
        }
    }

    void PlayFootSounds()
    {
        int randomIndex;
        if(footstepSounds.Length == 1)
        {
            randomIndex = 0;
        } else
        {
            // Random.Range(min, max) is exclusive on max for ints, so this now
            // covers the full array (previously the last clip could never be picked).
            randomIndex = Random.Range(0, footstepSounds.Length);
            if(randomIndex >= lastPlayedIndex)
            {
                // Wrap around instead of incrementing past the end of the array.
                randomIndex = (randomIndex + 1) % footstepSounds.Length;
            }
        }

        lastPlayedIndex = randomIndex;
        footStepSource.clip = footstepSounds[randomIndex];
        footStepSource.Play();
    }
}