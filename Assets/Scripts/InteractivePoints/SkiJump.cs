using TMPro;
using UnityEngine;

public class SkiJump : MonoBehaviour
{
    [SerializeField] private Transform startingPoint;
    [SerializeField] private TMP_Text jumpLength;
    private TimerDisplay timer;

    private void Start()
    {
        jumpLength.text = " ";
        timer = GameObject.FindObjectOfType<TimerDisplay>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 impactPoint = collision.contacts[0].point;
            float distance = Vector3.Distance(startingPoint.position, impactPoint);
            jumpLength.text = "Jump Length: " + distance.ToString("F2") + " meters";
            timer.StopTimer();
            GetComponent<Collider>().enabled = false;
        }
    }
}

