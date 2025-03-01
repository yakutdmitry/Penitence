using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerTakeDmg(20);
            //Debug.Log(GameManager.gameManager.playerHealth.Health);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerHeal(10);
            Debug.Log(GameManager.gameManager.playerHealth.Health);
        }
    }

    private void PlayerTakeDmg(int dmgAmount)
    {
        GameManager.gameManager.playerHealth.DmgUnit(dmgAmount);
        Debug.Log(GameManager.gameManager.playerHealth.Health);
    }

    private void PlayerHeal(int healing)
    {
        GameManager.gameManager.playerHealth.HealUnit(healing);
        Debug.Log("Player healed for " + healing + " health");
    }
}
