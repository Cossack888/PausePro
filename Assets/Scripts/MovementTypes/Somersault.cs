using UnityEngine;

public class Somersault : MovementType
{
    public Somersault(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
    }
    private Vector3 rotationAxis = Vector3.forward;
    private float duration;
    private float timeElapsed = 0.0f;

    private Quaternion startRotation;
    private Quaternion endRotation;

    public override void EnterMovement()
    {
        timeElapsed = 0.0f;
        duration = playerController.SomersaultDuration;
        startRotation = playerTransform.rotation;
        endRotation = startRotation * Quaternion.Euler(rotationAxis * 360f);
        momentum.ModifyMomentum(1);
    }
    public override void UpdateMovement()
    {
        if (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float angleThisFrame = (360f / duration) * Time.deltaTime;

            playerTransform.Rotate(rotationAxis, angleThisFrame, Space.Self);
        }
        else
        {
            playerTransform.rotation = endRotation;
        }
        if (IsGrounded())
        {
            playerController.SetMovement(playerController.RegularMovement);
        }
    }
}
