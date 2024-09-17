using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [SerializeField] float damage = 10;
    [SerializeField] GameObject spellEffect;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Destructable")))
        {
            GameObject effect = Instantiate(spellEffect, collision.transform.position, transform.rotation);
            Destroy(effect, 1);
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
