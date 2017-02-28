using UnityEngine;
using System.Collections;

public class FlipAnimationEvents : MonoBehaviour
{
    void PickIngredients()
    {
        HUD.GetInstance.ChangeBackTextures();
    }

    void SetIngredients()
    {
        HUD.GetInstance.ChangeFrontTextures();
    }

    void FlipSFX()
    {
        AudioController.Play ("SFX_Scoreboard"); 
    }
}
