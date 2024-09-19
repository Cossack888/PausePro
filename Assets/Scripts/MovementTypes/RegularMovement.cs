using System;
using Unity.VisualScripting;
using UnityEngine;

public class RegularMovement : MovementType
{

    public RegularMovement(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        action.OnJumpGlobal += Jump;
        action.OnDashGlobal += Dash;
        action.OnAttackGlobal += Attack;
        action.OnGhostGlobal += Ghost;
    }
    public override void EnterMovement()
    {
        playerRigidbody.velocity = new Vector3(0, 0, 0);
    }
    public override void UpdateMovement()
    {
        movement = new Vector3(playerAction.Movement.x, 0f, playerAction.Movement.y);
        float slowMultiplier = playerAction.IsSprinting ? 5 : 10;
        momentum.ModifyMomentum(-Time.deltaTime / slowMultiplier);
        if (playerAction.IsCrouching && !crouched)
        {
            crouched = true;
            playerController.SetMovement(playerController.Crouching);
        }
        else if (!playerAction.IsCrouching && crouched)
        {
            crouched = false;
        }
    }
    public override void FixedUpdateMovement()
    {
        Vector3 localMovement = new Vector3(movement.x, 0, movement.z);
        Vector3 worldMovement = playerTransform.TransformDirection(localMovement);
        Vector3 targetVelocity = (playerAction.IsSprinting ? playerController.RunSpeed + momentum.CurrentMomentum : playerController.WalkSpeed + momentum.CurrentMomentum) * worldMovement;
        targetVelocity.y = playerRigidbody.velocity.y;
        playerRigidbody.velocity = targetVelocity;
    }

    public void Jump()
    {
        if (IsGrounded() && playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.Jumping);
        }
    }
    public void Dash()
    {
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.Dash);
        }
    }
    public void Attack()
    {
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.Attacking);
        }
    }
    public void Ghost()
    {
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.GhostForm);
        }
    }

    ~RegularMovement()
    {
        playerAction.OnJumpGlobal -= Jump;
        playerAction.OnDashGlobal -= Dash;
        playerAction.OnAttackGlobal -= Attack;
        playerAction.OnGhostGlobal -= Ghost;
    }
}