using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerTakeDmg(20);
            Debug.Log(GameManager.gameManager.playerHealth.Health);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerHeal(10);
            Debug.Log(GameManager.gameManager.playerHealth.Health);
        }
    }

    private void PlayerTakeDmg(int dmg)
    {
        GameManager.gameManager.playerHealth.DmgUnit(dmg);
        Debug.Log("Player took " + dmg + " damage");
    }

    private void PlayerHeal(int healing)
    {
        GameManager.gameManager.playerHealth.HealUnit(healing);
        Debug.Log("Player healed for " + healing + " health");
    }
}
