using UnityEngine;

public class Dash : MovementType
{
    private float timeElapsed;
    private Quaternion rot;
    public Dash(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
    }

    public override void EnterMovement()
    {
        timeElapsed = 0;
        InitializeDash();
    }
    private void InitializeDash()
    {
        playerRigidbody.AddForce(playerController.Cam.forward * playerController.DashForce * (1 + momentum.CurrentMomentum / 5));
        momentum.ModifyMomentum(-0.1f);
    }
    public override void UpdateMovement()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > 0.5)
        {
            StopDashing();
        }

    }
    private void StopDashing()
    {
        playerController.SetMovement(playerController.RegularMovement);
    }
}
