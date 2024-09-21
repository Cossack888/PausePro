using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    PlayerController playerController;
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }
    private void Update()
    {
        if (playerController != null)
        {
            Vector3 toPlayer = playerController.transform.position - transform.position;
            toPlayer.y = 0;
            transform.rotation = Quaternion.Euler(0, playerController.Cam.rotation.eulerAngles.y, 0);



        }
    }


}
