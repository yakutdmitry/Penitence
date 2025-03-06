using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public Camera playerCamera; 
    public AudioClip shootSound; 
    public int maxAmmo; 
    protected int currentAmmo;
    public float fireRate; 
    protected float nextFireTime = 0f;

    public abstract void Fire(); 

    public virtual void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; 
            if (playerCamera == null)
            {
                Debug.LogError("No camera found. Please assign a camera to the playerCamera field.");
            }
        }

        currentAmmo = maxAmmo; 
    }

    public void PlayShootSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(shootSound); 
        }
    }

    public void DecreaseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
        }
    }

    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    // Debugging method to display firing status
    public void DebugFire(bool canFire)
    {
        if (canFire)
        {
            Debug.Log($"{gameObject.name} fired successfully.");
        }
        else
        {
            if (Time.time < nextFireTime)
            {
                Debug.Log($"{gameObject.name} failed to fire: Cooldown in progress (next fire time: {nextFireTime})");
            }
            else if (!HasAmmo())
            {
                Debug.Log($"{gameObject.name} failed to fire: No ammo (current ammo: {currentAmmo})");
            }
            else
            {
                Debug.Log($"{gameObject.name} failed to fire for an unknown reason.");
            }
        
    }

}
}
