using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyAI
{
    protected override IEnumerator Attack()
    {
        while (distanceToPlayer <= attackDistance)
        {
            yield return new WaitForSeconds(2);
            anim.SetTrigger("Slash");
            yield return new WaitForSeconds(0.1f);
        }
        isAttacking = false;
    }

    protected override void Block()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleCombat(float distanceToPlayer)
    {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        anim.SetBool("Walking", false);
        agent.destination = transform.position;
        if (distanceToPlayer <= attackDistance)
        {
            if (distanceToPlayer < 0.3f)
            {
                anim.SetBool("Walking", true);
                agent.Move(-transform.forward * Time.deltaTime);
            }
            else
            {
                anim.SetBool("Walking", false);
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

        anim.SetBool("Walking", true);
        agent.destination = player.position;
    }

    protected override IEnumerator Throw()
    {
        throw new System.NotImplementedException();
    }
}
