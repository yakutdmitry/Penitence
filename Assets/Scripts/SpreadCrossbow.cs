using UnityEngine;
using System.Collections;

public class SpreadCrossbow : BaseWeapon
{
    public int bolts = 5;
    public float shotRange = 50f;
    public float shotDamage = 10f;
    public GameObject impactEffect;
    public LineRenderer lineRendererPrefab; // LineRenderer prefab to be instantiated for each shot
    public float spreadMultiplier = 0.2f; // Controls the spread of the bolts

    public override void Fire()
    {
        if (Time.time >= nextFireTime && HasAmmo())
        {
            for (int i = 0; i < bolts; i++) // Fire multiple bolts
            {
                Vector3 spread = playerCamera.transform.right * Random.Range(-spreadMultiplier, spreadMultiplier) +
                                 playerCamera.transform.up * Random.Range(-spreadMultiplier, spreadMultiplier);

                Vector3 rayStart = playerCamera.transform.position;
                Vector3 rayDirection = (playerCamera.transform.forward + spread).normalized;

                RaycastHit hit;
                if (Physics.Raycast(rayStart, rayDirection, out hit, shotRange))
                {
                    Debug.Log("Bolt hit: " + hit.collider.gameObject.name + " at " + hit.point);

                    // Damage enemy
                    iDamageable damageable = hit.collider.GetComponentInParent<iDamageable>();
                    if (damageable != null)
                    {
                        Debug.Log("Crossbow hit an enemy! Applying " + shotDamage + " damage.");
                        damageable.TakeDamage(shotDamage);
                    }
                    else
                    {
                        Debug.LogWarning("Hit object does not implement iDamageable.");
                    }

                    // Spawn impact effect
                    if (impactEffect != null)
                    {
                        Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    // Draw line for visual feedback
                    StartCoroutine(DrawShotLine(rayStart, hit.point));
                }
                else
                {
                    // Draw line to max range if nothing is hit
                    StartCoroutine(DrawShotLine(rayStart, rayStart + rayDirection * shotRange));
                }
            }

            nextFireTime = Time.time + fireRate; // Apply fire cooldown
            PlayShootSound(); // Play firing sound
        }
    }

    private IEnumerator DrawShotLine(Vector3 start, Vector3 end)
    {
        LineRenderer shotLine = Instantiate(lineRendererPrefab);
        shotLine.positionCount = 2;
        shotLine.SetPosition(0, start);
        shotLine.SetPosition(1, end);

        yield return new WaitForSeconds(2f); // Delay before disappearing
        Destroy(shotLine.gameObject);
    }
}
