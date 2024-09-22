using System.Collections;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerHealth health;
    private Collider col;
    [SerializeField] float resetTimer;
    public bool heart;
    public bool flask;
    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        health = GameObject.FindObjectOfType<PlayerHealth>();
        col = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (flask)
            {
                gameManager.ChangeAmountOfBottles(1);

            }
            if (heart)
            {
                health.AddHealth(1);
            }
            col.enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            StartCoroutine(ResetCollectible(resetTimer));
        }
    }

    IEnumerator ResetCollectible(float resetTimer)
    {
        yield return new WaitForSeconds(resetTimer);
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        col.enabled = true;
    }
}
