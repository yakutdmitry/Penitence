using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs; // Array of weapon prefabs
    private BaseWeapon currentWeapon; // Currently equipped weapon
    public Camera playerCamera; // The player’s camera

    void Start()
    {
        EquipWeapon(0); // Equip the first weapon (pistol) by default
    }

    void Update()
    {
        // Switch weapons with number keys (e.g., 1 for Pistol, 2 for Shotgun)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(0); // Equip Pistol (index 0)
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(1); // Equip Shotgun (index 1)
        }

        // Fire the current weapon
        if (Input.GetButton("Fire1") && currentWeapon != null)
        {
            currentWeapon.Fire(); // Call the Fire method for the current weapon
        }
    }

    void EquipWeapon(int index)
    {
        if (index >= 0 && index < weaponPrefabs.Length)
        {
            // Destroy the current weapon if it exists
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
            }

            // Instantiate the new weapon prefab at the player's position
            GameObject weaponInstance = Instantiate(weaponPrefabs[index], transform.position, transform.rotation);

            // Get the BaseWeapon component from the instantiated prefab
            currentWeapon = weaponInstance.GetComponent<BaseWeapon>();

            // Assign the camera to the new weapon
            if (currentWeapon != null)
            {
                currentWeapon.playerCamera = playerCamera; // Ensure the camera reference is set
                currentWeapon.gameObject.SetActive(true);
            }
        }
    }
}
