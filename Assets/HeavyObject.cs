using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyObject : MonoBehaviour
{
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > 0.3f && collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(200);
        }
    }
}
