using UnityEngine;

public class Jumping : MovementType
{
    public Jumping(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
        //action.OnParkourGlobal += Somersault;
        action.OnJumpGlobal += WallRunOrDoubleJump;
        controller.OnLand += Landed;
    }
    private float initialYPosition;
    private float targetYPosition;
    private float currentYVelocity;
    private bool isAscending;
    private bool isAtPeak;
    private bool doubleJumped;
    private bool landed;
    public override void EnterMovement()
    {
        initialYPosition = playerRigidbody.position.y;
        targetYPosition = initialYPosition + playerController.JumpHeight;
        currentYVelocity = 0f;
        isAscending = true;
        isAtPeak = false;
        landed = true;
    }
    public override void UpdateMovement()
    {
        if (IsGrounded() && Falling() && !landed)
        {
            doubleJumped = false;
            Landed();
        }

        movement = new Vector3(playerAction.Movement.x, 0f, playerAction.Movement.y);

        if (!IsGrounded())
        {
            movement *= playerController.AirControlFactor;
        }

    }

    public void WallRunOrDoubleJump()
    {
        if (!IsGrounded() && playerController.CurrentMovement == this)
        {
            if (StuckToLeftSide() || StuckToRightSide())
            {
                WallRun();
            }
            else
            {
                DoubleJump();
            }
        }
    }
    public void Landed()
    {
        landed = true;
        if (playerController.CurrentMovement == this)
        {
            playerController.SetMovement(playerController.RegularMovement);
        }
    }
    public void WallRun()
    {
        playerController.SetMovement(playerController.WallRun);
    }

    public override void FixedUpdateMovement()
    {
        Vector3 localMovement = new Vector3(movement.x, 0, movement.z);
        Vector3 worldMovement = playerTransform.TransformDirection(localMovement);
        Vector3 targetVelocity = playerController.RunSpeed * worldMovement;
        targetVelocity.y = playerRigidbody.velocity.y;
        if (isAscending)
        {
            currentYVelocity += playerController.JumpForce * Time.deltaTime;
            if (playerRigidbody.position.y >= targetYPosition)
            {
                isAscending = false;
                isAtPeak = true;
            }
        }
        else if (isAtPeak)
        {
            landed = false;
            currentYVelocity -= playerController.JumpForce * Time.deltaTime;
            if (currentYVelocity <= 0)
            {
                isAtPeak = false;
            }
        }
        else
        {
            currentYVelocity -= playerController.JumpForce * Time.deltaTime * 2;
        }
        targetVelocity.y = currentYVelocity;
        playerRigidbody.velocity = targetVelocity;
    }
    void DoubleJump()
    {
        if (!doubleJumped)
        {
            doubleJumped = true;
            playerController.SetMovement(this);
        }
    }

    ~Jumping()
    {
        //playerAction.OnParkourGlobal -= Somersault;
        playerAction.OnJumpGlobal -= WallRunOrDoubleJump;
        playerController.OnLand -= Landed;
    }
}