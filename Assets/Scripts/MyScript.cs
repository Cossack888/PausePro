using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            HighlightStuff();
        }
    }
    void HighlightStuff()
    {
        Debug.Log("Running the HighlightStuff action");
        float sphereRadius = 10000.0f;
        LayerMask targetMask = Physics.AllLayers;
        Collider[] hitColliders = Physics.OverlapSphere(controller.Cam.position, sphereRadius, targetMask);

        foreach (Collider hit in hitColliders)
        {
            Debug.Log("Hit " + hit.transform.name);

        }
    }


}
