using UnityEditor;
using UnityEngine;

public class Carrying : MonoBehaviour
{
    public GameObject crosshair1;
    public GameObject crosshair2;
    GameObject carriedObject;
    public Transform hands;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Throwable"))
        {
            crosshair1.SetActive(false);
            crosshair2.SetActive(true);
            carriedObject = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Throwable"))
        {
            crosshair1.SetActive(true);
            crosshair2.SetActive(false);
            carriedObject = null;
        }
    }
    private void Update()
    {
        if (crosshair2.activeSelf)
        {
            if (Input.GetMouseButtonDown(1))
            {
                carriedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                Vector3 cameraForward = transform.forward;
                cameraForward.y = 0; // Remove any vertical component to ensure alignment with the ground
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

                // Set rotation to match camera's forward direction
                carriedObject.transform.rotation = targetRotation;
                carriedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                carriedObject.GetComponent<Rigidbody>().useGravity = false;
                carriedObject.GetComponent<CarriedObject>().carried = true;
                carriedObject.transform.SetParent(hands);
                carriedObject.GetComponent<DragAndDrop>().held = true;
                carriedObject.GetComponent<DragAndDrop>().SetForce();
                crosshair2.SetActive(false);

            }
        }
    }


}