using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    public TMP_Text deathText;
    public GameObject hitScreen;
    public float fadeSpeed = 0.5f;
    private Image image;
    private Color color;

    private void Start()
    {
        currentHealth = maxHealth;
        image = hitScreen.GetComponent<Image>();
        color = image.color;
        color.a = 0;
        image.color = color;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        color = image.color;
        color.a = 1f;
        image.color = color;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Update()
    {
        if (hitScreen != null && image.color.a > 0)
        {
            color = image.color;
            color.a -= fadeSpeed * Time.deltaTime;
            color.a = Mathf.Clamp01(color.a);
            image.color = color;
        }
    }

    public void Die()
    {
        deathText.enabled = true;
    }
}
