using UnityEngine;
using TMPro;  // Include TextMeshPro namespace

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public TextMeshProUGUI healthText;  // Reference to the TextMeshPro UI element

    void Start()
    {
        // Initialize health at the start of the game
        currentHealth = maxHealth;

        // Optionally, update the health display immediately
        UpdateHealthUI();
    }

    // This function updates the health display text
    void UpdateHealthUI()
    {
        healthText.text = "Health: " + currentHealth.ToString("F0");  // "F0" makes it an integer
    }

    // Function to decrease health (you can call this from your player or enemy script)
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        // Update the health UI after taking damage
        UpdateHealthUI();
    }

    // Optional: Function to heal the player
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        // Update the health UI after healing
        UpdateHealthUI();
    }
}
