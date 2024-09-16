using UnityEngine;

public class Collectible : MonoBehaviour
{
    private Momentum momentum;
    private void Start()
    {
        momentum = GameObject.FindObjectOfType<Momentum>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            momentum.ModifyMomentum(1);
            Destroy(gameObject);
        }
    }
}
