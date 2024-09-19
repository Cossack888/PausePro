using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    bool damageDealt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (!damageDealt)
            {
                playerHealth.TakeDamage(1);
                damageDealt = true;
                Invoke("ResetDamageDealt", 1);
            }

        }
    }

    private void ResetDamageDealt()
    {
        damageDealt = false;
    }
}
