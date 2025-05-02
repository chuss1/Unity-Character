using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    private Player player;
    private Animator animator;
    private float velocityLerpSpeed;

    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int CrouchEnterHash = Animator.StringToHash("CrouchEnter");
    private static readonly int CrouchExitHash = Animator.StringToHash("CrouchExit");
    private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private float currentVelocity = 0f;

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
            animator.SetTrigger(CrouchEnterHash);
        }
        else {
            animator.SetTrigger(CrouchExitHash);
        }
    }

    private void UpdateVelocity() {
        Vector3 movementInput = new Vector3(player.velocity.x, 0, player.velocity.z);
        float targetVelocity = movementInput.magnitude;

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

        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime * velocityLerpSpeed);
        animator.SetFloat(VelocityHash, currentVelocity);
    }

    private void UpdateFallingState() {
        animator.SetBool(IsGroundedHash, player.isGrounded);
        animator.SetBool(IsFallingHash, !player.isGrounded && player.velocity.y < 0);
    }

    public void TriggerJumpAnimation() {
        if (player.isGrounded) {
            animator.SetTrigger(JumpHash);
        }
    }
}
