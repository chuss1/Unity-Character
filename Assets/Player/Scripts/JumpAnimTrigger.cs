using UnityEngine;

public class JumpAnimTrigger : MonoBehaviour {
    private Player player;

    private void Start() {
        player = GetComponentInParent<Player>();
    }

    public void ApplyJumpPhysics() {
        player.playerMovement.ApplyJumpForce();
    }

    public void DeactivateLanding() {
        player.playerAnimation.DeactivateLanding();
    }
}
