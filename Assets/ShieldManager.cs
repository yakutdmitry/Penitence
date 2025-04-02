using UnityEngine;
using TMPro;

public class ShieldManager : MonoBehaviour
{
    public float MaxShield { get; set; } = 100f;  // Base shield value
    public float CurrentShield { get; set; }
    public TextMeshProUGUI shieldText;  // UI for shield display

    private HealthManager healthManager;  // Reference to HealthManager

    private void Start()
    {
        // Get HealthManager component
        healthManager = GetComponent<HealthManager>();

        // Initialize shield to max value
        CurrentShield = MaxShield;
        UpdateShieldUI();
    }

    private void Update()
    {
        // Update the shield UI
        UpdateShieldUI();
    }

    private void UpdateShieldUI()
    {
        // Display shield value in UI
        if (shieldText != null)
        {
            shieldText.text = "Shield: " + CurrentShield.ToString("F0");
        }
    }

    public void TakeDamage(float damageAmount)
    {
        // First, absorb damage with shield
        if (CurrentShield > 0)
        {
            float shieldDamage = Mathf.Min(damageAmount, CurrentShield);
            CurrentShield -= shieldDamage;
            damageAmount -= shieldDamage;  // Remaining damage
        }

        // If shield is depleted, apply damage to health
        if (damageAmount > 0)
        {
            healthManager.TakeDamage(damageAmount);
        }
    }
}
