using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public int HealthAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the HealthManager component from the player
            HealthManager healthManager = other.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                // Increase the player's health
                healthManager.ModifyHealth(HealthAmount);
            }
            // Destroy the health pickup object
            Destroy(gameObject);
        }
    }
}
