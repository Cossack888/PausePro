using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedObject : MonoBehaviour
{
    public bool carried;

    // Update is called once per frame
    void Update()
    {
        if (carried)
        {
            transform.position = transform.parent.position;
        }
    }
}
