using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    public Text healthText, rofText, attackText, cooldownText, abilityText, lockedText;
    public int upgradePoints = 10; // Example currency

    private void Start()
    {
        UpdateUI();
    }

    public void BuyHealthBoost()
    {
        if (upgradePoints >= 5)
        {
            PermanentUpgradeManager.Instance.healthBoost += 10;
            upgradePoints -= 5;
            PermanentUpgradeManager.Instance.SaveUpgrades();
            UpdateUI();
        }
    }

    public void BuyRoFBoost()
    {
        if (upgradePoints >= 5)
        {
            PermanentUpgradeManager.Instance.rateOfFireBoost += 0.1f;
            upgradePoints -= 5;
            PermanentUpgradeManager.Instance.SaveUpgrades();
            UpdateUI();
        }
    }

    public void UnlockAbility(int abilityID)
    {
        if (upgradePoints >= 10)
        {
            PermanentUpgradeManager.Instance.UnlockAbility(abilityID);
            upgradePoints -= 10;
            UpdateUI();
        }
    }

    public void LockUpgrade()
    {
        if (upgradePoints >= 15)
        {
            PermanentUpgradeManager.Instance.PermaLockUpgrade();
            upgradePoints -= 15;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        healthText.text = "Health Boost: " + PermanentUpgradeManager.Instance.healthBoost;
        rofText.text = "Rate of Fire Boost: " + PermanentUpgradeManager.Instance.rateOfFireBoost;
        attackText.text = "Attack Damage Boost: " + PermanentUpgradeManager.Instance.attackDamageBoost;
        cooldownText.text = "Cooldown Speed Boost: " + PermanentUpgradeManager.Instance.cooldownSpeedBoost;
        abilityText.text = "Unlocked Ability: " + PermanentUpgradeManager.Instance.unlockedBaseAbility;
        lockedText.text = "Locked Upgrades: " + PermanentUpgradeManager.Instance.lockedUpgrades;
    }
}
