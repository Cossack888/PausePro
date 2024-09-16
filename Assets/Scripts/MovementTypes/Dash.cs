using UnityEngine;

public class Dash : MovementType
{
    private Vector3 target;
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
        rot = playerTransform.rotation;
        playerRigidbody.AddForce(playerTransform.up * playerController.DashForce * (1 + momentum.CurrentMomentum));
        momentum.ModifyMomentum(-0.1f);
        playerTransform.Rotate(0, 0, 45);
    }
    public override void UpdateMovement()
    {
        float distance = Vector3.Distance(playerTransform.position, target);
        timeElapsed += Time.deltaTime;

        if (timeElapsed > 0.5)
        {
            StopDashing();
        }

    }
    private void StopDashing()
    {
        playerTransform.rotation = rot;
        playerController.SetMovement(playerController.RegularMovement);
    }
}
