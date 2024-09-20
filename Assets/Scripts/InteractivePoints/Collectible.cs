using UnityEngine;

public class Collectible : MonoBehaviour
{
    private Momentum momentum;
    private GameManager gameManager;
    private void Start()
    {
        momentum = GameObject.FindObjectOfType<Momentum>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            momentum.ModifyMomentum(1);
            gameManager.ChangeAmountOfBottles(1);
            Destroy(gameObject);
        }
    }
}
