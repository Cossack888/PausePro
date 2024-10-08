using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerDoors : MonoBehaviour
{
    public bool lever;
    public bool inRange;
    public Animator anim;
    private Animator triggerAnim;
    public GameObject doors;
    public bool open;
    private PlayerAction actions;
    private void OnEnable()
    {
        actions = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAction>();
        actions.OnInteractGlobal += Interact;
    }
    private void OnDisable()
    {
        actions.OnInteractGlobal -= Interact;
    }

    private void Start()
    {
        triggerAnim = GetComponent<Animator>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    public void Interact()
    {
        if (inRange)
        {
            if (triggerAnim != null)
            {
                if (open == false)
                {
                    triggerAnim.SetTrigger("Open");
                }
                else if (open == true)
                {
                    triggerAnim.SetTrigger("Close");
                }
            }
            if (anim && open == false)
            {
                anim.SetTrigger("Open");
                open = true;
            }
            else if (anim && open == true)
            {
                anim.SetTrigger("Close");
                open = false;
            }
        }
    }

    private void Update()
    {
        if (inRange)
        {
            if (GetComponentInChildren<MeshRenderer>())
            {
                GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GetComponentInChildren<MeshRenderer>())
            {
                GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            }
            inRange = false;
        }
    }
}
