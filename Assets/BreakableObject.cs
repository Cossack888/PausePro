using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] GameObject broken;
    [SerializeField] GameObject whole;
    public GameObject VFX;
    private void Start()
    {
        broken.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            FallApart();
        }
    }
    public void FallApart()
    {
        broken.SetActive(true);
        whole.SetActive(false);
        GameObject vfx = Instantiate(VFX, transform.position, Quaternion.identity);
        Destroy(vfx, 2);

        foreach (Transform t in broken.transform)
        {
            if (t.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Vector3 randomDirection = Random.onUnitSphere;
                float forceMagnitude = Random.Range(5f, 30f);
                rb.AddForce(randomDirection * forceMagnitude, ForceMode.Impulse);
                Vector3 randomTorque = new Vector3(
                    Random.Range(-100f, 100f),
                    Random.Range(-100f, 100f),
                    Random.Range(-100f, 100f)
                );
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
        }
    }

}
