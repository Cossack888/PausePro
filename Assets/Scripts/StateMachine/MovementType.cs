using UnityEngine;

public abstract class MovementType : IMovement
{
    protected Rigidbody playerRigidbody;
    protected Transform playerTransform;
    protected PlayerController playerController;
    protected PlayerAction playerAction;
    protected Momentum momentum;
    protected Vector3 movement;

    public MovementType(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action)
    {
        playerRigidbody = rb;
        playerTransform = transform;
        playerController = controller;
        playerAction = action;
        momentum = GameObject.FindObjectOfType<Momentum>();

    }

    public virtual void EnterMovement() { }
    public virtual void UpdateMovement() { }
    public virtual void FixedUpdateMovement() { }
    public virtual void ExitMovement() { }

    protected bool IsGrounded()
    {
        float sphereRadius = 0.2f;
        bool groundCheck = Physics.OverlapSphere(playerController.GroundCheck.position, sphereRadius, playerController.GroundMask).Length > 0;
        return groundCheck;
    }

    protected bool StuckToLeftSide()
    {
        bool isStuckBack = Physics.Raycast(playerTransform.position, -playerTransform.forward, out RaycastHit hitForward, playerController.WallDistance, playerController.WallMask);
        bool isStuckLeft = Physics.Raycast(playerTransform.position, -playerTransform.right, out RaycastHit hitRight, playerController.WallDistance, playerController.WallMask);
        return isStuckBack || isStuckLeft;
    }
    protected bool StuckToRightSide()
    {
        bool isStuckForward = Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit hitForward, playerController.WallDistance, playerController.WallMask);
        bool isStuckRight = Physics.Raycast(playerTransform.position, playerTransform.right, out RaycastHit hitRight, playerController.WallDistance, playerController.WallMask);
        return isStuckForward || isStuckRight;
    }
    protected bool Falling()
    {
        return playerRigidbody.velocity.y < 0;
    }

    protected void HandleRotation()
    {
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.forward);
            targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0);
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, playerController.RotationSpeed * Time.deltaTime);
        }
    }


}
