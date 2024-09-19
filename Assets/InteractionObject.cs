using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InteractionObject : MonoBehaviour
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
    public void Push()
    {
        hasBeenPushed = true;
        Invoke("CheckForMotion", 0.5f);
        Invoke("ResetBeingPushed", 5);
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
            rb.isKinematic = true;
        }

    }
}
