using UnityEngine;
using System.Collections;

public class TrayAnimationEvents : MonoBehaviour 
{
    public Tray tray;

    void SetHitFalse()
    {
        tray.GetComponent<Tray>().tray.SetBool("hit", false);
        tray.GetComponent<Tray>().lid.SetBool("hit", false);
    }

    void Trash()
    {
        tray.GetComponent<Tray>().tray.SetBool("wrongIngredient", false);        
    }
}
