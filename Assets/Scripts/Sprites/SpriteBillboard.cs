using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    PlayerController playerController;
    bool showBack;
    Animator anim;
    [SerializeField] bool twoSided;
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        anim = GetComponentInParent<Animator>();
    }
    private void Update()
    {
        if (playerController != null && twoSided)
        {
            Vector3 toPlayer = playerController.transform.position - transform.position;
            toPlayer.y = 0;
            float dotProduct = Vector3.Dot(transform.forward, toPlayer.normalized);

            if (playerController.CurrentMovement != playerController.GhostForm && playerController.CurrentMovement != playerController.GhostAttack)
            {
                transform.rotation = Quaternion.Euler(0, playerController.Cam.rotation.eulerAngles.y, 0);
            }
            else
            {
                if (dotProduct > 0)
                {
                    anim.SetBool("Back", true);
                    anim.SetBool("Stopped", false);
                }
                else
                {
                    anim.SetBool("Stopped", true);
                    anim.SetBool("Back", false);
                }
            }

        }
    }


}
