using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] GameObject broken;
    [SerializeField] GameObject whole;
    public GameObject VFX;
    public GameObject fire;
    public GameObject tempFire;
    public float explosionRadius = 10f;
    public LayerMask enemyLayer;
    public bool riggedToDestroy;
    private void Start()
    {
        broken.SetActive(false);
    }
    public void RiggExplosion()
    {
        riggedToDestroy = true;
        tempFire = Instantiate(fire, transform.position, Quaternion.identity);
    }

    public void FallApart()
    {
        broken.SetActive(true);
        whole.SetActive(false);
        Destroy(tempFire);
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

        DetectEnemies();
    }

    private void DetectEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Health>().TakeDamage(200);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the explosion radius in the scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
