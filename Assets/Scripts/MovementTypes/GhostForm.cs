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
    private FocusedObjectFinder focusedObjectFinder;
    public GhostForm(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        // action.OnParkourGlobal += InitializeDash;
        // action.OnGhostGlobal += LeaveGhostForm;
        // action.OnInteractGlobal += TransportObjectToPlayer;
        // action.OnShootGlobal += SaveForce;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        focusedObjectFinder= new FocusedObjectFinder(controller, transform);
    }

    public override void EnterMovement()
    {
        playerAction.OnParkourGlobal += InitializeDash;
        playerAction.OnGhostGlobal += LeaveGhostForm;
        playerAction.OnInteractGlobal += TransportObjectToPlayer;
        playerAction.OnShootGlobal += SaveForce;

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
        Vector3 targetVelocity = (playerAction.IsSprinting ? playerController.RunSpeed : playerController.WalkSpeed) * (worldMovement + verticalMovement);
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
            playerRigidbody.AddForce(playerController.Cam.forward * playerController.DashForce);
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

        GameObject currentFocusedObject = focusedObjectFinder.FindObjectInFocus();

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

    // GameObject FindObjectInFocus()
    // {
    //     GameObject objectInFocus = FindObjectInFocusWithRaycast();
    //     if (objectInFocus == null)
    //     {
    //         objectInFocus = FindObjectInFocusWithOverlapSphere();
    //     }
    //     return objectInFocus;
    // }

    // GameObject FindObjectInFocusWithRaycast()
    // {
    //     if (Physics.Raycast(playerController.Cam.position, playerController.Cam.forward, out RaycastHit hit, Mathf.Infinity, playerController.GhostInteractionLayer))
    //     {
    //         if (InteractionUtils.ObjectProneToInteraction(hit.collider.gameObject))
    //         {
    //             //Debug.Log("Found object with raycast:" + hit.collider.gameObject.name);
    //             return hit.collider.gameObject;
    //         }
    //         else
    //         {
    //             return null;
    //         }
    //     }
    //     return null;
    // }

    // GameObject FindObjectInFocusWithOverlapSphere()
    // {
    //     float sphereRadius = 10000.0f;
    //     LayerMask targetMask = Physics.AllLayers;
    //     Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, sphereRadius, targetMask);

    //     Transform camera = playerController.GhostCam.transform;

    //     GameObject nearestObject = null;
    //     float lowestAngle = Mathf.Infinity;

    //     foreach (Collider hit in hitColliders)
    //     {
    //         Vector3 directionToObject = hit.transform.position - camera.position;
    //         float angle = Vector3.Angle(camera.forward, directionToObject);

    //         if (angle < lowestAngle && InteractionUtils.ObjectProneToInteraction(hit.gameObject))
    //         {
    //             //check line of sight - OverlapSphere goes through walls
    //             //use AllLayers - the Raycast might go throught walls otherwise (?)
    //             if (Physics.Raycast(playerController.Cam.position, directionToObject, out RaycastHit raycastHit, Mathf.Infinity, Physics.AllLayers))
    //             {
    //                 if (raycastHit.collider.gameObject != hit.gameObject)
    //                 {
    //                     continue;
    //                 }
    //                 lowestAngle = angle;
    //                 nearestObject = hit.gameObject;
    //             }
    //         }
    //     }
    //     // if(nearestObject!= null) {
    //     //     Debug.Log("Found object with overlap sphere:" + nearestObject.name);
    //     // }
    //     return nearestObject;
    // }



    public void SaveForce()
    {
        Debug.Log("Saving force");
        Vector3 start = playerController.Cam.transform.position;
        Vector3 direction = playerController.Cam.transform.forward;
        Debug.DrawRay(start, direction * playerController.GhostInteractionDistance, Color.red);

        GameObject objectInFocus =  focusedObjectFinder.FindObjectInFocus();
        //if (Physics.Raycast(start, direction, out RaycastHit hit, playerController.GhostInteractionDistance, playerController.GhostInteractionLayer))
        if (objectInFocus != null)
        {
            TurnOff turnOff = objectInFocus.GetComponent<TurnOff>();
            InteractionObject hitObject = objectInFocus.gameObject.GetComponent<InteractionObject>();
            BreakableObject breakable = objectInFocus.GetComponent<BreakableObject>();
            playerController.GhostLeftHand.SetTrigger("push");
            TurnColor(Color.red, objectInFocus);
            redObjects.Add(objectInFocus);
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

                NavMeshAgent navMeshAgent = objectInFocus.GetComponent<NavMeshAgent>();
                EnemyAI enemy = objectInFocus.GetComponent<EnemyAI>();
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                //Vector3 forceDirection = (hit.point - start).normalized;
                Vector3 forceDirection = (objectInFocus.transform.position - start).normalized;
                savedForces.Add(new ForceData(rb, hitObject, forceDirection, objectInFocus.transform.position, navMeshAgent, enemy));
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
            if (forceData != null && forceData.enemy != null)
                forceData.enemy.ApplyForce(forceData.forceDirection, forceData.hitPoint);
            if (forceData != null && forceData.interactionObject != null)
                forceData.interactionObject.ApplyForce(forceData.forceDirection, forceData.hitPoint);

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
            if (InteractionUtils.ObjectProneToInteraction(hitObject))
            {
                playerController.GhostLeftHand.SetTrigger("pull");
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
            if (InteractionUtils.ObjectProneToInteraction(hit.gameObject))
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
        playerAction.OnParkourGlobal -= InitializeDash;
        playerAction.OnGhostGlobal -= LeaveGhostForm;
        playerAction.OnInteractGlobal -= TransportObjectToPlayer;
        playerAction.OnShootGlobal -= SaveForce;
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
        playerAction.OnInteractGlobal -= TransportObjectToPlayer;
        playerAction.OnShootGlobal -= SaveForce;
    }
}
