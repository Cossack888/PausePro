using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class GhostForm : MovementType
{
    float timer;
    float time;
    private float timeElapsed;
    private bool dashTimer;
    private float cooldown;
    public Vector3 previousPosition;
    private GameObject playerBody;
    private List<ForceData> savedForces = new List<ForceData>();
    private List<TurnOff> savedTurnOffs = new List<TurnOff>();
    private List<BreakableObject> explosions = new List<BreakableObject>();
    private List<GameObject> redObjects = new List<GameObject>();
    private GameObject highlightedObject;
    private GameManager gameManager;
    public GhostForm(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        action.OnParkourGlobal += InitializeDash;
        action.OnGhostGlobal += LeaveGhostForm;
        action.OnAttackGlobal += Attack;
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public override void EnterMovement()
    {
        cooldown = gameManager.GetBottles() * 10;
        gameManager.ChangeAmountOfBottles(-gameManager.GetBottles());
        previousPosition = playerTransform.position;
        playerController.CreatePlayerBody(previousPosition);
        timer = 0;
        playerRigidbody.useGravity = false;
        playerController.GhostCam.gameObject.SetActive(true);
        playerController.NormalCam.gameObject.SetActive(false);
        playerController.SwitchCamera(playerController.GhostCam);
        AudioManager.instance.SwitchToAlbum("ghost");
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
        HighlightStuff();
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
        timeElapsed += Time.deltaTime;
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            LeaveGhostForm();
        }
        FindObjectInFocus();
        if (dashTimer == true)
        {
            if (timeElapsed > 0.5)
            {
                playerRigidbody.velocity = Vector3.zero;
                dashTimer = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SaveForce();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ApplySavedForces();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            TransportObjectToPlayer();
        }

    }

    private void InitializeDash()
    {
        if (playerController.CurrentMovement == this)
        {
            dashTimer = true;
            timeElapsed = 0;
            playerRigidbody.AddForce(playerController.Cam.forward * playerController.DashForce * (1 + momentum.CurrentMomentum / 5));
            momentum.ModifyMomentum(-0.1f);
        }

    }
    public void Attack()
    {
        playerController.GhostRightHand.SetTrigger("Slash");
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
        ApplySavedForces();
        AudioManager.instance.SwitchToAlbum("flesh");
    }

    public void FindObjectInFocus()
    {
        if (Physics.Raycast(playerController.Cam.position, playerController.Cam.forward, out RaycastHit hit, Mathf.Infinity, playerController.GhostInteractionLayer))
        {
            GameObject objectInFocus = hit.collider.gameObject;
            if (highlightedObject != null && highlightedObject != objectInFocus)
            {
                TurnColor(Color.green, highlightedObject);
            }
            TurnColor(Color.blue, objectInFocus);
            highlightedObject = objectInFocus;
        }
        else
        {
            if (highlightedObject != null)
            {
                TurnColor(Color.green, highlightedObject);
                highlightedObject = null;
            }
        }
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
            if (hitObject != null && !hitObject.hasBeenPushed)
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
            BreakableObject breakable = hit.collider.GetComponent<BreakableObject>();
            GameObject hitData = hit.collider.gameObject;

            TurnColor(Color.red, hitData);
            redObjects.Add(hitData);
            if (breakable != null)
            {
                breakable.RiggExplosion();
                explosions.Add(breakable);
            }

            if (turnOff != null && turnOff.on == true)
            {
                turnOff.Turn();
                savedTurnOffs.Add(turnOff);
                Debug.Log("TurnoFF:     " + savedTurnOffs.Count);
            }

            if (hitObject != null && !hitObject.hasBeenPushed)
            {

                NavMeshAgent navMeshAgent = hit.collider.GetComponent<NavMeshAgent>();
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                Vector3 forceDirection = (hit.point - start).normalized;
                savedForces.Add(new ForceData(rb, hitObject, forceDirection, hit.point, navMeshAgent, enemy));
                Debug.Log("Forces:    " + savedForces.Count);
            }
        }
    }

    public void ApplySavedForces()
    {

        while (time < 1)
        {
            time += Time.deltaTime;
        }
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
        foreach (BreakableObject breakable in explosions)
        {
            if (breakable.riggedToDestroy)
            {
                breakable.FallApart();
            }
        }
        savedForces.Clear();
        savedTurnOffs.Clear();
    }
    public void TransportObjectToPlayer()
    {
        LayerMask mask = Physics.AllLayers;
        if (Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit hit, Mathf.Infinity, mask))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (ObjectProneToInteraction(hitObject))
            {
                hitObject.transform.position = playerTransform.position + playerTransform.forward;
                TurnColor(Color.white, hitObject);
            }
        }
    }
    void HighlightStuff()
    {
        Debug.Log("Running the HighlightStuff action");
        float sphereRadius = 10000.0f;
        LayerMask targetMask = Physics.AllLayers;
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, sphereRadius, targetMask);

        foreach (Collider hit in hitColliders)
        {
            Debug.Log("Hit " + hit.transform.name);
            if (ObjectProneToInteraction(hit.gameObject))
                TurnColor(Color.green, hit.gameObject);
        }
    }
    public bool ObjectProneToInteraction(GameObject gameObject)
    {
        if (gameObject.GetComponent<BreakableObject>() != null || gameObject.GetComponent<InteractionObject>() != null || gameObject.GetComponent<TurnOff>() != null)
            return true;
        else return false;
    }
    public void TurnColor(Color color, GameObject hitData)
    {
        if (!redObjects.Contains(hitData))
        {
            if (hitData.GetComponentInChildren<MeshRenderer>() != null)
            {
                hitData.GetComponentInChildren<MeshRenderer>().material.color = color;
            }
            if (hitData.GetComponentInChildren<SpriteRenderer>() != null)
            {
                hitData.GetComponentInChildren<SpriteRenderer>().color = color;
            }
        }
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
        playerAction.OnParkourGlobal -= InitializeDash;
        playerAction.OnGhostGlobal -= LeaveGhostForm;
        playerAction.OnAttackGlobal -= Attack;
    }
}
