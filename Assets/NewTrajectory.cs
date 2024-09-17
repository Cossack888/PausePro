using System.Collections.Generic;
using UnityEngine;

public class NewTrajectory : MonoBehaviour
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField][Range(3f, 30f)] int _lineSegmentCount = 20;
    List<Vector3> _linePoints = new List<Vector3>();
    public void UpdateTrajectory(Vector3 forceVector, Rigidbody rigidbody, Vector3 startingPoint)
    {
        Vector3 velocity = (forceVector / rigidbody.mass) * Time.fixedDeltaTime;
        float flightDuration = (2 * velocity.y) / Physics.gravity.y;
        float stepTime = flightDuration / _lineSegmentCount;
        _linePoints.Clear();

        int segmentCount = Mathf.RoundToInt(_lineSegmentCount * velocity.magnitude);

        Vector3 currentPosition = startingPoint;
        Vector3 currentVelocity = velocity;

        for (int i = 0; i < segmentCount; i++)
        {
            float stepTimePassed = stepTime * i;
            Vector3 movementVector = new Vector3(
                currentVelocity.x * stepTimePassed,
                currentVelocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed,
                currentVelocity.z * stepTimePassed
            );

            RaycastHit hit;
            if (Physics.Raycast(currentPosition, -movementVector, out hit, movementVector.magnitude))
            {
                _linePoints.Add(hit.point);
                currentPosition = hit.point;
                Vector3 incomingVector = currentVelocity.normalized;
                Vector3 normal = hit.normal;
                Vector3 reflectedVector = Vector3.Reflect(incomingVector, normal);
                float bounciness = 1f;
                Collider collider = hit.collider;
                if (collider != null && collider.sharedMaterial != null)
                {
                    bounciness = collider.sharedMaterial.bounciness;
                }
                float velocityMagnitude = currentVelocity.magnitude;
                float adjustedMagnitude = velocityMagnitude * Mathf.Sqrt(bounciness);
                Vector3 adjustedVelocity = reflectedVector * adjustedMagnitude;
                adjustedVelocity.y -= Physics.gravity.y * stepTimePassed;
                currentVelocity = adjustedVelocity;
            }
            else
            {
                _linePoints.Add(-movementVector + currentPosition);
            }
        }

        _lineRenderer.positionCount = _linePoints.Count;
        _lineRenderer.SetPositions(_linePoints.ToArray());
    }




    public void HideLine()
    {
        _lineRenderer.positionCount = 0;
    }
}