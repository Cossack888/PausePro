using UnityEngine;

public class CrouchJump : MovementType
{
    private Vector3 originalScale;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    private MeshFilter meshFilter;
    private Mesh capsuleMesh;
    private Mesh sphereMesh;
    private float originalColliderHeight;
    private float originalColliderRadius;
    private float initialYPosition;
    private float targetYPosition;
    private float currentYVelocity;
    private bool isAscending;
    private bool isAtPeak;
    private bool landed;
    public CrouchJump(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        action.OnDashGlobal += Dash;
        controller.OnLand += Landed;
        capsuleCollider = controller.GetComponent<CapsuleCollider>();
        sphereCollider = controller.GetComponent<SphereCollider>();
        meshFilter = controller.GetComponent<MeshFilter>();
        capsuleMesh = playerController.Capsule;
        sphereMesh = playerController.Sphere;
    }

    public override void EnterMovement()
    {
        if (capsuleCollider != null)
        {
            originalColliderHeight = capsuleCollider.height;
            originalColliderRadius = capsuleCollider.radius;
            capsuleCollider.enabled = false;
            float sphereColliderRadius = originalColliderRadius;
            if (sphereCollider != null)
            {
                sphereCollider.radius = sphereColliderRadius;
                sphereCollider.enabled = true;
            }
            if (meshFilter != null)
            {
                meshFilter.mesh = sphereMesh;
            }
            originalScale = playerTransform.localScale;
            float newScale = sphereColliderRadius * 0.5f;
            playerTransform.localScale = new Vector3(newScale, newScale, newScale);
        }
        initialYPosition = playerRigidbody.position.y;
        targetYPosition = initialYPosition + playerController.JumpHeight;
        currentYVelocity = 0f;
        isAscending = true;
        isAtPeak = false;
        landed = true;
    }
    public override void UpdateMovement()
    {
        if (IsGrounded() && Falling() && !landed)
        {
            Landed();
        }

        movement = new Vector3(playerAction.Movement.x, 0f, playerAction.Movement.y);

        if (!IsGrounded())
        {
            movement *= playerController.AirControlFactor;
        }

    }
    public override void ExitMovement()
    {
        crouched = false;
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
        }
        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.radius = originalColliderRadius;
            capsuleCollider.enabled = true;
        }
        if (meshFilter != null)
        {
            meshFilter.mesh = capsuleMesh;
        }
        playerTransform.localScale = originalScale;
    }
    public void Landed()
    {
        landed = true;
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.Crouching);
        }
    }

    public void Dash()
    {
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.Dash);
        }
    }

    public override void FixedUpdateMovement()
    {
        Vector3 localMovement = new Vector3(movement.x, 0, movement.z);
        Vector3 worldMovement = playerTransform.TransformDirection(localMovement);
        Vector3 targetVelocity = playerController.RunSpeed * worldMovement;
        targetVelocity.y = playerRigidbody.velocity.y;
        if (isAscending)
        {
            currentYVelocity += playerController.JumpForce * Time.deltaTime;
            if (playerRigidbody.position.y >= targetYPosition)
            {
                isAscending = false;
                isAtPeak = true;
            }
        }
        else if (isAtPeak)
        {
            landed = false;
            currentYVelocity -= playerController.JumpForce * Time.deltaTime;
            if (currentYVelocity <= 0)
            {
                isAtPeak = false;
            }
        }
        else
        {
            currentYVelocity -= playerController.JumpForce * Time.deltaTime * 2;
        }
        targetVelocity.y = currentYVelocity;
        playerRigidbody.velocity = targetVelocity;
    }

    ~CrouchJump()
    {

        playerAction.OnDashGlobal -= Dash;
        playerController.OnLand -= Landed;
    }
}