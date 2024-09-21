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
    private bool inGhostForm = true;
    private float cooldown;
    public Vector3 previousPosition;
    private GameObject playerBody;
    private List<ForceData> savedForces = new List<ForceData>();
    private List<TurnOff> savedTurnOffs = new List<TurnOff>();
    private List<BreakableObject> explosions = new List<BreakableObject>();
    private List<GameObject> redObjects = new List<GameObject>();
    private List<GameObject> greenObjects = new List<GameObject>();
    private GameObject highlightedObject;
    private GameManager gameManager;
    public GhostForm(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        action.OnParkourGlobal += InitializeDash;
        action.OnGhostGlobal += LeaveGhostForm;
        action.OnAttackGlobal += TransportObjectToPlayer;
        action.OnShootGlobal += SaveForce;
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
        foreach (EnemyAI enemy in GameObject.FindObjectsOfType<EnemyAI>())
        {
            enemy.isStationary = true;
        }
        inGhostForm = true;
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
        HighlightObject();
        if (cooldown <= 0)
        {
            LeaveGhostForm();
        }
        if (dashTimer == true)
        {
            if (timeElapsed > 0.5)
            {
                playerRigidbody.velocity = Vector3.zero;
                dashTimer = false;
            }
        }
    }

    private void InitializeDash()
    {
        if (playerController.CurrentMovement == this)
        {
            dashTimer = true;
            timeElapsed = 0;
            playerRigidbody.AddForce(playerController.Cam.forward * playerController.DashForce * (1 + momentum.CurrentMomentum / 5));
        }

    }
    public void Attack()
    {
        playerController.GhostRightHand.SetTrigger("Slash");
    }
    public override void ExitMovement()
    {
        inGhostForm = false;
        playerRigidbody.useGravity = true;

        foreach (EnemyAI enemy in GameObject.FindObjectsOfType<EnemyAI>())
        {
            enemy.UnpauseEnemy();
        }
        redObjects.Clear();
        foreach (GameObject objects in greenObjects)
        {
            if (objects != null)
            {
                TurnColor(Color.white, objects);
            }
        }
        playerTransform.position = previousPosition;
        playerController.DestroyPlayerBody();
        ApplySavedForces();
        AudioManager.instance.SwitchToAlbum("flesh");

        UnhighlightStuff();
    }

    public void HighlightObject()
    {
        if (!inGhostForm)
        {
            return;
        }

        GameObject currentFocusedObject = FindObjectInFocusWithRaycast();
        if (currentFocusedObject == null)
        {
            currentFocusedObject = FindObjectInFocusWithOverlapSphere();
        }

        if (highlightedObject != null && highlightedObject != currentFocusedObject)
        {
            TurnColor(Color.green, highlightedObject);
        }

        if (currentFocusedObject != null)
        {
            TurnColor(Color.blue, currentFocusedObject);
        }

        highlightedObject = currentFocusedObject;
    }

    GameObject FindObjectInFocusWithRaycast()
    {
        if (Physics.Raycast(playerController.Cam.position, playerController.Cam.forward, out RaycastHit hit, Mathf.Infinity, playerController.GhostInteractionLayer))
        {
            if (ObjectProneToInteraction(hit.collider.gameObject))
            {
                Debug.Log("Found with raycast: " + hit.collider.gameObject.name);
                return hit.collider.gameObject;
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    GameObject FindObjectInFocusWithOverlapSphere()
    {
        float sphereRadius = 10000.0f;
        LayerMask targetMask = Physics.AllLayers;
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, sphereRadius, targetMask);

        Transform camera = playerController.GhostCam.transform;

        GameObject nearestObject = null;
        float lowestAngle = Mathf.Infinity;

        foreach (Collider hit in hitColliders)
        {
            Vector3 directionToObject = hit.transform.position - camera.position;
            float angle = Vector3.Angle(camera.forward, directionToObject);

            if (angle < lowestAngle && ObjectProneToInteraction(hit.gameObject))
            {
                lowestAngle = angle;
                nearestObject = hit.gameObject;
            }
        }
        Debug.Log("Found with overlapSphere: " + nearestObject.name);
        return nearestObject;
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
            if (forceData != null)
            {
                if (forceData.enemy != null)
                {
                    forceData.enemy.ApplyForce(forceData.forceDirection, forceData.hitPoint);
                } else if (forceData.interactionObject != null)
                {
                    forceData.interactionObject.ApplyForce(forceData.forceDirection, forceData.hitPoint);
                }
            }
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

    void ChangeColorOfObjectsProneToInteraction(Color color)
    {
        float sphereRadius = 10000.0f;
        LayerMask targetMask = Physics.AllLayers;
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, sphereRadius, targetMask);

        foreach (Collider hit in hitColliders)
        {
            greenObjects.Add(hit.gameObject);
            if (ObjectProneToInteraction(hit.gameObject))
                TurnColor(color, hit.gameObject);
        }

    }

    void HighlightStuff()
    {
        ChangeColorOfObjectsProneToInteraction(Color.green);
    }

    void UnhighlightStuff()
    {
        ChangeColorOfObjectsProneToInteraction(Color.white);
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
        playerAction.OnAttackGlobal -= TransportObjectToPlayer;
        playerAction.OnShootGlobal -= SaveForce;
    }
}
