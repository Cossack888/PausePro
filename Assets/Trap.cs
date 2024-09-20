using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] float trapDamageInterval;
    public float timer;
    public bool timerOn;

    private void OnCollisionEnter(Collision collision)
    {
        timer = 0;
        timerOn = true;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(200);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }
    private void OnCollisionStay(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (timerOn)
            {
                timer += Time.deltaTime;
                if (timer > trapDamageInterval)
                {
                    collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
                    timer = 0;
                }

            }
        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timerOn = false;
        }
    }
}
