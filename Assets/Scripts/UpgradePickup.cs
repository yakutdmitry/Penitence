using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePickup : MonoBehaviour
{
    private GameManager gameManager;
    public string abilityName; // The name of the ability to be added


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
            // Check if the player already has the ability
            if (!gameManager.abilities.Contains(abilityName))
            {
                // Add the ability to the GameManager
                gameManager.abilities.Add(abilityName);
                Debug.Log("Ability " + abilityName + " added to player.");

                // Try to enable the matching script on the Player
                var ability = other.GetComponent(abilityName);
                if (ability != null && ability is MonoBehaviour mono)
                {
                    mono.enabled = true;
                    Debug.Log("Enabled ability script: " + abilityName);
                }
                else
                {
                    Debug.LogWarning("Ability script not found on player: " + abilityName);
                }

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Player already has this ability.");
            }
        }
    }


}
