using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour {
    private Player player;
    private CameraMode currentCameraMode;
    private Transform firstPersonCameraHolder;
    private CinemachineCamera firstPersonCamera;
    private CinemachineCamera thirdPersonCamera;
    private Vector3 standingCameraLocalPos;
    private Vector3 crouchingCameraLocalPos;
    private float cameraLerpSpeed;
    private float verticalRotation = 0f;

    public void Initialize(Player player) {
        this.player = player;
        currentCameraMode = player.currentCameraMode;
        firstPersonCameraHolder = player.firstPersonCameraHolder;
        firstPersonCamera = player.firstPersonCamera;
        thirdPersonCamera = player.thirdPersonCamera;
        standingCameraLocalPos = player.standingCameraLocalPos;
        crouchingCameraLocalPos = player.crouchingCameraLocalPos;
        cameraLerpSpeed = player.cameraLerpSpeed;
    }

    private void Start() {
        if (currentCameraMode == CameraMode.FirstPerson) {
            firstPersonCamera.Priority = 1;
            thirdPersonCamera.Priority = 0;
        }
        else {
            firstPersonCamera.Priority = 0;
            thirdPersonCamera.Priority = 1;
        }
        player.inputHandler.onPOVToggleAction += ToggleCameraMode;
    }

    private void OnDisable() {
        player.inputHandler.onPOVToggleAction -= ToggleCameraMode;
    }

    private void Update() {
        HandleLook();
        HandleCameraHeight();
    }

    private void ToggleCameraMode() {
        if (currentCameraMode == CameraMode.FirstPerson) {
            currentCameraMode = CameraMode.ThirdPerson;
            firstPersonCamera.Priority = 0;
            thirdPersonCamera.Priority = 1;
        }
        else {
            currentCameraMode = CameraMode.FirstPerson;
            firstPersonCamera.Priority = 1;
            thirdPersonCamera.Priority = 0;
        }
        player.currentCameraMode = currentCameraMode;
    }

    private void HandleLook() {
        Vector2 look = player.inputHandler.lookDelta;
        if (currentCameraMode == CameraMode.FirstPerson) {
            transform.Rotate(Vector3.up * look.x * player.lookSensitivity);
            verticalRotation -= look.y * player.lookSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            firstPersonCameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }

    private void HandleCameraHeight() {
        Vector3 targetPos = player.isCrouching ? crouchingCameraLocalPos : standingCameraLocalPos;
        firstPersonCameraHolder.localPosition = Vector3.Lerp(
            firstPersonCameraHolder.localPosition,
            targetPos,
            Time.deltaTime * cameraLerpSpeed
        );
    }
}
