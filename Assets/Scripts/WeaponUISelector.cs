using UnityEngine;
using UnityEngine.UI;

public class WeaponUISelector : MonoBehaviour
{
    public Image[] weaponImages;
    public AudioClip[] weaponAudioClips;
    public AudioSource audioSource;
    public float audioCooldown = 1f;

    private int currentWeaponIndex = 0;
    private float[] lastAudioTimes;

    void Start()
    {
        currentWeaponIndex = PlayerPrefs.GetInt("currentWeapon", 0);
        lastAudioTimes = new float[weaponAudioClips.Length];

        for (int i = 0; i < lastAudioTimes.Length; i++)
        {
            lastAudioTimes[i] = -Mathf.Infinity;
        }

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
            weaponImages[currentWeaponIndex].gameObject.SetActive(false);
            currentWeaponIndex = newIndex;
            weaponImages[currentWeaponIndex].gameObject.SetActive(true);

            if (Time.time - lastAudioTimes[currentWeaponIndex] >= audioCooldown)
            {
                audioSource.clip = weaponAudioClips[currentWeaponIndex];
                audioSource.Play();
                lastAudioTimes[currentWeaponIndex] = Time.time;
            }

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
