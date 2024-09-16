using UnityEngine;

public class WallRun : MovementType
{
    private float duration;
    private float timeElapsed = 0.0f;
    private Vector3 wallNormal;
    private Vector3 wallRunDirection;
    private bool isWallRunning = false;
    private Vector3 targetPosition;
    private bool isSwitchingSides = false;
    private float switchDuration = 1f;
    private float switchingTime = 0f;

    public WallRun(Rigidbody rb, Transform transform, PlayerController controller, PlayerAction action)
        : base(rb, transform, controller, action)
    {
        action.OnParkourGlobal += SwitchSides;
    }

    public override void EnterMovement()
    {
        InitializeWallRun();
    }

    private void InitializeWallRun()
    {
        timeElapsed = 0f;
        switchingTime = 0f;
        duration = playerController.WallRunDuration;

        playerRigidbody.useGravity = false;
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);

        if (DetectWall(out wallNormal))
        {
            DetermineWallRunDirection();
            RotatePlayerToWallRunDirection();
            isWallRunning = true;
        }
        else
        {
            EndWallRun();
        }
    }


    private void DetermineWallRunDirection()
    {
        if (Physics.Raycast(playerTransform.position, -playerTransform.forward, playerController.WallDistance, playerController.WallMask))
        {
            wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
        }
        else if (Physics.Raycast(playerTransform.position, playerTransform.forward, playerController.WallDistance, playerController.WallMask))
        {
            wallRunDirection = Vector3.Cross(Vector3.up, wallNormal).normalized;
        }
        else if (Physics.Raycast(playerTransform.position, -playerTransform.up, playerController.WallDistance, playerController.WallMask))
        {
            wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
        }
        else if (Physics.Raycast(playerTransform.position, playerTransform.up, playerController.WallDistance, playerController.WallMask))
        {
            wallRunDirection = Vector3.Cross(Vector3.up, wallNormal).normalized;
        }
    }

    public void SwitchSides()
    {
        if (playerController.CurrentMovement == this)
        {
            if (DetectWall(out Vector3 wallNormal))
            {
                CapsuleCollider playerCollider = playerTransform.GetComponent<CapsuleCollider>();
                float playerRadius = playerCollider.radius;
                if (Physics.Raycast(playerTransform.position, wallNormal, out RaycastHit hit, 10, playerController.WallMask))
                {
                    momentum.ModifyMomentum(0.5f);
                    float distanceToWall = Vector3.Distance(playerTransform.position, hit.point);
                    float stopDistance = playerRadius + 0.05f;
                    if (distanceToWall > stopDistance)
                    {
                        targetPosition = hit.point - wallNormal * stopDistance;
                    }
                    else
                    {
                        targetPosition = hit.point;
                    }
                    isSwitchingSides = true;
                    isWallRunning = false;
                    switchingTime = 0f;

                }
                else
                {
                    momentum.ResetMomentum();
                    playerRigidbody.useGravity = true;
                    playerRigidbody.AddForce((wallNormal + wallRunDirection) * 400f, ForceMode.Impulse);
                }
            }
        }
    }

    public override void FixedUpdateMovement()
    {
        if (isSwitchingSides)
        {
            PerformSideSwitch();
        }
        else if (isWallRunning)
        {
            ContinueWallRun();
        }
    }

    private void PerformSideSwitch()
    {
        switchingTime += Time.deltaTime;
        float t = switchingTime / switchDuration;
        Vector3 wallRunForwardOffset = wallRunDirection * 1.5f;
        Vector3 forwardTargetPosition = targetPosition + wallRunForwardOffset;
        Vector3 newPosition = Vector3.Lerp(playerTransform.position, forwardTargetPosition, t);
        playerRigidbody.MovePosition(newPosition);
        float distanceToTarget = Vector3.Distance(playerTransform.position, forwardTargetPosition);

        if (distanceToTarget < 0.2f || t >= 1f)
        {
            isSwitchingSides = false;
            InitializeWallRun();
        }
    }

    private void ContinueWallRun()
    {
        if (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            if (IsWallRunning())
            {
                playerRigidbody.velocity = wallRunDirection * (playerController.RunSpeed + momentum.CurrentMomentum);
                RotatePlayerToWallRunDirection();
            }
            else
            {
                EndWallRun();
            }
        }
        else
        {
            EndWallRun();
        }
    }
    private bool DetectWall(out Vector3 wallNormal)
    {
        wallNormal = Vector3.zero;
        RaycastHit hit;
        float rayLength = playerController.WallDistance;
        Vector3 rightDirection = playerTransform.forward;
        if (Physics.Raycast(playerTransform.position, rightDirection, out hit, rayLength, playerController.WallMask))
        {
            wallNormal = hit.normal;
            return true;
        }
        Vector3 leftDirection = -playerTransform.forward;
        if (Physics.Raycast(playerTransform.position, leftDirection, out hit, rayLength, playerController.WallMask))
        {
            wallNormal = hit.normal;
            return true;
        }

        Vector3 frontDirection = playerTransform.right;
        if (Physics.Raycast(playerTransform.position, frontDirection, out hit, rayLength, playerController.WallMask))
        {
            wallNormal = hit.normal;
            return true;
        }

        Vector3 behindDirection = -playerTransform.right;
        if (Physics.Raycast(playerTransform.position, behindDirection, out hit, rayLength, playerController.WallMask))
        {
            wallNormal = hit.normal;
            return true;
        }
        return false;
    }


    private bool IsWallRunning()
    {
        return StuckToLeftSide() || StuckToRightSide();
    }

    private void RotatePlayerToWallRunDirection()
    {
        if (wallRunDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(wallRunDirection, Vector3.forward);
            targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0);
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation * Quaternion.Euler(0f, 90f, 90f), playerController.RotationSpeed * Time.deltaTime);
        }
    }

    private void EndWallRun()
    {
        playerRigidbody.useGravity = true;
        isWallRunning = false;
        playerController.SetMovement(playerController.RegularMovement);

    }

    public override void ExitMovement()
    {
        playerRigidbody.useGravity = true;
    }

    ~WallRun()
    {
        playerAction.OnParkourGlobal -= SwitchSides;
    }
}
