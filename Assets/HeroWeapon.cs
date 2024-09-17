using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroWeapon : MonoBehaviour
{
    bool damageDealt;
    [SerializeField] GameObject bloodEffect;
    Collider col;
    private void Start()
    {
        col = GetComponent<Collider>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!damageDealt)
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 collisionPoint = contact.point;
                collision.gameObject.GetComponent<Health>().TakeDamage(20);
                GameObject effect = Instantiate(bloodEffect, collisionPoint, Quaternion.identity, collision.transform);
                Destroy(effect, 1);
                damageDealt = true;
                col.isTrigger = true;
                Invoke("ResetWeapon", 1);
            }
        }
    }
    public void ResetWeapon()
    {
        damageDealt = false;
        col.isTrigger = false;
    }
}
