using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAttack : MovementType
{
    Animator anim;
    public float timer;
    public GhostAttack(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        anim = controller.GhostRightHand;
    }
    public override void EnterMovement()
    {
        anim.SetTrigger("Slash");
    }
    public override void UpdateMovement()
    {
        timer += Time.deltaTime;
        AnimatorClipInfo[] m_CurrentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        float m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
        if (m_CurrentClipLength > timer)
        {
            playerController.SetMovement(playerController.GhostForm);
        }
    }
    public override void ExitMovement()
    {
        base.ExitMovement();
    }
}
