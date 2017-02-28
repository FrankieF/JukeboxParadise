using UnityEngine;
using System.Collections;

public class AnimationEvents : MonoBehaviour
{
    public PlayerController players;

    /// <summary>
    /// Attack functions
    /// </summary>

    void Move()
    {
//        players.MovePlayerTowardsOpponent();
    }

    void Punch()
    {
        if(players.contact == false)
        {
            players.CheckAttack();
        }
    }

    void Left()
    {
        AudioController.Play ("SFX_LeftRun"); 
    }

    void Right()
    {
        AudioController.Play ("SFX_RightRun"); 
    }

    /// <summary>
    /// Ingredient functions
    /// </summary>

    void Pickup()
    {
        players.PickUpObject();
    }

    void Place()
    {
        players.PlaceObject();
    }

    void Skid()
    {
        AudioController.Play ("SFX_skid");
    }

    void Spin()
    {
        AudioController.Play ("SFX_Spin"); 
    }

    void SetDaze()
    {
        players.animator.SetBool("dazed", false);
    }
}                                                                                                                                                                                                                     
