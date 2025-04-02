using UnityEngine;
using UnityEngine.UI;

public class WeaponUISelector : MonoBehaviour
{
    public Image[] weaponImages; // Assign UI images in the inspector
    private int currentWeaponIndex = 0;

    void Start()
    {
        currentWeaponIndex = PlayerPrefs.GetInt("currentWeapon", 0);
        UpdateWeaponUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) ChangeWeapon(0);
        if (Input.GetKeyDown(KeyCode.E)) ChangeWeapon(1);
        if (Input.GetKeyDown(KeyCode.R)) ChangeWeapon(2);
        if (Input.GetKeyDown(KeyCode.F)) ChangeWeapon(3);
        if (Input.GetKeyDown(KeyCode.T)) ChangeWeapon(4);
    }

    void ChangeWeapon(int newIndex)
    {
        if (newIndex >= 0 && newIndex < weaponImages.Length)
        {
            weaponImages[currentWeaponIndex].gameObject.SetActive(false); // Disable previous
            currentWeaponIndex = newIndex;
            weaponImages[currentWeaponIndex].gameObject.SetActive(true);  // Enable new
            PlayerPrefs.SetInt("currentWeapon", currentWeaponIndex);
        }
    }

    void UpdateWeaponUI()
    {
        for (int i = 0; i < weaponImages.Length; i++)
        {
            weaponImages[i].gameObject.SetActive(i == currentWeaponIndex);
        }
    }
}
