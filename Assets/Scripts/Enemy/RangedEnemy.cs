using System.Collections;
using UnityEngine;

public class RangedEnemy : EnemyAI
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    private float fireTimer;
    private Coroutine attackCoroutine;

    protected override IEnumerator Attack()
    {
        isAttacking = true;

        while (distanceToPlayer <= attackDistance)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireRate)
            {
                fireTimer = 0f;
                anim.SetTrigger("Attack");
                yield return new WaitForSeconds(1f);

                FireProjectile();
            }

            yield return null;
        }

        isAttacking = false;
        attackCoroutine = null;
    }

    protected void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - projectileSpawnPoint.position).normalized;
            rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
        }
    }

    protected override void HandleMovement()
    {
        if (isStationary) return;

        navMeshAgent.isStopped = false;
        navMeshAgent.destination = player.position;
    }

    protected override void HandleCombat(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackDistance)
        {
            navMeshAgent.isStopped = true;
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(Attack());
            }
        }
        else
        {
            navMeshAgent.isStopped = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
                isAttacking = false;
            }
        }
    }
}
