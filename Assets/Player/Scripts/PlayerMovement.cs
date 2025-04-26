using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Player player;
    private CharacterController characterController;
    private float moveMult;
    private float verticalRotation = 0f;
    private bool jumpRequested = false;


    private void Start()
    {
        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();
        moveMult = player.normalSpeedMult;

        player.inputHandler.onSprintStartAction += StartSprinting;
        player.inputHandler.onSprintStopAction += StopSprinting;
        player.inputHandler.onJumpAction += Jump;
        player.inputHandler.onCrouchAction += ToggleCrouch;
    }

    private void OnDestroy()
    {
        player.inputHandler.onSprintStartAction -= StartSprinting;
        player.inputHandler.onSprintStopAction -= StopSprinting;
        player.inputHandler.onJumpAction -= Jump;
        player.inputHandler.onCrouchAction -= ToggleCrouch;
    }

    private void Update()
    {
        player.isGrounded = characterController.isGrounded;
        if (player.isGrounded && !jumpRequested)
        {
            HandleMovement();
        }
        HandleLook();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        if (player.isGrounded && player.velocity.y < 0)
        {
            player.velocity.y = -2f;
        }

        Vector3 moveDirection = player.currentCameraMode == CameraMode.FirstPerson
            ? transform.forward * player.inputHandler.moveComposite.y + transform.right * player.inputHandler.moveComposite.x
            : GetCameraRelativeMovement();

        characterController.Move(moveDirection * player.moveSpeed * moveMult * Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0.01f && player.currentCameraMode == CameraMode.ThirdPerson)
        {
            FaceMoveDirection(moveDirection);
        }
    }

    private Vector3 GetCameraRelativeMovement()
    {
        Vector3 cameraForward = new Vector3(player.mainCamera.forward.x, 0, player.mainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(player.mainCamera.right.x, 0, player.mainCamera.right.z).normalized;

        return cameraForward * player.inputHandler.moveComposite.y + cameraRight * player.inputHandler.moveComposite.x;
    }

    private void FaceMoveDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    private void HandleLook()
    {
        Vector2 look = player.inputHandler.lookDelta;

        if (player.currentCameraMode == CameraMode.FirstPerson)
        {
            transform.Rotate(Vector3.up * look.x * player.lookSensitivity);

            verticalRotation -= look.y * player.lookSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            player.firstPersonCameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
        else if (player.currentCameraMode == CameraMode.ThirdPerson)
        {
            // Usually, Cinemachine handles rotation in third-person mode
            // If you want manual control, you can manipulate Cinemachine's FreeLook axis input here
        }
    }

    private void StartSprinting()
    {
        if (player.isCrouching) return;
        player.isSprinting = true;
        moveMult = player.sprintSpeedMult;
    }

    private void StopSprinting()
    {
        player.isSprinting = false;
        moveMult = player.normalSpeedMult;
    }

    private void Jump()
    {
        if (player.isGrounded)
        {
            jumpRequested = true; 
            player.playerAnimation.TriggerJumpAnimation();
        }
    }

    public void ApplyJumpForce()
    {
        player.velocity.y = Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
        jumpRequested = false;
    }

    private void ApplyGravity()
    {
        player.velocity.y += player.gravity * Time.deltaTime;
        characterController.Move(player.velocity * Time.deltaTime);
    }

    private void ToggleCrouch()
    {
        player.isCrouching = !player.isCrouching;

        if (player.isCrouching)
        {
            characterController.height = player.crouchingHeight; // Reduce height
            moveMult = player.crouchSpeedMult; // Reduce movement speed
        }
        else
        {
            characterController.height = player.standingHeight; // Restore height
            moveMult = player.normalSpeedMult; // Restore movement speed
        }

        // Trigger the crouch animation
        player.playerAnimation.TriggerCrouchAnimation(player.isCrouching);
    }
}
