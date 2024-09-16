using UnityEngine;

public class Crouching : MovementType
{
    private Vector3 originalScale;
    private Quaternion originalRotation;
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
    }

    public override void EnterMovement()
    {
        if (capsuleCollider != null)
        {
            originalColliderHeight = capsuleCollider.height;
            originalColliderRadius = capsuleCollider.radius;
            originalRotation = playerTransform.rotation;
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
            float newScale = sphereColliderRadius * 1.5f;
            playerTransform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }

    public override void UpdateMovement()
    {
        movement = new Vector3(playerAction.Movement.x, 0f, playerAction.Movement.y);
        momentum.ModifyMomentum(-Time.deltaTime / 5);
        if (!playerAction.IsCrouching || momentum.CurrentMomentum == 0)
        {
            playerController.SetMovement(playerController.RegularMovement);
        }
    }

    public override void FixedUpdateMovement()
    {
        Vector3 targetVelocity = ((playerController.RunSpeed + momentum.CurrentMomentum) / 2) * new Vector3(movement.x, 0, movement.z);
        targetVelocity.y = playerRigidbody.velocity.y;

        playerRigidbody.velocity = targetVelocity;

        RollPlayer();
    }

    public override void ExitMovement()
    {
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
        playerTransform.rotation = originalRotation;
    }

    private float rollAngle = 0f;

    private void RollPlayer()
    {
        if (movement.sqrMagnitude > 0.01f)
        {
            Vector3 movementDirection = new Vector3(movement.x, 0f, movement.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            rollAngle += playerController.RollSpeed * Time.deltaTime;
            playerTransform.rotation = targetRotation * Quaternion.Euler(rollAngle, 0, 0);
        }
    }
}
