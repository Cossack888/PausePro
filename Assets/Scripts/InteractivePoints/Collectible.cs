using System.Collections;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private GameManager gameManager;
    private Collider col;
    [SerializeField] float resetTimer;
    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        col = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.ChangeAmountOfBottles(1);
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
