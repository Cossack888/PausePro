using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GhostForm : MovementType
{
    float timer;
    private List<ForceData> savedForces = new List<ForceData>();

    public GhostForm(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        action.OnParkourGlobal += ApplyForce;
        action.OnGhostGlobal += LeaveGhostForm;
    }

    public override void EnterMovement()
    {
        timer = 0;
        playerRigidbody.useGravity = false;
    }

    public override void FixedUpdateMovement()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 localMovement = new Vector3(movement.x, 0, 0);
        Vector3 worldMovement = playerTransform.TransformDirection(localMovement);
        Vector3 verticalMovement = cameraForward * movement.z;
        Vector3 targetVelocity = (playerAction.IsSprinting ? playerController.RunSpeed + momentum.CurrentMomentum : playerController.WalkSpeed + momentum.CurrentMomentum) * (worldMovement + verticalMovement);
        playerRigidbody.velocity = targetVelocity;
    }

    public override void UpdateMovement()
    {
        timer += Time.deltaTime;
        movement = new Vector3(playerAction.Movement.x, 0f, playerAction.Movement.y);
        if (Input.GetKeyDown(KeyCode.E))
        {
            SaveForce();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ApplySavedForces();
        }
    }

    public override void ExitMovement()
    {
        playerRigidbody.useGravity = true;
    }

    public void ApplyForce()
    {
        Vector3 start = playerTransform.position;
        Vector3 direction = Camera.main.transform.forward;
        Debug.DrawRay(start, direction * playerController.GhostInteractionDistance, Color.red);

        if (Physics.Raycast(start, direction, out RaycastHit hit, playerController.GhostInteractionDistance, playerController.GhostInteractionLayer))
        {
            Debug.DrawLine(start, hit.point, Color.green);
            InteractionObject hitObject = hit.collider.gameObject.GetComponent<InteractionObject>();
            if (!hitObject.hasBeenPushed)
            {
                NavMeshAgent navMeshAgent = hit.collider.GetComponent<NavMeshAgent>();
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
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
        }
    }

    public void SaveForce()
    {
        Vector3 start = playerTransform.position;
        Vector3 direction = Camera.main.transform.forward;
        Debug.DrawRay(start, direction * playerController.GhostInteractionDistance, Color.red);

        if (Physics.Raycast(start, direction, out RaycastHit hit, playerController.GhostInteractionDistance, playerController.GhostInteractionLayer))
        {
            InteractionObject hitObject = hit.collider.gameObject.GetComponent<InteractionObject>();
            if (!hitObject.hasBeenPushed)
            {
                NavMeshAgent navMeshAgent = hit.collider.GetComponent<NavMeshAgent>();
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                Vector3 forceDirection = (hit.point - start).normalized;
                savedForces.Add(new ForceData(rb, hitObject, forceDirection, hit.point, navMeshAgent, enemy));
                Debug.Log(savedForces.Count);
            }
        }
    }

    public void ApplySavedForces()
    {
        foreach (ForceData forceData in savedForces)
        {
            if (forceData.navMeshAgent != null)
            {
                forceData.navMeshAgent.enabled = false;
            }
            if (forceData.enemy != null)
            {
                forceData.enemy.enabled = false;
            }
            forceData.rb.isKinematic = false;
            forceData.rb.AddForceAtPosition(forceData.forceDirection * 10, forceData.hitPoint, ForceMode.Impulse);
            forceData.interactionObject.Push();
        }
        savedForces.Clear();
    }

    public void LeaveGhostForm()
    {
        if (playerController.CurrentMovement == this && timer > 1)
        {
            playerController.SetMovement(playerController.RegularMovement);
        }
    }

    ~GhostForm()
    {
        playerAction.OnParkourGlobal -= ApplyForce;
        playerAction.OnGhostGlobal -= LeaveGhostForm;
    }
}
