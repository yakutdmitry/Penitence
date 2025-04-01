using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs; // weapon array
    private BaseWeapon currentWeapon; // 
    public Camera playerCamera;

    void Start()
    {
        EquipWeapon(0); 
    }

    void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(0); // pistol, its on 1
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(1); // crossbow, on 2
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeapon(2); //Syringer on 3
        }


        if (Input.GetButton("Fire1") && currentWeapon != null)
        {
            currentWeapon.Fire(); 
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
        }
    }
}
