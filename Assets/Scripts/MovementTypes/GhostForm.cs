using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GhostForm : MovementType
{
    float timer;
    public Vector3 previousPosition;
    private GameObject playerBody;
    private List<ForceData> savedForces = new List<ForceData>();
    private List<TurnOff> savedTurnOffs = new List<TurnOff>();
    public GhostForm(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        action.OnParkourGlobal += ApplyForce;
        action.OnGhostGlobal += LeaveGhostForm;
        action.OnAttackGlobal += Attack;
    }

    public override void EnterMovement()
    {
        previousPosition = playerTransform.position;
        playerController.CreatePlayerBody(previousPosition);
        timer = 0;
        playerRigidbody.useGravity = false;
        playerController.GhostCam.gameObject.SetActive(true);
        playerController.NormalCam.gameObject.SetActive(false);
        playerController.SwitchCamera(playerController.GhostCam);
        foreach (Animator anim in GameObject.FindObjectsOfType<Animator>())
        {
            if (anim.gameObject.GetComponent<EnemyAI>() != null)
            {
                anim.SetBool("Stopped", true);
            }
        }
        foreach (EnemyAI enemy in GameObject.FindObjectsOfType<EnemyAI>())
        {
            enemy.isStationary = true;
        }
    }

    public override void FixedUpdateMovement()
    {
        Vector3 cameraForward = playerController.Cam.transform.forward;
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
    public void Attack()
    {
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.GhostAttack);
        }

    }
    public override void ExitMovement()
    {
        playerRigidbody.useGravity = true;
        foreach (Animator anim in GameObject.FindObjectsOfType<Animator>())
        {
            if (anim.gameObject.GetComponent<EnemyAI>() != null)
            {
                anim.SetBool("Stopped", false);
                anim.SetBool("Back", false);
            }
        }
        foreach (EnemyAI enemy in GameObject.FindObjectsOfType<EnemyAI>())
        {
            enemy.UnpauseEnemy();
            //enemy.ReenableNavMeshAgent();
        }
        playerTransform.position = previousPosition;
        playerController.DestroyPlayerBody();
    }

    public void ApplyForce()
    {
        Vector3 start = playerTransform.position;
        Vector3 direction = playerController.Cam.transform.forward;
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
        Vector3 direction = playerController.Cam.transform.forward;
        Debug.DrawRay(start, direction * playerController.GhostInteractionDistance, Color.red);

        if (Physics.Raycast(start, direction, out RaycastHit hit, playerController.GhostInteractionDistance, playerController.GhostInteractionLayer))
        {
            TurnOff turnOff = hit.collider.GetComponent<TurnOff>();
            InteractionObject hitObject = hit.collider.gameObject.GetComponent<InteractionObject>();

            if (turnOff != null && turnOff.on == true)
            {
                turnOff.on = false;
                savedTurnOffs.Add(turnOff);
                Debug.Log(savedTurnOffs.Count);
            }

            if (hitObject != null && !hitObject.hasBeenPushed)
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
            forceData.rb.AddForceAtPosition(forceData.forceDirection * 30, forceData.hitPoint, ForceMode.Impulse);
            forceData.interactionObject.Push();
        }
        foreach (TurnOff turnOff in savedTurnOffs)
        {
            if (turnOff.on == false)
            {
                turnOff.Unhook();
            }
        }
        savedForces.Clear();
        savedTurnOffs.Clear();
    }

    public void LeaveGhostForm()
    {
        if (playerController.CurrentMovement == this && timer > 1)
        {
            playerController.SetMovement(playerController.RegularMovement);
            playerController.GhostCam.gameObject.SetActive(false);
            playerController.NormalCam.gameObject.SetActive(true);
            playerController.SwitchCamera(playerController.NormalCam);
        }
    }

    ~GhostForm()
    {
        playerAction.OnParkourGlobal -= ApplyForce;
        playerAction.OnGhostGlobal -= LeaveGhostForm;
        playerAction.OnAttackGlobal -= Attack;
    }
}
