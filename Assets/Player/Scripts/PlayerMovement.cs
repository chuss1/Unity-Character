using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    private Player player;
    private CharacterController characterController;
    private float moveMult;
    private bool jumpRequested = false;
    [SerializeField] private Vector3 moveVelocity;

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

        player.velocity = new Vector3(player.inputHandler.moveComposite.x, player.velocity.y, player.inputHandler.moveComposite.y);

        Vector3 moveDirection = player.currentCameraMode == CameraMode.FirstPerson
            ? GetFirstPersonMovement(player.velocity)
            : GetCameraRelativeMovement(player.velocity);

        moveVelocity = moveDirection * player.moveSpeed * moveMult;
        characterController.Move(moveVelocity * Time.deltaTime);

        if (player.currentCameraMode == CameraMode.ThirdPerson && moveDirection.sqrMagnitude > 0.01f) {
            FaceMoveDirection(moveDirection);
        }
    }

    private void FaceMoveDirection(Vector3 direction) {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    private Vector3 GetFirstPersonMovement(Vector3 velocity) {
        return transform.forward * velocity.z + transform.right * velocity.x;
    }

    private Vector3 GetCameraRelativeMovement(Vector3 velocity) {
        Vector3 cameraForward = new Vector3(player.mainCamera.forward.x, 0, player.mainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(player.mainCamera.right.x, 0, player.mainCamera.right.z).normalized;

        Vector3 move = cameraForward * velocity.z + cameraRight * velocity.x;
        Debug.Log("Camera forward: " + cameraForward + ", Camera right: " + cameraRight);
        Debug.Log("Move direction: " + move);

        return move.magnitude > 1f ? move.normalized : move;
    }

    private void ToggleSprinting() {
        if (player.isCrouching) return;

        player.isSprinting = !player.isSprinting;
        moveMult = player.isSprinting ? player.sprintSpeedMult : player.normalSpeedMult;
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
            characterController.center = new Vector3(0, 0.6f, 0);
            characterController.radius = 0.55f;
            moveMult = player.crouchSpeedMult;
        }
        else {
            characterController.height = player.standingHeight;
            characterController.center = new Vector3(0, 1f, 0);
            characterController.radius = 0.25f;
            moveMult = player.normalSpeedMult;
        }
        player.playerAnimation.TriggerCrouchAnimation(player.isCrouching);
    }
}
