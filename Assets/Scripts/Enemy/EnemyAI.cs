using System.Collections;
using System.Collections.Generic;  // This is optional but often used for collections like List<T>
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour
{
    public Transform player;
    protected Animator anim;
    [SerializeField] protected float throwDistance;
    [SerializeField] protected float throwSpeed;
    [SerializeField] protected float attackDistance;
    protected NavMeshAgent agent;
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
    [SerializeField] protected bool isStationary = false;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

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

        distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer > throwDistance)
        {
            HandleMovement();
        }
        else
        {
            HandleCombat(distanceToPlayer);
        }

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
        }
    }

    protected abstract IEnumerator Throw();
    protected abstract IEnumerator Attack();
}
