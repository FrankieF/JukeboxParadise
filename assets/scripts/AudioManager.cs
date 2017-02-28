using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	
	//public AudioClip hit; 
    [Header("Characters VO Volume")]
    public bool charVOvolume; 
    public float VOsophieVolume;
    public float VOjohnnyVolume; 
    public float VOgingerVolume;
    public float VOlouisVolume; 

    [Header("Interactable Objects Volume")]
    public bool ioVolume; 
    public float ObjectsVolume; 

    [Header("Background Volume")]
    public bool bgVolume; 
    public float backgroundVolume; 
    public float uiVolume; 

    [Header("Foley Volume")]
    public bool foleyvolume; 
    public float foleyVolume; 
 


	void Start () 
    {   
   

        AudioController.SetCategoryVolume( "IO_SFX", ObjectsVolume );
        AudioController.SetCategoryVolume( "FOLEY_SFX", foleyVolume );
        AudioController.SetCategoryVolume( "VOsophie", VOsophieVolume );
        AudioController.SetCategoryVolume( "VOjohnny", VOjohnnyVolume );
        AudioController.SetCategoryVolume( "VOginger", VOgingerVolume );
        AudioController.SetCategoryVolume( "VOlouis", VOlouisVolume );
        AudioController.SetCategoryVolume ("BG_Music", backgroundVolume); 
        AudioController.SetCategoryVolume("UI_Crowd_Whistle", uiVolume); 

        AudioController.Play ("bg_Ambience"); 
     
	}


}