using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    private GameManager gameManager;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming you have a method to apply the upgrade
            gameManager.abilities.Add("AbilityName"); // Replace "AbilityName" with the actual ability name
            Destroy(gameObject); // Destroy the pickup after use
        }
    }

}
