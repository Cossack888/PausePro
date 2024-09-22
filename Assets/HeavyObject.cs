using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyObject : MonoBehaviour
{
    Rigidbody rb;
    InteractionObject interactionObject;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        interactionObject = GetComponent<InteractionObject>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if ((interactionObject.inMotion || rb.velocity.magnitude > 2f) && collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponentInParent<Health>().TakeDamage(200);
        }
    }
}
