using UnityEngine;
using System.Collections;

public class Ginger : PlayerController
{
    override public void Start()
    {
        base.Start();
        characterName = "Ginger";
    }

    override protected void punchingVO()
    {
        voTimer = Random.Range (1, 20);
        if (voTimer >= 5) 
        {AudioController.Play ("CHAR_Ginger_Punch", 0.4f, 0,0); }
        
    }

    override protected void punchedVO()
    {
        if (hitCounter <= 2) {
            AudioController.Play ("CHAR_Ginger_Ow"); 
        }
//        if(force == strongAttack)
//            {AudioController.Play ("CHAR_Ginger_BigOw");}
//     
//        else
//            {AudioController.Play ("CHAR_Ginger_Ow");}
    }

    override protected void throwObjectVO()
    { 
        voTimer = Random.Range (1, 10);
        if (voTimer >= 5) 
            { AudioController.Play ("CHAR_Ginger_ObjectThrow"); }
    }

    override protected void respawnVO()
    {
        AudioController.Play ("CHAR_Ginger_Respawn"); 
    }

    override protected void stageOffVO()
    { 
        AudioController.Play ("CHAR_Ginger_StageOff"); 
    }

    override protected void guitarUseVO()
    {
        AudioController.Play ("CHAR_Ginger_GuitarUse"); 
    }

    override protected void dazedVO()
    {
        AudioController.Play ("CHAR_Ginger_Dazed"); 
    
    }
//    override protected void shieldVO()
//    {
//        if(shield.bShieldObject == true){
//            AudioController.Play ("CHAR_Sophie_Shield");
//        }
//    }
}
