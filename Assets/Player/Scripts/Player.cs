using Unity.Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Components")]
    public PlayerMovement playerMovement { get; private set; }
    public PlayerAnimation playerAnimation { get; private set; }
    public PlayerCamera playerCamera { get; private set; }
    public InputHandler inputHandler { get; private set; }
    public Animator animator { get; private set; }
    public Transform mainCamera { get; private set; }
    private CharacterController characterController;
    public Vector3 velocity;

    public bool isGrounded;
    public bool isSprinting;
    public bool isCrouching;

    [Header("Player Settings")]
    public float standingHeight;
    public float crouchingHeight;
    public float moveSpeed = 5f;
    public float normalSpeedMult = 1f;
    public float sprintSpeedMult = 1.5f;
    public float crouchSpeedMult = 0.5f;
    public float lookSensitivity = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    public GameObject playerMesh1;
    public GameObject playerMesh2;

    [Header("Camera Settings")]
    public CameraMode currentCameraMode;
    public Transform firstPersonCameraHolder;
    public CinemachineCamera firstPersonCamera;
    public CinemachineCamera thirdPersonCamera;

    [Header("First Person Settings")]
    public Vector3 standingCameraLocalPos = new Vector3(0f, 1.6f, 0f);
    public Vector3 crouchingCameraLocalPos = new Vector3(0f, 1.0f, 0f);
    public float cameraLerpSpeed = 10f;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimation>();
        inputHandler = GetComponent<InputHandler>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main.transform;
        standingHeight = characterController.height;
        standingCameraLocalPos = firstPersonCameraHolder.localPosition;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentCameraMode == CameraMode.FirstPerson)
        {
            firstPersonCamera.Priority = 1;
            thirdPersonCamera.Priority = 0;

            lookSensitivity = lookSensitivity * 0.1f;
        }
        else
        {
            firstPersonCamera.Priority = 0;
            thirdPersonCamera.Priority = 1;
        }
    }

    private void OnEnable()
    {
        inputHandler.onPOVToggleAction += ToggleCameraMode;
    }

    private void OnDisable()
    {
        inputHandler.onPOVToggleAction -= ToggleCameraMode;
    }

    private void ToggleCameraMode()
    {
        if (currentCameraMode == CameraMode.FirstPerson)
        {
            currentCameraMode = CameraMode.ThirdPerson;

            firstPersonCamera.Priority = 0;
            thirdPersonCamera.Priority = 1;
            lookSensitivity = lookSensitivity * 10f;

            mainCamera.GetComponent<Camera>().cullingMask = LayerMask.GetMask("Default", "ThirdPersonOnly");
        }
        else
        {
            currentCameraMode = CameraMode.FirstPerson;

            firstPersonCamera.Priority = 1;
            thirdPersonCamera.Priority = 0;
            lookSensitivity = lookSensitivity * 0.1f;

            mainCamera.GetComponent<Camera>().cullingMask = LayerMask.GetMask("Default", "FirstPersonOnly");
        }
    }
}
