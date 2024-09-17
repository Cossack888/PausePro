using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : MovementType
{
    public Falling(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
    }

}
