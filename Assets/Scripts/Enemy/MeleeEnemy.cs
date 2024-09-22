using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyAI
{
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

    protected override void HandleCombat(float distanceToPlayer)
    {
        if (isStationary) return;

        // Look at the player (ignore Y-axis)
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

    protected override void HandleMovement()
    {
        if (isStationary) return;

        navMeshAgent.isStopped = false;
        navMeshAgent.destination = player.position;
    }
}
