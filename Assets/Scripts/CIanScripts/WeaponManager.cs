using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject[] weaponPrefabs;  // 0 = Pistol, 1 = Crossbow, 2 = Syringer

    [Header("References")]
    public Camera playerCamera;

    [Header("Weapon UI Canvases")]
    public GameObject pistolUI;
    public GameObject crossbowUI;
    public GameObject syringerUI;

    private BaseWeapon currentWeapon;
    private SpriteAnimatorUI currentWeaponAnimator;

    void Start()
    {
        EquipWeapon(0);
    }

    void Update()
    {
        // Weapon switch input
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        // Fire and animate
        if (Input.GetButtonDown("Fire1") && currentWeapon != null)
        {
            currentWeapon.Fire();
            currentWeaponAnimator?.PlayShootAnimation();
        }
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponPrefabs.Length) return;

        // Destroy old weapon
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        // Instantiate new
        var weaponGO = Instantiate(weaponPrefabs[index], transform.position, transform.rotation);
        currentWeapon = weaponGO.GetComponent<BaseWeapon>();
        currentWeapon.playerCamera = playerCamera;
        currentWeapon.gameObject.SetActive(true);

        // Deactivate all UIs
        pistolUI.SetActive(false);
        crossbowUI.SetActive(false);
        syringerUI.SetActive(false);

        // Activate the one for this weapon and grab its animator
        switch (index)
        {
            case 0:
                pistolUI.SetActive(true);
                currentWeaponAnimator = pistolUI.GetComponent<SpriteAnimatorUI>();
                break;
            case 1:
                crossbowUI.SetActive(true);
                currentWeaponAnimator = crossbowUI.GetComponent<SpriteAnimatorUI>();
                break;
            case 2:
                syringerUI.SetActive(true);
                currentWeaponAnimator = syringerUI.GetComponent<SpriteAnimatorUI>();
                break;
            default:
                currentWeaponAnimator = null;
                break;
        }
    }
}

