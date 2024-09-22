using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    float timer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2)
        {
            Destroy(gameObject);
        }
    }

}
