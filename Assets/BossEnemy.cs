using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossEnemy : EnemyAI
{
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask walls;
    [SerializeField] LineRenderer rend;
    bool isLaser;
    bool inRange;
    [SerializeField] GameObject spellProjectile;
    [SerializeField] private float projectileSpeed;
    protected override IEnumerator Attack()
    {
        isAttacking = true;

        while (distanceToPlayer <= attackDistance)
        {
            anim.SetTrigger("Attack");
            col.enabled = true;
            yield return new WaitForSeconds(0.1f);
            col.enabled = false;
            yield return new WaitForSeconds(2f);
        }

        isAttacking = false;
        col.enabled = false;
    }

    public IEnumerator LaserAttack()
    {
        isLaser = true;

        while (distanceToPlayer <= rayDistance)
        {
            yield return new WaitForSeconds(10f);
            if (inRange)
            {
                //player.GetComponent<PlayerHealth>().TakeDamage(1);
                FireProjectile();
            }

            yield return new WaitForSeconds(10f);
        }

        isLaser = false;
    }

    protected void FireProjectile()
    {
        if (isStationary) { return; }
        if (!inRange) { return; }
        GameObject projectile = Instantiate(spellProjectile, transform.position + Vector3.up, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - transform.position + Vector3.up).normalized;
            rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
        }
    }

    protected override void HandleCombat(float distanceToPlayer)
    {
        if (isStationary) return;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        navMeshAgent.isStopped = true;

        if (distanceToPlayer <= attackDistance)
        {
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            if (isAttacking)
            {
                StopCoroutine(Attack());
                isAttacking = false;
                col.enabled = false;
            }
        }
    }
    private void LateUpdate()
    {
        if (distanceToPlayer <= rayDistance)
        {
            Targeting();
            if (!isLaser)
            {
                StartCoroutine(LaserAttack());
            }
        }
        else
        {
            rend.positionCount = 0;
        }
    }


    public void Targeting()
    {
        Vector3 origin = transform.position + Vector3.up;
        inRange = !Physics.Raycast(origin, player.transform.position - origin, out RaycastHit hit, Vector3.Distance(origin, player.transform.position), walls);
        if (inRange)
        {
            rend.positionCount = 2;
            rend.SetPosition(0, origin);
            rend.SetPosition(1, player.transform.position);
        }
        else
        {
            rend.positionCount = 0;
        }
    }
    protected override void HandleMovement()
    {
        if (isStationary) return;

        navMeshAgent.isStopped = false;
        navMeshAgent.destination = player.position;
    }
}
