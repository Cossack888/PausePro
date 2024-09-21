
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Vector3 currentTarget;
    public bool stopped;
    public Animator anim;
    EnemySpawner spawner;
    [SerializeField] protected float throwDistance;
    [SerializeField] protected float throwSpeed;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float noticeDistance;
    protected NavMeshAgent navMeshAgent;
    protected bool isThrowing = false;
    protected Coroutine throwCoroutine = null;
    [SerializeField] protected GameObject missile;
    [SerializeField] protected float missileSpeed;
    [SerializeField] protected Transform missileSpawnPosition;
    public GameObject swordDrawn;
    public GameObject swordSheathed;
    public GameObject swordWalkingBlock;
    protected bool swordReady;
    protected bool isAttacking = false;
    protected float distanceToPlayer;
    PlayerController playerController;
    public bool isStationary = false;
    [SerializeField] protected Collider col;
    Rigidbody rigidBody;
    InteractionObject interactionObject;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        interactionObject = GetComponent<InteractionObject>();
        spawner = FindObjectOfType<EnemySpawner>();
    }

    // NavMeshAgent agent;
    // EnemyAI enemyAI;
    // Rigidbody rb;
    // bool inMotion;
    // private void Start()
    // {
    //     agent = GetComponent<NavMeshAgent>();
    //     enemyAI = GetComponent<EnemyAI>();
    //     rb = GetComponent<Rigidbody>();
    // }

    private void OnEnable()
    {
        playerController = FindObjectOfType<PlayerController>();
        player = playerController.transform;
    }
    private void OnDisable()
    {

    }
    protected virtual void Update()
    {
        if (isStationary)
        {
            navMeshAgent.isStopped = true;
            col.enabled = false;
            return;
        }

        distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer < noticeDistance)
        {
            //FIXME:
            if (distanceToPlayer > throwDistance)
            {
                //this should only happen if the enemy is an a surface
                //and has navmeshagent enabled. Fix the conditions
                if (navMeshAgent.enabled)
                {
                    HandleMovement();
                }
            }
            else
            {
                if (navMeshAgent.enabled)
                {
                    HandleCombat(distanceToPlayer);
                }

            }
        }
        else
        {
            if (navMeshAgent.enabled)
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

    public void UnpauseEnemy()
    {
        isStationary = false;
        navMeshAgent.isStopped = false;
        HandleMovement();
    }

    protected abstract void Block();
    protected abstract void HandleMovement();
    protected abstract void HandleCombat(float distanceToPlayer);


    protected virtual void StartThrowing()
    {
        if (!isThrowing)
        {
            isThrowing = true;
            throwCoroutine = StartCoroutine(Throw());
        }
    }

    protected virtual void StopThrowing()
    {
        if (isThrowing)
        {
            StopCoroutine(throwCoroutine);
            isThrowing = false;
        }
    }
    public void ReenableNavMeshAgent()
    {
        if (!navMeshAgent.enabled)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.ResetPath();  // Clear the previous path
            //agent.SetDestination(player.position);  // Set new destination to the player
        }
    }
    protected virtual void StartAttacking()
    {

        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(Attack());
            StopThrowing();
        }
    }

    protected virtual void StopAttacking()
    {
        if (isAttacking)
        {
            isAttacking = false;
            if (col)
            {
                col.enabled = false;
            }
        }
    }

    protected abstract IEnumerator Throw();
    protected abstract IEnumerator Attack();

    internal void ApplyForce(Vector3 forceDirection, Vector3 hitPoint)
    {
        navMeshAgent.enabled = false;
        rigidBody.isKinematic = false;
        interactionObject.ApplyForce(forceDirection, hitPoint);
    }
}
