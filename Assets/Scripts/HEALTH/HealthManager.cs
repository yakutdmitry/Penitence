using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour, iDamageable
{
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    public TextMeshProUGUI healthText;

    // Invincibility fields
    public float invincibilityTime = 1f; // Time after taking damage where player is invincible
    private bool isInvincible = false;

    private void Start()
    {
        MaxHealth = GameManager.Instance.playerHealth;
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
        if (!isInvincible || !CompareTag("Player"))
        {
            CurrentHealth -= damageAmount;
            if (CompareTag("Player"))
            {
                GameManager.Instance.ModifyHealth((int)-damageAmount); // Update GameManager
            }

            if (CurrentHealth <= 0)
            {
                Die();
            }
            else if (CompareTag("Player"))
            {
                // Apply temporary invincibility after taking damage
                StartCoroutine(InvincibilityFrames());
            }
        }
    }

    // Method to modify health
    public void ModifyHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        if (CompareTag("Player"))
        {
            GameManager.Instance.ModifyHealth(amount); // Update GameManager
        }
        UpdateHealthUI();
    }

    // Coroutine to handle invincibility after taking damage
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    public void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("Player has died.");
            // Reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
        else
        {
            Debug.Log(gameObject.name + " has died.");
            // Handle enemy death (e.g., drop loot, destroy object)
            Destroy(gameObject);
        }
    }
}