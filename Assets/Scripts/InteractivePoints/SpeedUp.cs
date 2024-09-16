using UnityEngine;

public class SpeedUp : MonoBehaviour
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
            this.enabled = false;
        }
    }
}
