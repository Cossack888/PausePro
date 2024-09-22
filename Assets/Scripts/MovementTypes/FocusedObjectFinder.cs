using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FocusedObjectFinder
{
    private PlayerController playerController;
    private Transform playerTransform;
    
    public FocusedObjectFinder(PlayerController playerController, Transform playerTransform)
    {
        this.playerController = playerController;
        this.playerTransform = playerTransform;
    }

    public GameObject FindObjectInFocus()
    {
        GameObject objectInFocus = FindObjectInFocusWithRaycast();
        if (objectInFocus == null)
        {
            objectInFocus = FindObjectInFocusWithOverlapSphere();
        }
        return objectInFocus;
    }

    private GameObject FindObjectInFocusWithRaycast()
    {
        if (Physics.Raycast(playerController.Cam.position, playerController.Cam.forward, out RaycastHit hit, Mathf.Infinity, playerController.GhostInteractionLayer))
        {
            if (InteractionUtils.ObjectProneToInteraction(hit.collider.gameObject))
            {
                //Debug.Log("Found object with raycast:" + hit.collider.gameObject.name);
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

            if (angle < lowestAngle && InteractionUtils.ObjectProneToInteraction(hit.gameObject))
            {
                //check line of sight - OverlapSphere goes through walls
                //use AllLayers - the Raycast might go throught walls otherwise (?)
                if (Physics.Raycast(playerController.Cam.position, directionToObject, out RaycastHit raycastHit, Mathf.Infinity, Physics.AllLayers))
                {
                    if (raycastHit.collider.gameObject != hit.gameObject)
                    {
                        continue;
                    }
                    lowestAngle = angle;
                    nearestObject = hit.gameObject;
                }
            }
        }
        // if(nearestObject!= null) {
        //     Debug.Log("Found object with overlap sphere:" + nearestObject.name);
        // }
        return nearestObject;
    }
    

}