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
    public void Unhook()
    {
        joint.connectedBody = null;
    }
}
