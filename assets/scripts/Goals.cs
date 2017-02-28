using UnityEngine;
using System.Collections;

public class Goals : MonoBehaviour
{
    public bool goal1; // Passed into the true of false for change texture
    public bool itemScored; // Plays sound when player scores
    public int goalNumber; // Used to determine what should be checked for win conditions
    PlayerController players; // Used to refernece the item scored in the player class
    public Transform[] smoke;

    void Start()
    {
        players = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Checks if the item is from the ingredient layer and then stores it in a game object
    void OnTriggerEnter(Collider collider)
    {       
        int layerIngredient = 11;
        if (collider.gameObject.layer == layerIngredient)
            CheckObject(collider.gameObject);
    }    
    
    void CheckObject(GameObject tempObject)
    {
        AudioController.Play ("SFX_goal"); // Tells audio to play
       // AudioController.Play ("SFX_goal"); // Tells audio to play
        PlayParticle();
        float time = 1.5f; 
        Invoke("StopParticle", time);
        AudioController.Play ("SFX_goal"); // Tells audio to play
        AudioController.Play ("UI_Crowd_Whistle");
        itemScored = true;
        players.hasScored = itemScored;
        HUD.GetInstance.ChangeTexture(goal1, tempObject.name.Substring(0, 5)); // Changes the texture of the HUD
    }

    void PlayParticle()
    {
        foreach (Transform t in smoke)
        {
            t.GetComponent<ParticleSystem>().Play();
        }
    }

    void StopParticle()
    {
        foreach (Transform t in smoke)
        {
            t.GetComponent<ParticleSystem>().Stop();
            t.GetComponent<ParticleSystem>().Clear();           
        }
    }
}
