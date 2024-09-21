using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAction : MonoBehaviour
{
    PlayerAction action;
    //[SerializeField] Camera cam;
    //[SerializeField] float range;
    [SerializeField] float speed;
    //[SerializeField] LayerMask targetMask;
    [SerializeField] Animator anim;
    PlayerController playerController;
    private void Start()
    {
        action = GetComponentInParent<PlayerAction>();
        playerController = GetComponentInParent<PlayerController>();
    }

    //FIXME: bind this to an acutal action
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            HighlightStuff();
        }
    }

    // void HighlightStuff()
    // {
    //     Debug.Log("Running the HighlightStuff action");
    //     //FIXME: should use the current cam
    //     Camera cam = playerController.Cam.GetComponent<Camera>();

    //     float sphereRadius = 1000.0f;
    //     float range = 0f;
    //     LayerMask targetMask = Physics.AllLayers;

    //     // Draw a debug ray to visualize the SphereCast direction
    //     Debug.DrawRay(cam.transform.position, cam.transform.forward * range, Color.red, 2.0f);


    //     if (Physics.SphereCast(cam.transform.position, sphereRadius, cam.transform.forward, out RaycastHit hit, range, targetMask))
    //     {
    //         //If the SphereCast hits something, print out the name of the object
    //         Debug.Log("Hit " + hit.transform.name);
    //     }
    // }

    void HighlightStuff()
    {
        Debug.Log("Running the HighlightStuff action");
        float sphereRadius = 10000.0f;
        LayerMask targetMask = Physics.AllLayers;
        Collider[] hitColliders = Physics.OverlapSphere(playerController.Cam.transform.position, sphereRadius, targetMask);

        foreach (Collider hit in hitColliders)
        {
            Debug.Log("Hit " + hit.transform.name);
        }
    }

    // void Shoot()
    // {
    //     anim.SetTrigger("Shoot");
    //     Invoke("Cast", 0.2f);
    // }

    // void Cast()
    // {
    //     if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit target, range, targetMask))
    //     {
    //         GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
    //         Vector3 directionToTarget = (target.point - transform.position).normalized;
    //         proj.GetComponent<Rigidbody>().AddForce(directionToTarget * speed, ForceMode.Impulse);
    //     }

    private void OnDisable()
    {
    }
}
