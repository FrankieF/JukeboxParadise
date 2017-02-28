//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic; 
//
//public class PlayAudio : MonoBehaviour
//{
//     Players players; 
//    static public bool punching; 
//    
//    // Use this for initialization
//    void Start ()
//    {
//        players = GameObject.Find ("Player2").GetComponent<Players> (); 
//        
//    }
//
//
//
//
//    // Update is called once per frame
//    void Update ()
//    {
//        attackSFX (); 
//        
//    }
//
//    public bool attackSFX ()  
//    {
//       // attack = players.CheckAttack (); 
//        punching = players.CheckAttack ();
//        if (punching == true) 
//        {
//            AudioController.Play("CHAR_Sophia_Punch"); 
//        }
//
// 
//        
//
//        
//    }
//}
