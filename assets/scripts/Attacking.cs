using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attacking : MonoBehaviour
{
    public List<GameObject> colliders = new List<GameObject>();
 
    // The trigger will add the players to a list
    // The list is then sent to the playerController class
    // Using the list the players can attack other players
    // When a player leaves the trigger, they are removed from the list
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Tray"))
        {
            colliders.Add(col.gameObject);
        }
    }
    
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player") || (col.gameObject.CompareTag("Tray")))
        {
            colliders.Remove(col.gameObject);
        }
    }
}
