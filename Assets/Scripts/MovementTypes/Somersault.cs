using UnityEngine;

public class Somersault : MovementType
{
    public Somersault(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action) : base(rb, transform, controller, action)
    {
    }

    private Vector3 rotationAxis = Vector3.right;  // Axis for somersault (X-axis)
    private float duration;
    private float timeElapsed = 0.0f;
    private float totalRotation = 0.0f;  // Track total rotation
    private Quaternion initialRotation;  // Store the initial rotation of the player

    public override void EnterMovement()
    {
        timeElapsed = 0.0f;
        totalRotation = 0.0f;  // Reset the total rotation
        duration = playerController.SomersaultDuration;  // Somersault duration
        momentum.ModifyMomentum(1);  // Optional for movement effect

        initialRotation = playerTransform.rotation;  // Store the initial rotation
        Debug.Log("Somersault started");
    }

    public override void UpdateMovement()
    {
        if (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            // Calculate how much to rotate this frame
            float angleThisFrame = (360f / duration) * Time.deltaTime;

            // Accumulate the total rotation
            totalRotation += angleThisFrame;

            // Clamp the total rotation to not exceed 360 degrees
            if (totalRotation > 360f)
            {
                angleThisFrame -= (totalRotation - 360f);  // Correct for overshoot
                totalRotation = 360f;
            }

            // Rotate the player incrementally only around the X-axis (forward somersault)
            playerTransform.Rotate(rotationAxis, angleThisFrame, Space.Self);

            Debug.Log("Somersault progress: " + timeElapsed / duration + " | Angle rotated this frame: " + angleThisFrame + " | Total rotation: " + totalRotation);
        }
        else
        {
            // Ensure the player finishes exactly at 360 degrees if there's any leftover
            if (totalRotation < 360f)
            {
                float remainingRotation = 360f - totalRotation;
                playerTransform.Rotate(rotationAxis, remainingRotation, Space.Self);
                Debug.Log("Final adjustment: Rotated remaining " + remainingRotation + " degrees");
            }

            // Snap to the initial rotation to ensure the player ends up exactly as they started
            playerTransform.rotation = initialRotation;

            Debug.Log("Somersault ended | Final rotation: " + playerTransform.rotation.eulerAngles);

            // Once grounded, switch to regular movement
            if (IsGrounded())
            {
                playerController.SetMovement(playerController.RegularMovement);
            }
        }
    }
}
