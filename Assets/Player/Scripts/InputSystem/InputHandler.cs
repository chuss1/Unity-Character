using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, PlayerInput.IPlayerBaseActions {
    public Vector2 lookDelta;
    public Vector2 moveComposite;

    public Action onPOVToggleAction;
    public Action onJumpAction;
    public Action onSprintAction;
    public Action onCrouchAction;

    private Player player;
    private PlayerInput playerInput;

    private void Awake() {
        player = GetComponent<Player>();
    }

    private void OnEnable() {
        if (playerInput != null)
            return;

        playerInput = new PlayerInput();
        playerInput.PlayerBase.SetCallbacks(this);
        playerInput.PlayerBase.Enable();
    }

    private void OnDisable() {
        playerInput.PlayerBase.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context) {
        // Update player.velocity based on input
        Vector2 moveInput = context.ReadValue<Vector2>();
        player.velocity = new Vector3(moveInput.x, player.velocity.y, moveInput.y); // Map input to velocity
    }

    public void OnLook(InputAction.CallbackContext context) {
        lookDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.performed) {
            onJumpAction?.Invoke();
        }
    }

    public void OnSprint(InputAction.CallbackContext context) {
        if (context.performed) {
            onSprintAction?.Invoke();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context) {
        if (context.performed) {
            onCrouchAction?.Invoke();
        }
    }

    public void OnPOVToggle(InputAction.CallbackContext context) {
        if (context.performed) {
            onPOVToggleAction?.Invoke();
        }
    }

    public void OnZoom(InputAction.CallbackContext context) {
        // Handle zoom input if needed
    }
}
