using UnityEngine;

public class ActivatePrompt : MonoBehaviour
{
    private Prompts prompts;
    [SerializeField] private int promptId;
    private void Start()
    {
        prompts = GameObject.FindObjectOfType<Prompts>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            prompts.ChangePrompt(promptId);
            GetComponent<Collider>().enabled = false;
        }
    }
}
