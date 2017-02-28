using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Picking : MonoBehaviour 
{
    public string showButton = "ShowAButton";
    public int ingredients = 11;
    public List<GameObject> objs = new List<GameObject>();

    // The trigger will add the ingredients to a list
    // The list is then sent to the playerController class
    // Using the list the players can pick up ingredients
    // When an ingredient leaves the trigger, they are removed from the list
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == ingredients)
        {
            objs.Add(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("Ingredients"))
            objs.Remove(collider.gameObject);
    }
}
