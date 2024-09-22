using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    Animator anim;
    public GameObject VFX;
    public int points;
    GameManager gameManager;
    private void Start()
    {
        anim = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit");
        }
    }

    public void TakeDamageFromBehind(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        gameManager.ChangeAmountOfPoints(points);
        GameObject tempVFX = Instantiate(VFX, transform.position, Quaternion.identity);
        Destroy(tempVFX, 2);
        Destroy(gameObject);
    }
}
