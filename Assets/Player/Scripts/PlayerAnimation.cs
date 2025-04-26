using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    private Player player;
    private Animator animator;
    private float velocityLerpSpeed;

    private static readonly int VelocityHash = Animator.StringToHash("Velocity"); // Hash for the velocity parameter
    private static readonly int JumpHash = Animator.StringToHash("Jump"); // Hash for the jump trigger
    private static readonly int CrouchEnterHash = Animator.StringToHash("CrouchEnter"); // Hash for the crouch enter trigger
    private static readonly int CrouchExitHash = Animator.StringToHash("CrouchExit"); // Hash for the crouch exit trigger
    private static readonly int IsFallingHash = Animator.StringToHash("IsFalling"); // Hash for the falling bool
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded"); // Hash for the grounded bool

    private float currentVelocity = 0f; // Smoothly interpolated velocity

    public void Initialize(Player player) {
        this.player = player;
        animator = player.animator;
        velocityLerpSpeed = player.velocityLerpSpeed;
    }

    private void Update() {
        UpdateVelocity();
        UpdateFallingState();
    }

    public void TriggerCrouchAnimation(bool isCrouching) {
        if (isCrouching) {
            animator.SetTrigger(CrouchEnterHash); // Trigger crouch enter animation
        }
        else {
            animator.SetTrigger(CrouchExitHash); // Trigger crouch exit animation
        }
    }

    private void UpdateVelocity() {
        Vector2 movementInput = player.inputHandler.moveComposite;
        float targetVelocity = movementInput.magnitude;

        if (player.isCrouching && targetVelocity > 0f) {
            targetVelocity = 1f; // Crouch walking
        }
        else if (player.isSprinting) {
            targetVelocity = 1f; // Running
        }
        else if (targetVelocity > 0f) {
            targetVelocity = 0.5f; // Walking
        }
        else {
            targetVelocity = 0f; // Idle
        }

        // Smoothly interpolate current velocity to target velocity
        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime * velocityLerpSpeed);

        // Update the Animator parameter
        animator.SetFloat(VelocityHash, currentVelocity);
    }

    private void UpdateFallingState() {
        // Update grounded and falling states
        animator.SetBool(IsGroundedHash, player.isGrounded);
        animator.SetBool(IsFallingHash, !player.isGrounded && player.velocity.y < 0);
    }

    public void TriggerJumpAnimation() {
        if (player.isGrounded) {
            animator.SetTrigger(JumpHash); // Trigger the jump animation
        }
    }
}
