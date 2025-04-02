using UnityEngine;
using System.Collections;

public class Meathook_script : MonoBehaviour
{
    public float MeathookRange = 20f;
    public LineRenderer TendrilRenderer;
    public Transform playerTransform;
    public Rigidbody playerRb;
    public Transform MeathookOrigin;
    private Vector3 grapplePoint;
    private bool isGrappling = false;
    private bool canGrapple = true;  
    public float grappleSpeed = 15f;  
    public float cooldownTime = 5f;  


    void Start()
    {
        playerRb = playerTransform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //E's the keybind for the grapple, but
        if (Input.GetKeyDown(KeyCode.E) && canGrapple && !isGrappling)
        {
            ShootGrapple();
        }
        if (isGrappling)
        {
            MoveTowardsGrapplePoint();
        }
    }

    void ShootGrapple()
    {
        RaycastHit hit;
        Vector3 rayDirection = Camera.main.transform.forward;

        // draws out the ray in the scene view for debug stuff ill need later, comment it out if not needed
        Debug.DrawRay(playerTransform.position, rayDirection * MeathookRange, Color.green, 2f);

        if (Physics.Raycast(playerTransform.position, rayDirection, out hit, MeathookRange))
        {
            Debug.Log("Meathook attached");

            if (hit.collider.CompareTag("Grappleable"))
            {
                grapplePoint = hit.point;
                isGrappling = true;
                playerRb.useGravity = false;
                TendrilRenderer.positionCount = 2;
                TendrilRenderer.SetPosition(0, MeathookOrigin.position);
                TendrilRenderer.SetPosition(1, grapplePoint);
                StartCoroutine(CooldownRoutine());


            }
        }
        else
        {
            Debug.Log("meathook not attached");
        }
    }



    void MoveTowardsGrapplePoint()
    {
        float step = grappleSpeed * Time.deltaTime;
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, grapplePoint, step);
        TendrilRenderer.SetPosition(0, MeathookOrigin.position);
        TendrilRenderer.SetPosition(1, grapplePoint);

    //kinda inconsistant on removing the line renderer, will fix later once fully functional
        if (playerTransform.position == grapplePoint)
        {
            isGrappling = false;
            playerRb.useGravity = true;
            TendrilRenderer.SetPosition(1, MeathookOrigin.position);
        }
    }

    //cooldowns a lil buggy atm, try and buff out scratches before final sub
    private IEnumerator CooldownRoutine()
    {
        canGrapple = false;
        yield return new WaitForSeconds(cooldownTime);  
        canGrapple = true;
    }
}
