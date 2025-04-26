using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    private Player player;
    private CharacterController characterController;
    private float moveMult;
    private bool jumpRequested = false;

    public void Initialize(Player player) {
        this.player = player;
        characterController = GetComponent<CharacterController>();
        moveMult = player.normalSpeedMult;
    }

    private void Start() {
        player.inputHandler.onSprintStartAction += StartSprinting;
        player.inputHandler.onSprintStopAction += StopSprinting;
        player.inputHandler.onJumpAction += Jump;
        player.inputHandler.onCrouchAction += ToggleCrouch;
    }

    private void OnDestroy() {
        player.inputHandler.onSprintStartAction -= StartSprinting;
        player.inputHandler.onSprintStopAction -= StopSprinting;
        player.inputHandler.onJumpAction -= Jump;
        player.inputHandler.onCrouchAction -= ToggleCrouch;
    }

    private void Update() {
        player.isGrounded = characterController.isGrounded;
        if (player.isGrounded && !jumpRequested) {
            HandleMovement();
        }
        ApplyGravity();
    }

    private void HandleMovement() {
        if (player.isGrounded && player.velocity.y < 0) {
            player.velocity.y = -2f;
        }

        // Determine movement direction based on the current camera mode
        Vector3 moveDirection = player.currentCameraMode == CameraMode.FirstPerson
            ? GetFirstPersonMovement()
            : GetCameraRelativeMovement();

        characterController.Move(moveDirection * player.moveSpeed * moveMult * Time.deltaTime);

        // Only rotate the player to face the movement direction in third-person mode
        if (player.currentCameraMode == CameraMode.ThirdPerson && moveDirection.sqrMagnitude > 0.01f) {
            FaceMoveDirection(moveDirection);
        }
    }

    private Vector3 GetFirstPersonMovement() {
        // In first-person mode, movement is relative to the player's local axes
        return transform.forward * player.inputHandler.moveComposite.y +
               transform.right * player.inputHandler.moveComposite.x;
    }

    private void FaceMoveDirection(Vector3 direction) {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    private Vector3 GetCameraRelativeMovement() {
        Vector3 cameraForward = new Vector3(player.mainCamera.forward.x, 0, player.mainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(player.mainCamera.right.x, 0, player.mainCamera.right.z).normalized;

        return cameraForward * player.inputHandler.moveComposite.y + cameraRight * player.inputHandler.moveComposite.x;
    }

    private void StartSprinting() {
        if (player.isCrouching) return;
        player.isSprinting = true;
        moveMult = player.sprintSpeedMult;
    }

    private void StopSprinting() {
        player.isSprinting = false;
        moveMult = player.normalSpeedMult;
    }

    private void Jump() {
        if (player.isGrounded) {
            jumpRequested = true;
            player.playerAnimation.TriggerJumpAnimation();
        }
    }

    public void ApplyJumpForce() {
        player.velocity.y = Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
        jumpRequested = false;
    }

    private void ApplyGravity() {
        player.velocity.y += player.gravity * Time.deltaTime;
        characterController.Move(player.velocity * Time.deltaTime);
    }

    private void ToggleCrouch() {
        player.isCrouching = !player.isCrouching;

        if (player.isCrouching) {
            characterController.height = player.crouchingHeight;
            moveMult = player.crouchSpeedMult;
        }
        else {
            characterController.height = player.standingHeight;
            moveMult = player.normalSpeedMult;
        }
        player.playerAnimation.TriggerCrouchAnimation(player.isCrouching);
    }
}
