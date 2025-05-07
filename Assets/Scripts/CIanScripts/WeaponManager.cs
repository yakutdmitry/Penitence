using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs; // weapon array
    private BaseWeapon currentWeapon; // 
    public Camera playerCamera;
    public GameObject crossbowUI;
    public GameObject pistolUI;
    public GameObject SyringerUI;

    private SpriteAnimatorUI currentWeaponAnimator; 

    void Start()
    {
        EquipWeapon(0); 
    }

    void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(0); // pistol, its on 1
            pistolUI.SetActive(true);
            crossbowUI.SetActive(false);
            SyringerUI.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(1); // crossbow, on 2
            pistolUI.SetActive(false);
            crossbowUI.SetActive(true);
            SyringerUI.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeapon(2); //Syringer on 3
            SyringerUI.SetActive(true);
            pistolUI.SetActive(false);
            crossbowUI.SetActive(false);
        }


        if (Input.GetButton("Fire1") && currentWeapon != null)
        {
            currentWeapon.Fire(); 
            // animation for the weapon (trigger)
            if (currentWeaponAnimator != null)
            {
                currentWeaponAnimator.PlayShootAnimation();
            }
        }
    }

    void EquipWeapon(int index)
    {
        if (index >= 0 && index < weaponPrefabs.Length)
        {
     
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
            }

            GameObject weaponInstance = Instantiate(weaponPrefabs[index], transform.position, transform.rotation);


            currentWeapon = weaponInstance.GetComponent<BaseWeapon>();

            if (currentWeapon != null)
            {
                currentWeapon.playerCamera = playerCamera; 
                currentWeapon.gameObject.SetActive(true);
            }

            if (index == 0)
            {
                currentWeaponAnimator = pistolUI.GetComponent<SpriteAnimatorUI>();
            }
            else if (index == 1)
            {
                currentWeaponAnimator = crossbowUI.GetComponent<SpriteAnimatorUI>();
            }
            else
            {
                //this is where we insert the other weapon anims if needed
                currentWeaponAnimator = null;
            }
        }
    }
}
