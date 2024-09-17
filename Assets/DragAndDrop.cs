using UnityEditor;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    Rigidbody rb;
    bool isShot;
    Vector3 forceV;
    float force;
    [SerializeField] float initialForce;
    public bool held;
    public GameObject cam;
    NewTrajectory trajectory;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        trajectory = GetComponent<NewTrajectory>();
        trajectory.HideLine();
    }

    public void SetForce()
    {
        force = initialForce;
    }
    private void Update()
    {
        if (held)
        {

            if (Input.GetMouseButton(0))
            {
                force += Time.deltaTime * 75f;
            }
            if (Input.GetMouseButton(1))
            {
                force -= Time.deltaTime * 75f;
            }

            if (Input.GetMouseButton(2))
            {
                Shoot();
                force = 0;
            }
            if (held && !isShot)
            {
                CalculateForceVector();
                trajectory.UpdateTrajectory(forceV, rb, transform.position);
            }
            else
            {
                trajectory.HideLine();
            }
        }
    }


    void CalculateForceVector()
    {
        Vector3 forwardForce = cam.transform.forward * force * forceMultiplierForward;
        Vector3 upwardsForce = Vector3.up * force * forceMultiplierUpwards;
        forceV = forwardForce + upwardsForce;
    }

    [SerializeField]
    [Range(1f, 10f)]
    float forceMultiplierForward = 1;

    [SerializeField]
    [Range(1f, 10f)]
    float forceMultiplierUpwards = 1;
    void Shoot()
    {
        if (isShot)
        {
            return;
        }
        held = false;
        GetComponent<CarriedObject>().carried = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        transform.SetParent(null);
        rb.AddForce(forceV);
        forceV = Vector3.zero;
        isShot = true;
        Invoke("ResetShot", 3);
    }

    void ResetShot()
    {
        isShot = false;
    }
}

