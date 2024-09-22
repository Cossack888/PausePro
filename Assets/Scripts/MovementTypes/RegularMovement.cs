using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RegularMovement : MovementType
{

    private FocusedObjectFinder focusedObjectFinder;

    public RegularMovement(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        // action.OnJumpGlobal += Jump;
        // //action.OnAttackGlobal += Attack;
        // action.OnInteractGlobal += Push;
        // action.OnGhostGlobal += Ghost;

        focusedObjectFinder= new FocusedObjectFinder(controller, transform);
    }
    public override void EnterMovement()
    {
        playerAction.OnJumpGlobal += Jump;
        //playerAction.OnAttackGlobal += Attack;
        //playerAction.OnInteractGlobal += Push;
        playerAction.OnShootGlobal += Push;
        playerAction.OnGhostGlobal += Ghost;

        playerRigidbody.velocity = new Vector3(0, 0, 0);
    }

    public override void ExitMovement()
    {
        playerAction.OnJumpGlobal -= Jump;
        //playerAction.OnAttackGlobal -= Attack;
        //playerAction.OnInteractGlobal -= Push;
        playerAction.OnShootGlobal -= Push;
        playerAction.OnGhostGlobal -= Ghost;
    }

    public override void UpdateMovement()
    {
        movement = new Vector3(playerAction.Movement.x, 0f, playerAction.Movement.y);
        if (playerAction.IsCrouching && !crouched)
        {
            crouched = true;
            playerController.SetMovement(playerController.Crouching);
        }
        else if (!playerAction.IsCrouching && crouched)
        {
            crouched = false;
        }
    }
    public override void FixedUpdateMovement()
    {
        Vector3 localMovement = new Vector3(movement.x, 0, movement.z);
        Vector3 worldMovement = playerTransform.TransformDirection(localMovement);
        Vector3 targetVelocity = (playerAction.IsSprinting ? playerController.RunSpeed : playerController.WalkSpeed) * worldMovement;
        targetVelocity.y = playerRigidbody.velocity.y;
        playerRigidbody.velocity = targetVelocity;
    }

    public void Push()
    {
        if (playerController.CurrentMovement == this)
        {
            ApplyForce();
        }
    }

    public void ApplyForce()
    {
        
        GameObject objectInFocus = focusedObjectFinder.FindObjectInFocus();

        EnemyAI enemy = objectInFocus.GetComponent<EnemyAI>();

        if (enemy != null)
        {
<<<<<<< HEAD
            InteractionObject hitObject = hit.collider.gameObject.GetComponent<InteractionObject>();
            if (hitObject != null && !hitObject.hasBeenPushed)
            {
                NavMeshAgent navMeshAgent = hit.collider.GetComponent<NavMeshAgent>();
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                playerController.LeftHand.SetTrigger("push");
                rb.isKinematic = false;
                if (navMeshAgent != null)
                {
                    navMeshAgent.enabled = false;
                }
                if (enemy != null)
                {
                    enemy.enabled = false;
                }
                Vector3 forceDirection = (hit.point - start).normalized;
                rb.AddForceAtPosition(forceDirection * 10, hit.point, ForceMode.Impulse);

                hitObject.Push();
            }
=======
            Vector3 directionToTarget = enemy.transform.position - playerController.NormalCam.transform.position;
            enemy.ApplyForce(directionToTarget, enemy.transform.position);
>>>>>>> 1a1e0ed (Use loose aim in regular form)
        }

        // Vector3 start = playerTransform.position;
        // Vector3 direction = playerController.Cam.transform.forward;

        // if (Physics.Raycast(start, direction, out RaycastHit hit, playerController.GhostInteractionDistance, playerController.GhostInteractionLayer))
        // {
        //     InteractionObject hitObject = hit.collider.gameObject.GetComponent<InteractionObject>();
        //     if (hitObject != null && !hitObject.hasBeenPushed)
        //     {
        //         NavMeshAgent navMeshAgent = hit.collider.GetComponent<NavMeshAgent>();
        //         EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
        //         Rigidbody rb = hitObject.GetComponent<Rigidbody>();
        //         rb.isKinematic = false;
        //         if (navMeshAgent != null)
        //         {
        //             navMeshAgent.enabled = false;
        //         }
        //         if (enemy != null)
        //         {
        //             enemy.enabled = false;
        //         }
        //         Vector3 forceDirection = (hit.point - start).normalized;
        //         rb.AddForceAtPosition(forceDirection * 10, hit.point, ForceMode.Impulse);

        //         hitObject.Push();
        //     }
        // }
    }

    public void Jump()
    {
        if (IsGrounded() && playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.Jumping);
        }
    }

    public void Attack()
    {
        playerController.RightHand.SetTrigger("Slash");
    }
    public void Ghost()
    {
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.GhostForm);
        }
    }

    ~RegularMovement()
    {
        playerAction.OnJumpGlobal -= Jump;
        //playerAction.OnAttackGlobal -= Attack;
        //playerAction.OnInteractGlobal -= Push;
        playerAction.OnShootGlobal -= Push;
        playerAction.OnGhostGlobal -= Ghost;
    }
}