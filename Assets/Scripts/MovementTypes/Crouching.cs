using UnityEngine;

public class Crouching : MovementType
{
    private Vector3 originalScale;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    private MeshFilter meshFilter;
    private Mesh capsuleMesh;
    private Mesh sphereMesh;
    private float originalColliderHeight;
    private float originalColliderRadius;

    public Crouching(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        capsuleCollider = controller.GetComponent<CapsuleCollider>();
        sphereCollider = controller.GetComponent<SphereCollider>();
        meshFilter = controller.GetComponent<MeshFilter>();
        capsuleMesh = playerController.Capsule;
        sphereMesh = playerController.Sphere;
        action.OnJumpGlobal += CrouchJump;
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
    }

    public override void UpdateMovement()
    {
        movement = new Vector3(playerAction.Movement.x, 0f, playerAction.Movement.y);
        if (!playerAction.IsCrouching)
        {
            playerController.SetMovement(playerController.RegularMovement);
        }
    }

    public void CrouchJump()
    {
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.CrouchJump);
        }
    }
    public override void FixedUpdateMovement()
    {
        Vector3 localMovement = new Vector3(movement.x, 0, movement.z);
        Vector3 worldMovement = playerTransform.TransformDirection(localMovement);
        Vector3 targetVelocity = playerController.WalkSpeed * worldMovement;
        targetVelocity.y = playerRigidbody.velocity.y;

        playerRigidbody.velocity = targetVelocity;
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

    ~Crouching()
    {
        playerAction.OnJumpGlobal -= CrouchJump;
    }
}
