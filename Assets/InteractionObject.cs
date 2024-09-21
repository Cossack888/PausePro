using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class InteractionObject : MonoBehaviour, IProneToInteraction
{
    public bool hasBeenPushed;
    public bool hasBeenMarked;
    NavMeshAgent agent;
    EnemyAI enemyAI;
    Rigidbody rb;
    bool inMotion;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyAI = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (inMotion && rb.velocity.magnitude == 0)
        {
            ResetBeingPushed();
        }
    }

    //FIXME: change name
    public void Push()
    {
        hasBeenPushed = true;
        if (GetComponentInChildren<MeshRenderer>() != null)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
        if (GetComponentInChildren<SpriteRenderer>() != null)
        {
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }

        Invoke("CheckForMotion", 0.5f);
        //Invoke("ResetBeingPushed", 5);
    }

    public void ApplyForce(Vector3 forceDirection, Vector3 hitPoint)
    {
        rb.AddForceAtPosition(forceDirection * 30, hitPoint, ForceMode.Impulse);
        Push();
    }

    public void CheckForMotion()
    {
        inMotion = true;
    }
    public void ResetBeingPushed()
    {
        hasBeenPushed = false;
        inMotion = false;
        if (gameObject.GetComponent<EnemyAI>() != null)
        {
            if (agent != null)
            {
                agent.enabled = true;
            }
            if (enemyAI != null)
            {
                enemyAI.enabled = true;
                enemyAI.ReenableNavMeshAgent();
            }

        }
        else
        {
            rb.isKinematic = true;
        }

    }
}
