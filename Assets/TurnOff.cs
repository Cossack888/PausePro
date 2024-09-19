using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOff : MonoBehaviour
{
    HingeJoint joint;
    public bool on = true;
    private void Start()
    {
        joint = GetComponent<HingeJoint>();
    }

    public void Turn()
    {
        on = false;
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void Unhook()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
        Rigidbody rb = joint.connectedBody;
        joint.connectedBody = null;
        rb.AddForce(Vector3.down * 0.1f, ForceMode.Impulse);
    }
}
