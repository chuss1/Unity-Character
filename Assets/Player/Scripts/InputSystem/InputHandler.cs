using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, PlayerInput.IPlayerBaseActions {
    public Vector2 lookDelta;
    public Vector2 moveComposite;

    public Action onPOVToggleAction;
    public Action onJumpAction;
    public Action onSprintStartAction;
    public Action onSprintStopAction;
    public Action onCrouchAction;

    private PlayerInput playerInput;

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
        moveComposite = context.ReadValue<Vector2>();
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
            onSprintStartAction?.Invoke();
        }
        else if (context.canceled) {
            onSprintStopAction?.Invoke();
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

    }
}
