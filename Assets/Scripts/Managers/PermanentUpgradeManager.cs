using UnityEngine;

public class PermanentUpgradeManager : MonoBehaviour
{
    public static PermanentUpgradeManager Instance { get; private set; }

    public int healthBoost;
    public float rateOfFireBoost;
    public int attackDamageBoost;
    public float cooldownSpeedBoost;
    public int unlockedBaseAbility; // 0 = None, 1 = Ability A, 2 = Ability B, etc.
    public int lockedUpgrades; // Number of locked upgrades late in the game

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyUpgrades(ref int playerHealth, ref float rof, ref int attackDmg, ref float cooldown)
    {
        playerHealth += healthBoost;
        rof += rateOfFireBoost;
        attackDmg += attackDamageBoost;
        cooldown -= cooldownSpeedBoost;
    }

    public void UnlockAbility(int abilityID)
    {
        unlockedBaseAbility = abilityID;
        SaveUpgrades();
    }

    public void PermaLockUpgrade()
    {
        lockedUpgrades++;
        SaveUpgrades();
    }

    public void SaveUpgrades()
    {
        PlayerPrefs.SetInt("HealthBoost", healthBoost);
        PlayerPrefs.SetFloat("RoFBoost", rateOfFireBoost);
        PlayerPrefs.SetInt("AttackDmgBoost", attackDamageBoost);
        PlayerPrefs.SetFloat("CooldownBoost", cooldownSpeedBoost);
        PlayerPrefs.SetInt("UnlockedAbility", unlockedBaseAbility);
        PlayerPrefs.SetInt("LockedUpgrades", lockedUpgrades);
        PlayerPrefs.Save();
    }

    public void LoadUpgrades()
    {
        healthBoost = PlayerPrefs.GetInt("HealthBoost", 0);
        rateOfFireBoost = PlayerPrefs.GetFloat("RoFBoost", 0);
        attackDamageBoost = PlayerPrefs.GetInt("AttackDmgBoost", 0);
        cooldownSpeedBoost = PlayerPrefs.GetFloat("CooldownBoost", 0);
        unlockedBaseAbility = PlayerPrefs.GetInt("UnlockedAbility", 0);
        lockedUpgrades = PlayerPrefs.GetInt("LockedUpgrades", 0);
    }

    public void ResetUpgrades()
    {
        PlayerPrefs.DeleteAll();
        LoadUpgrades();
    }
}
