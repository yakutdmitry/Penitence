using UnityEngine;

public class FlintlockPistol : BaseWeapon
{
    public float shotRange = 50f;
    public float shotDamage = 10f;
    public GameObject impactEffect;
    public LineRenderer lineRenderer; // LineRenderer to visualize the shot path
    private iDamageable enemy;
    private HealthManager healthManager;

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
            isCooldownActive = true; 
        }
    }

    private void FireShot()
    {
        Vector3 rayStart = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, rayStart);

        if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, shotRange))
        {
            lineRenderer.SetPosition(1, hit.point);
            iDamageable damageable = hit.collider.GetComponentInParent<iDamageable>();


            if (damageable != null)
            {

                damageable.TakeDamage(shotDamage); 
            }


            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        else
        {
            lineRenderer.SetPosition(1, rayStart + rayDirection * shotRange);
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
