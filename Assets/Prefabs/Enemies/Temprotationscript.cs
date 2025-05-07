using UnityEngine;

public class Temprotationscript : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        GameObject orientationObject = GameObject.Find("orientation");

        if (orientationObject != null)
        {
            player = orientationObject.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            //this makes the enemy just look straight at the player on the x axis, the other commented out one removes the limit
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPosition);

            //transform.LookAt (transform.position);
        }
    }
}
