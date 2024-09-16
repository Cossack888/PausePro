using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    private PlayerAction action;
    private Camera cam;
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        action = player.GetComponent<PlayerAction>();
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        transform.position = new Vector3(player.position.x, 15f, player.position.z);
        cam.orthographicSize = Mathf.Clamp(action.ScrollAmount, 5, 20);
    }
}
