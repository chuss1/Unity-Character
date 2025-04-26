using UnityEngine;

public class JumpAnimTrigger : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    public void ApplyJumpPhysics()
    {
        player.playerMovement.ApplyJumpForce(); // Call the method in PlayerMovement to apply the jump force
    }
}
