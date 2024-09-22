using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    public GameObject hitScreen;
    public float fadeSpeed = 0.5f;
    private Image image;
    private Color color;
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Sprite heartFull;
    public Sprite heartEmpty;
    public GameObject deathScreen;
    bool vulnerable = true;
    private void Start()
    {
        currentHealth = maxHealth;
        vulnerable = true;
        image = hitScreen.GetComponent<Image>();
        color = image.color;
        color.a = 0;
        image.color = color;
    }

    public void TakeDamage(float damage)
    {
        if (vulnerable)
        {
            vulnerable = false;
            currentHealth -= damage;
            color = image.color;
            color.a = 1f;
            image.color = color;
            StartCoroutine(ResetVulnerability());
            switch (currentHealth)
            {
                case 3:
                    heart1.sprite = heartFull;
                    heart2.sprite = heartFull;
                    heart3.sprite = heartFull;
                    break;
                case 2:
                    heart1.sprite = heartFull;
                    heart2.sprite = heartFull;
                    heart3.sprite = heartEmpty;
                    break;
                case 1:
                    heart1.sprite = heartFull;
                    heart2.sprite = heartEmpty;
                    heart3.sprite = heartEmpty;
                    break;
                case 0:
                    heart1.sprite = heartEmpty;
                    heart2.sprite = heartEmpty;
                    heart3.sprite = heartEmpty;
                    break;
                default:
                    heart1.sprite = heartEmpty;
                    heart2.sprite = heartEmpty;
                    heart3.sprite = heartEmpty;
                    break;

            }
            if (currentHealth <= 0)
            {
                Die();
            }
        }

    }
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        switch (currentHealth)
        {
            case 3:
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartFull;
                break;
            case 2:
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartEmpty;
                break;
            case 1:
                heart1.sprite = heartFull;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;
                break;
            case 0:
                heart1.sprite = heartEmpty;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;
                break;
            default:
                heart1.sprite = heartEmpty;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;
                break;

        }
    }
    IEnumerator ResetVulnerability()
    {
        yield return new WaitForSeconds(1f);
        vulnerable = true;
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
        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
}
