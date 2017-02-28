using UnityEngine;
using System.Collections;

public class Sophia : PlayerController
{
    override public void Start()
    {
        base.Start();

        characterName = "Sophia";
    }

    override protected void punchedVO()
    {
        if (hitCounter <=2) {
            AudioController.Play ("CHAR_Sophie_Ow"); 
        }

//        if(force == strongAttack)
//        {AudioController.Play ("CHAR_Sophie_BigOw");
//            Debug.Log ("Sophie Big Ow");}
//        
//        else
//        {AudioController.Play ("CHAR_Sophie_Ow");}
    }
    
    override protected void punchingVO()
    {
        voTimer = Random.Range (1, 20);
        if (voTimer >= 5) 
        {AudioController.Play ("CHAR_Sophie_Punch", 0.5f, 0, 0
                                   ); }
       
    }
    
    override protected void throwObjectVO()
    { 
        voTimer = Random.Range (1, 10);
        if (voTimer >= 5) 
            {AudioController.Play ("CHAR_Sophie_ObjectThrow"); }
    }
    
    override protected void respawnVO()
    {
        AudioController.Play ("CHAR_Sophie_Respawn"); 
    }
    
    override protected void stageOffVO()
    { 
        AudioController.Play ("CHAR_Sophie_StageOff"); 
    }

    override protected void guitarUseVO()
    {
        AudioController.Play ("CHAR_Sophie_GuitarUse"); 
    }

    override protected void dazedVO()
    {
        AudioController.Play ("CHAR_Sophie_Tripped"); 
    }
//    override protected void shieldVO()
//    {
//        if(shield.bShieldObject == true){
//            AudioController.Play ("CHAR_Sophie_Shield");
//        }
//    }
}
