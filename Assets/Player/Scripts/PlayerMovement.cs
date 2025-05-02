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
        player.inputHandler.onSprintAction += ToggleSprinting;
        player.inputHandler.onJumpAction += Jump;
        player.inputHandler.onCrouchAction += ToggleCrouch;
    }

    private void OnDestroy() {
        player.inputHandler.onSprintAction -= ToggleSprinting;
        player.inputHandler.onJumpAction -= Jump;
        player.inputHandler.onCrouchAction -= ToggleCrouch;
    }

    private void Update() {
        // Update grounded state
        player.isGrounded = characterController.isGrounded;

        // Handle movement only when grounded and no jump is requested
        if (player.isGrounded && !jumpRequested) {
            HandleMovement();
        }

        // Apply gravity to the player
        ApplyGravity();

        // Move the character controller using player.velocity
        characterController.Move(player.velocity * Time.deltaTime);
    }

    private void HandleMovement() {
        // Reset vertical velocity if grounded
        if (player.isGrounded && player.velocity.y < 0) {
            player.velocity.y = -2f;
        }

        // Determine movement direction based on the current camera mode
        Vector3 moveDirection = player.currentCameraMode == CameraMode.FirstPerson
            ? GetFirstPersonMovement()
            : GetCameraRelativeMovement();

        // Update player.velocity with the calculated movement direction
        player.velocity.x = moveDirection.x * player.moveSpeed * moveMult;
        player.velocity.z = moveDirection.z * player.moveSpeed * moveMult;

        // Stop sprinting if the player stops moving
        if (moveDirection.sqrMagnitude < 0.01f && player.isSprinting) {
            player.isSprinting = false;
            moveMult = player.normalSpeedMult;
        }

        // Rotate the player to face the movement direction only in third-person mode
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
        // Rotate the player smoothly to face the given direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    private Vector3 GetCameraRelativeMovement() {
        // Calculate movement direction relative to the camera's orientation
        Vector3 cameraForward = new Vector3(player.mainCamera.forward.x, 0, player.mainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(player.mainCamera.right.x, 0, player.mainCamera.right.z).normalized;

        return cameraForward * player.inputHandler.moveComposite.y + cameraRight * player.inputHandler.moveComposite.x;
    }

    private void ToggleSprinting() {
        if (player.isCrouching) {
            ToggleCrouch();
        }
        player.isSprinting = !player.isSprinting;
        moveMult = player.isSprinting ? player.sprintSpeedMult : player.normalSpeedMult;
    }

    private void Jump() {
        if (player.isGrounded) {
            if (player.isCrouching) {
                ToggleCrouch();
            }
            jumpRequested = true;
            player.playerAnimation.TriggerJumpAnimation();
        }
    }

    public void ApplyJumpForce() {
        player.velocity.y = Mathf.Sqrt(2 * player.jumpHeight * -player.gravity);
        jumpRequested = false;
    }

    private void ApplyGravity() {
        if (player.isGrounded && player.velocity.y < 0) {
            player.velocity.y = -2f; // Small value to keep the player grounded
        }
        else {
            player.velocity.y += player.gravity * Time.deltaTime;
        }
    }

    private void ToggleCrouch() {
        if (player.isSprinting) return;
        player.isCrouching = !player.isCrouching;

        float targetHeight = player.isCrouching ? player.crouchingHeight : player.standingHeight;
        float targetRadius = player.isCrouching ? player.crouchingRadius : player.standingRadius;
        Vector3 targetCenter = player.isCrouching ? player.crouchingCenter : player.standingCenter;
        Vector3 targetCameraPos = player.isCrouching ? player.crouchingCameraLocalPos : player.standingCameraLocalPos;


        characterController.height = targetHeight;
        characterController.center = targetCenter;
        characterController.radius = targetRadius;
        player.firstPersonCameraHolder.localPosition = Vector3.Lerp(
            player.firstPersonCameraHolder.localPosition,
            targetCameraPos,
            Time.deltaTime * player.cameraLerpSpeed
        );

        player.playerAnimation.TriggerCrouchAnimation(player.isCrouching);

        moveMult = player.isCrouching ? player.crouchSpeedMult : player.normalSpeedMult;
    }
}
