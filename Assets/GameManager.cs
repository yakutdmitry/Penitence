using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject doorwayGenerationPrefab;


    // Player Stats

    [Header("Player Stats")]

    public int health = 100;
    public int maxHealth = 100;
    public int gunAmmo = 30;
    public int maxGunAmmo = 30;
    public int crossbowAmmo = 30;
    public int maxCrossbowAmmo = 30;
    public int score = 0;

    // Upgrades & Abilities

    [Header("Upgrades & Abilities")]
    public List<string> upgrades = new List<string>();
    public List<string> abilities = new List<string>();


    [Header("Permanent Upgrades")]
    public int playerHealth = 100;
    public float rateOfFire = 1.0f;
    public int attackDamage = 10;
    public float cooldownSpeed = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keeps it alive through scene changes
        }
        else
        {
            Destroy(gameObject);  // Prevents duplicate GameManagers
        }
    }

    public void ModifyHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        Debug.Log("Health Updated: " + health);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score Updated: " + score);
    }

    public void ModifyAmmo(int amount)
    {
        // Check if the picked up ammo is for the gun or crossbow
        if (amount > 0)
        {
            gunAmmo = Mathf.Clamp(gunAmmo + amount, 0, maxGunAmmo);
            Debug.Log("Gun Ammo Updated: " + gunAmmo);
        }
        else
        {
            crossbowAmmo = Mathf.Clamp(crossbowAmmo + amount, 0, maxCrossbowAmmo);
            Debug.Log("Crossbow Ammo Updated: " + crossbowAmmo);
        }

    }

    public void AddUpgrade(string upgradeName)
    {
        if (!upgrades.Contains(upgradeName))
        {
            upgrades.Add(upgradeName);
            Debug.Log("Upgrade Acquired: " + upgradeName);
        }
    }

    public void AddAbility(string abilityName)
    {
        if (!abilities.Contains(abilityName))
        {
            abilities.Add(abilityName);
            Debug.Log("Ability Unlocked: " + abilityName);
        }
    }

    public void StartNewRun()
    {
        // Apply permanent upgrades
        PermanentUpgradeManager.Instance.ApplyUpgrades(ref playerHealth, ref rateOfFire, ref attackDamage, ref cooldownSpeed);
    }
}
