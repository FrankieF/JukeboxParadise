using UnityEngine;
using System.Collections;
using System;

public class IngredientGeneric : MonoBehaviour
{
    public float priority; // How common is the item
    public string parentReference = ""; // For sounds and tray to collide with ingredient
    public bool inTrigger; // Checks to see if A button should turn off

    public GameObject location; // Where to respawn
    Quaternion reset = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f); // Reset rotation
    public UISprite aButton; // Pops up to let players know they can pick the ingredient up
    public ParticleSystem poofParticle = new ParticleSystem();

    public void ReturnToLocation()
    {
        transform.position = location.transform.position;
        IngredientLogic.GetInstance.currentItems--;
        IngredientLogic.GetInstance.items.Add(gameObject);
        transform.localRotation = reset;
        gameObject.SetActive(false);
    }

    public void ChangePriority(int value)
    {
        priority = value;
    }

    public void Pool()
    {
        renderer.enabled = true;
        gameObject.SetActive(false); // Turns off
        transform.position = location.transform.position; // Pools ingredient
        IngredientLogic.GetInstance.currentItems--; // Tells Ingredient Logic ingredient count to decrease by one
        IngredientLogic.GetInstance.usedItems.Add(gameObject); // Adds ingredient to list before it is moved back to regular list
        transform.localRotation = reset;
    }
    
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name.Contains("Floor"))
        {
            poofParticle.Play();
            renderer.enabled = false;
            Invoke("Pool", .5f);
        }
    }

	void OnTriggerEnter(Collider other)
    {
		if (other.tag == "Volume" || other.tag == "Goals")
        {
            Pool();

        } 
        else if (GetComponent<Objects>().obtainable)
        {
            if(other.transform.parent != null)
            {
                if(other.transform.parent.name.Contains("Player"))
                {
                    aButton.gameObject.SetActive(true);
                    inTrigger = true;
                }
            }
        }
	}

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent != null && inTrigger)
        {
            if(other.transform.parent.name.Contains("Player"))
            {
                aButton.gameObject.SetActive(false);
                inTrigger = false;
            }
        }
    }
}