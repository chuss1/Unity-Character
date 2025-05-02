using Unity.Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour {
    [Header("Player Components")]
    public PlayerMovement playerMovement { get; private set; }
    public PlayerAnimation playerAnimation { get; private set; }
    public PlayerCamera playerCamera { get; private set; }
    public InputHandler inputHandler { get; private set; }
    public Animator animator { get; private set; }
    public Transform mainCamera { get; private set; }

    public Vector3 velocity;
    public bool isGrounded;
    public bool isSprinting;
    public bool isCrouching;

    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public float normalSpeedMult = 1f;
    public float sprintSpeedMult = 1.5f;
    public float crouchSpeedMult = 0.5f;
    public float lookSensitivity = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float standingHeight;
    public float crouchingHeight;

    [Header("Camera Settings")]
    public CameraMode currentCameraMode;
    public Transform firstPersonCameraHolder;
    public Transform thirdPersonCameraTarget;
    public CinemachineCamera firstPersonCamera;
    public CinemachineCamera thirdPersonCamera;
    public Vector3 standingCameraLocalPos = new Vector3(0f, 1.6f, 0f);
    public Vector3 crouchingCameraLocalPos = new Vector3(0f, 1.0f, 0f);
    public float cameraLerpSpeed = 10f;

    [Header("Animation Settings")]
    public float velocityLerpSpeed = 10f;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerCamera = GetComponent<PlayerCamera>();
        inputHandler = GetComponent<InputHandler>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main.transform;

        playerMovement.Initialize(this);
        playerAnimation.Initialize(this);
        playerCamera.Initialize(this);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
