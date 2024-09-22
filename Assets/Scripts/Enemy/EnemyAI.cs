using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour
{
    protected Transform player;
    public bool stopped;
    public Animator anim;
    public bool isStationary = false;
    protected NavMeshAgent navMeshAgent;
    protected bool isAttacking = false;
    protected float distanceToPlayer;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float noticeDistance;
    [SerializeField] protected Collider col;
    protected Rigidbody rigidBody;
    InteractionObject interactionObject;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        interactionObject = GetComponent<InteractionObject>();
        player = FindObjectOfType<PlayerController>().transform;
    }

    protected virtual void Update()
    {
        distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (isStationary)
        {
            navMeshAgent.isStopped = true;
            col.enabled = false;
            return;
        }
        if (navMeshAgent.enabled == true)
        {
            if (distanceToPlayer < noticeDistance)
            {
                if (distanceToPlayer <= attackDistance)
                {
                    HandleCombat(distanceToPlayer);
                }
                else
                {
                    HandleMovement();
                }
            }
            else
            {
                Patrol();
            }
        }
    }

    public void Patrol()
    {
        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            Vector3 patrolPoint = GetRandomPatrolPoint(10f);
            if (patrolPoint != Vector3.zero)
            {
                navMeshAgent.SetDestination(patrolPoint);
            }
        }
    }

    public Vector3 GetRandomPatrolPoint(float patrolRadius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }

    protected abstract void HandleMovement();
    protected abstract void HandleCombat(float distanceToPlayer);
    protected abstract IEnumerator Attack();
    internal void ApplyForce(Vector3 forceDirection, Vector3 hitPoint)
    {
        navMeshAgent.enabled = false;
        rigidBody.isKinematic = false;
        interactionObject.ApplyForce(forceDirection, hitPoint);
    }
    public void ReenableNavMeshAgent()
    {
        if (!navMeshAgent.enabled)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.ResetPath();
        }
    }
    public void UnpauseEnemy()
    {
        isStationary = false;
        navMeshAgent.isStopped = false;
        HandleMovement();
    }
}
