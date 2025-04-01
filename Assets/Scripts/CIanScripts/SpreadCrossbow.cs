using UnityEngine;
using System.Collections;

public class SpreadCrossbow : BaseWeapon
{
    public int bolts = 5;
    public float shotRange = 50f;
    public float shotDamage = 10f;
    public GameObject impactEffect;
    public LineRenderer lineRendererPrefab;
    public float spreadMultiplier = 0.2f;

    public override void Fire()
    {
        if (Time.time >= nextFireTime && HasAmmo())
        {
            for (int i = 0; i < bolts; i++)
            {
                Vector3 spread = playerCamera.transform.right * Random.Range(-spreadMultiplier, spreadMultiplier) +
                                 playerCamera.transform.up * Random.Range(-spreadMultiplier, spreadMultiplier);

                Vector3 rayStart = playerCamera.transform.position;
                Vector3 rayDirection = (playerCamera.transform.forward + spread).normalized;

                RaycastHit hit;
                if (Physics.Raycast(rayStart, rayDirection, out hit, shotRange))
                {
                    iDamageable damageable = hit.collider.GetComponentInParent<iDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(shotDamage);
                    }

                    if (impactEffect != null)
                    {
                        Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    StartCoroutine(DrawShotLine(rayStart, hit.point));
                }
                else
                {
                    StartCoroutine(DrawShotLine(rayStart, rayStart + rayDirection * shotRange));
                }
            }

            nextFireTime = Time.time + fireRate;
            PlayShootSound();
        }
    }

    private IEnumerator DrawShotLine(Vector3 start, Vector3 end)
    {
        LineRenderer shotLine = Instantiate(lineRendererPrefab);
        shotLine.positionCount = 2;
        shotLine.SetPosition(0, start);
        shotLine.SetPosition(1, end);

        yield return new WaitForSeconds(2f);
        Destroy(shotLine.gameObject);
    }
}
