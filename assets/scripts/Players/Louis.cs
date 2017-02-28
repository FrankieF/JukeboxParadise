using UnityEngine;
using System.Collections;

public class Louis : PlayerController
{
    override public void Start()
    {
        base.Start();

        characterName = "Louis";
    }

    override protected void punchedVO()
    {
       if (hitCounter <= 2) {
            AudioController.Play ("CHAR_Louis_Ow"); 
        }
//        if(force == strongAttack)
//        {AudioController.Play ("CHAR_Louis_BigOw");
//            Debug.Log ("Sophie Big Ow");}
//        
//        else
//        {AudioController.Play ("CHAR_Louis_Ow");}
    }
    
    override protected void punchingVO()
    {
        voTimer = Random.Range (1, 20);
        if (voTimer >= 5) 
            {AudioController.Play ("CHAR_Louis_Punch");}
    }
        
    override protected void throwObjectVO()
    { 
        voTimer = Random.Range (1, 10);
        if (voTimer >= 5) 
            {AudioController.Play ("CHAR_Louis_ObjectThrow"); }
    }
    
    override protected void respawnVO()
    {
        AudioController.Play ("CHAR_Louis_Respawn"); 
    }
    
    override protected void stageOffVO()
    { 
        AudioController.Play ("CHAR_Louis_StageOff"); 
    }

    override protected void dropObjectVO()
    {
        AudioController.Play ("CHAR_Louis_DropObject"); 
    }

    override protected void guitarUseVO()
    {
        AudioController.Play ("CHAR_Louis_GuitarUse"); 
    }

    override protected void dazedVO()
    {
        AudioController.Play ("CHAR_Louis_Dazed"); 
//        Debug.Log ("Louis is dazed"); 
    }

//    override protected void shieldVO()
//    {
//        if(shield.hasShield == true){
//            AudioController.Play ("CHAR_Sophie_Shield");
//        }
    }

