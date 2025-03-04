using UnityEngine;

public class SpreadCrossbow : BaseWeapon
{
    public int bolts = 5;
    public float shotRange = 50f;
    public GameObject impactEffect;
    public LineRenderer lineRendererPrefab; // LineRenderer prefab to be instantiated for each shot
    public float spreadMultiplier = 0.2f; // Control the spread of the bolts

    public override void Fire()
    {
        if (Time.time >= nextFireTime && HasAmmo())
        {
            for (int i = 0; i < bolts; i++) // Loop through each bolt
            {
                // Math for spread (adjusted for a wider spread)
                Vector3 spread = Random.insideUnitSphere * spreadMultiplier; // Increase the spread multiplier to make it wider

                // Start and end points for the line
                Vector3 rayStart = playerCamera.transform.position + spread; // Start point with spread applied
                Vector3 rayEnd = rayStart + playerCamera.transform.forward * shotRange; // Default end point (no collision)

                // Instantiate a new LineRenderer for each bolt
                LineRenderer currentLine = Instantiate(lineRendererPrefab);
                currentLine.positionCount = 2; // Ensure 2 points are set for the LineRenderer
                currentLine.SetPosition(0, rayStart); // Set start point
                currentLine.SetPosition(1, rayEnd);   // Set default end point

                RaycastHit hit;

                // Check if the ray hits something (for collision detection)
                if (Physics.Raycast(rayStart, playerCamera.transform.forward, out hit, shotRange))
                {
                    // If we hit something, update the line's end point to the hit point
                    currentLine.SetPosition(1, hit.point);

                    // Optional: Instantiate an impact effect at the hit point
                    if (impactEffect != null)
                    {
                        Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); // Impact effect
                    }

                    Debug.Log("Bolt hit something at: " + hit.point); // Log where it hit
                }
                else
                {
                    Debug.Log("Bolt didn't hit anything.");
                }

                // Decrease ammo after firing this bolt
                DecreaseAmmo();

                // Destroy the line after a brief period
                Destroy(currentLine.gameObject, 2f); // Destroy the line after 0.2 seconds (adjust time as necessary)
            }

            nextFireTime = Time.time + fireRate; // Set the cooldown to control the next shot
            PlayShootSound(); // Play the shoot sound after firing
        }
    }
}
