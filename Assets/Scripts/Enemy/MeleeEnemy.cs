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

    protected override void Block()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleCombat(float distanceToPlayer)
    {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        agent.destination = transform.position;
        if (distanceToPlayer <= attackDistance)
        {
            if (distanceToPlayer < 0.3f)
            {
                agent.Move(-transform.forward * Time.deltaTime);
            }
            else
            {
                StartAttacking();
            }
        }
        else
        {
            StopAttacking();
        }
    }

    protected override void HandleMovement()
    {
        if (isStationary) return;
        agent.destination = player.position;
    }

    protected override IEnumerator Throw()
    {
        throw new System.NotImplementedException();
    }
}
