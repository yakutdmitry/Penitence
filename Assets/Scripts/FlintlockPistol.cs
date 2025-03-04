using UnityEngine;

public class FlintlockPistol : BaseWeapon
{
    public float shotRange = 50f;
    public GameObject impactEffect;
    public LineRenderer lineRenderer; // LineRenderer to visualize the shot path

    private bool canFireSecondShot = false;
    private float secondShotTime = 0.1f;
    private bool isCooldownActive = false;

    public override void Fire()
    {
        if (Time.time >= nextFireTime && HasAmmo()) // Check if it's time to fire and there's ammo
        {
            // Fire the first shot only if enough time has passed since the last shot
            Debug.Log("Firing first shot...");
            FireShot();

            // Fire the second shot after a short delay
            canFireSecondShot = true;
            nextFireTime = Time.time + fireRate;
        }
        else if (canFireSecondShot && Time.time >= nextFireTime)
        {
            Debug.Log("Firing second shot...");
            FireShot();

            canFireSecondShot = false;
            nextFireTime = Time.time + secondShotTime; // cooldown for 2nd shot
        }
        else if (!isCooldownActive)
        {
            // Only log this once per cooldown
            Debug.Log("Can't fire yet. Cooldown in progress.");
            isCooldownActive = true; // Mark the cooldown as active
        }
    }

    private void FireShot()
    {
        // Start and end points for the line
        Vector3 rayStart = playerCamera.transform.position; // Start point is the camera position (where the gun is aiming)
        Vector3 rayEnd = rayStart + playerCamera.transform.forward * shotRange; // Default end point (no collision)

        // Enable the LineRenderer to show the bullet trajectory
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2; // Ensure 2 points are set for the LineRenderer
        lineRenderer.SetPosition(0, rayStart); // Start point of the line (camera position)
        lineRenderer.SetPosition(1, rayEnd);   // Default end point (no collision)

        RaycastHit hit;

        // Check if the ray hits something (for collision detection)
        if (Physics.Raycast(rayStart, playerCamera.transform.forward, out hit, shotRange))
        {
            // If we hit something, update the line's end point to the hit point
            lineRenderer.SetPosition(1, hit.point);

            // Optional: Instantiate an impact effect at the hit point
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); // Impact effect
            }

            Debug.Log("Shot hit something at: " + hit.point); // Log where it hit
        }
        else
        {
            // If the ray didn't hit anything, the line will reach the maximum shot range
            Debug.Log("Shot didn't hit anything.");
        }

        // Decrease ammo and play sound after firing
        DecreaseAmmo();
        PlayShootSound();

        // Disable the LineRenderer after a brief moment (e.g., 0.2 seconds)
        StartCoroutine(DisableLineRenderer());
    }

    private System.Collections.IEnumerator DisableLineRenderer()
    {
        // Wait for 0.2 seconds (increase the time for better visibility)
        yield return new WaitForSeconds(2f);
        lineRenderer.enabled = false;
    }

    // Call this method to reset the cooldown logging flag
    private void Update()
    {
        // resets cooldown
        if (Time.time >= nextFireTime)
        {
            isCooldownActive = false;
        }
    }
}
