using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    public void Unhook()
    {
        if (GetComponentInChildren<MeshRenderer>() != null)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
        if (GetComponentInChildren<SpriteRenderer>() != null)
        {
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
        Rigidbody rb = joint.connectedBody;
        joint.connectedBody = null;
        rb.AddForce(Vector3.down * 0.1f, ForceMode.Impulse);
    }
}
