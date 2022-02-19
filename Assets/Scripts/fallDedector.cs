using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallDedector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) // if player reach endline
    {
        if (!other.CompareTag("Player")) // if its bot
        {
            navigateBots botScript = other.GetComponent<navigateBots>();
            other.transform.position = botScript.spawnPoint.position; // set pos back to spawnpoint
        }
        else                    // probably player cause there is no dynamic object can trigger this rn
        {
            characterController playerScript = other.GetComponent<characterController>();
            other.transform.position = playerScript.spawnPoint.position;
        }
    }
}
