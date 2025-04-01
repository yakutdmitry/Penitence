using UnityEngine;

public class GatlingSyringer : BaseWeapon
{
    public float syringeRange = 50f;
    public float syringeDamage = 10f;
    public GameObject impactEffect;
    public float tickEffect = 3f; 
    public float sloweffectPercentage = 0.5f; 
    public LineRenderer lineRenderer;

    private bool isCooldownActive = false;

    public override void Fire()
    {
        if (Time.time >= nextFireTime && HasAmmo()) // checks 4 cooldown
        {
            FireShot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void FireShot()
    {
        Vector3 rayStart = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, rayStart);

        if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, syringeRange))
        {
            lineRenderer.SetPosition(1, hit.point);

            Debug.Log($"Shot hit {hit.collider.gameObject.name} at {hit.point}");

            // Get the damageable component from the hit object
            iDamageable damageable = hit.collider.GetComponentInParent<iDamageable>();

            if (damageable != null)
            {
                // Apply damage to the enemy
                damageable.TakeDamage(syringeDamage);

                // If the object is an enemy, apply the slow effect
                if (hit.collider.GetComponentInParent<Enemy>() != null)
                {
                    Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                    
                }
            }
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        else
        {
            lineRenderer.SetPosition(1, rayStart + rayDirection * syringeRange);
        }

        DecreaseAmmo();
        PlayShootSound();
        StartCoroutine(DisableLineRenderer());
    }

    private System.Collections.IEnumerator DisableLineRenderer()
    {
        yield return new WaitForSeconds(2f);
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        // resets cooldown
        if (Time.time >= nextFireTime)
        {
            isCooldownActive = false;
        }
    }
}
