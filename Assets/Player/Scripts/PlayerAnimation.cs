using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    private Player player;
    private Animator animator;
    private float velocityLerpSpeed;
    private bool isLanding = false;

    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int CrouchEnterHash = Animator.StringToHash("CrouchEnter");
    private static readonly int CrouchExitHash = Animator.StringToHash("CrouchExit");
    private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int StartLandHash = Animator.StringToHash("StartLand");

    private float currentVelocity = 0f;
    private float landingDistance = .8f;

    public void Initialize(Player player) {
        this.player = player;
        animator = player.animator;
        velocityLerpSpeed = player.velocityLerpSpeed;
    }

    private void Update() {
        UpdateVelocity();
        UpdateFallingState();
        CheckForLanding();
    }

    public void TriggerCrouchAnimation(bool isCrouching) {
        if (isCrouching) {
            animator.SetTrigger(CrouchEnterHash);
        }
        else {
            animator.SetTrigger(CrouchExitHash);
        }
    }

    private void UpdateVelocity() {
        // Calculate horizontal velocity (X and Z only)
        Vector3 horizontalVelocity = new Vector3(player.velocity.x, 0, player.velocity.z);
        float targetVelocity = horizontalVelocity.magnitude;

        // Adjust target velocity based on player state
        if (player.isCrouching && targetVelocity > 0f) {
            targetVelocity = 1f;
        }
        else if (player.isSprinting) {
            targetVelocity = 1f;
        }
        else if (targetVelocity > 0f) {
            targetVelocity = 0.5f;
        }
        else {
            targetVelocity = 0f;
        }

        // Smoothly interpolate the velocity for animation
        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime * velocityLerpSpeed);
        animator.SetFloat(VelocityHash, currentVelocity);
    }

    private void UpdateFallingState() {
        // Update grounded and falling states based on vertical velocity (Y)
        animator.SetBool(IsGroundedHash, player.isGrounded);
        animator.SetBool(IsFallingHash, !player.isGrounded && player.velocity.y < 0);
    }

    private void CheckForLanding() {
        // Only check for landing if the player is falling
        if (!player.isGrounded && player.velocity.y < 0 && !isLanding) {
            // Draw the raycast in the Scene view for debugging
            Debug.DrawRay(transform.position, Vector3.down * landingDistance, Color.red);

            // Perform a raycast to check the distance to the ground
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, landingDistance)) {
                // Ensure the player is falling vertically enough to trigger the landing
                if (Mathf.Abs(player.velocity.y) > 0.5f) { // Adjust the threshold as needed
                    isLanding = true;
                    // Trigger the landing animation if within the landing distance
                    animator.SetTrigger(StartLandHash);
                }
            }
        }

        // Reset the landing state when grounded
        if (player.isGrounded) {
            isLanding = false;
        }
    }

    public void TriggerJumpAnimation() {
        if (player.isGrounded) {
            animator.SetTrigger(JumpHash);
        }
    }
}
