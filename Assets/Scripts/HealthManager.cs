using UnityEngine;
using TMPro;

public class HealthManager : MonoBehaviour, iDamageable
{
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    public TextMeshProUGUI healthText;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + CurrentHealth.ToString("F0");
        }
    }

    public void TakeDamage(float damageAmount)
    {
        //Debug.Log($"{gameObject.name} took {damageAmount} damage! Current health: {CurrentHealth - damageAmount}");

        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
         //   Debug.Log($"{gameObject.name} died! Calling Die().");
            Die();  // This should remove the enemy
        }
    }

    public void Die()
    {
       // Debug.Log($"{gameObject.name} is being destroyed!");

        Destroy(gameObject);
    }

}
