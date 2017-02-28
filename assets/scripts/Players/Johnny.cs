using UnityEngine;
using System.Collections;

public class Johnny : PlayerController
{
    override public void Start ()
    {
        base.Start();
        characterName = "Johnny";
    }

    override protected void punchedVO()
    {
        
        if (hitCounter <= 2 ) 
        {
            AudioController.Play ("CHAR_Johnny_Ow"); 
        }
        
    }

    protected  override  void punchingVO()
    {
        voTimer = Random.Range (1, 20);
        if (voTimer >= 5) 
            {AudioController.Play ("CHAR_Johnny_Punch", 0.5f, 0,0);}
 
    }
    override protected void stageOffVO()
    { 
        AudioController.Play ("CHAR_Johnny_StageOff"); 
    }

    override protected void throwObjectVO()
    {
        voTimer = Random.Range (1, 10);
        if (voTimer >= 5) 
            {AudioController.Play ("CHAR_Johnny_ObjectThrow");}
    }

    override protected void guitarUseVO()
    {
        AudioController.Play ("CHAR_Johnny_GuitarUse"); 
    }

    override protected void cheerVO()
    {
        AudioController.Play ("CHAR_Johnny_Cheer");
            Debug.Log ("cheering"); 
    }

    override protected void dazedVO()
    {
        AudioController.Play ("CHAR_Johnny_BigOw"); 
    }

//    override protected void slideSFX()
//    {
//        base.slideSFX (); 
//        AudioController.Play ("SFX_Slide"); 
//    }
}
