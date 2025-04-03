using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public int HealthAmount = 1;
    public AudioClip healthPickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.ModifyHealth(HealthAmount);
                GameManager.Instance.ModifyHealth(HealthAmount); // Update GameManager
                AudioSource.PlayClipAtPoint(healthPickupSound, transform.position);
                Destroy(gameObject);
            }
        }
    }
}
