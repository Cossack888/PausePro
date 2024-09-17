using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    PlayerAction action;
    [SerializeField] GameObject projectile;
    [SerializeField] Camera cam;
    [SerializeField] float range;
    [SerializeField] float speed;
    [SerializeField] LayerMask targetMask;
    [SerializeField] Animator anim;
    private void Start()
    {
        action = GetComponentInParent<PlayerAction>();
        action.OnShootGlobal += Shoot;
    }
    void Shoot()
    {
        anim.SetTrigger("Shoot");
        Invoke("Cast", 0.2f);
    }

    void Cast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit target, range, targetMask))
        {
            GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
            Vector3 directionToTarget = (target.point - transform.position).normalized;
            proj.GetComponent<Rigidbody>().AddForce(directionToTarget * speed, ForceMode.Impulse);
        }

    }
    private void OnDisable()
    {
        action.OnShootGlobal -= Shoot;
    }
}
