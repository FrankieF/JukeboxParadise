using UnityEngine;
using System.Collections;

public class ParticleAnimationEvents : MonoBehaviour 
{
    void TurnOnChildren()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<ParticleSystem>().renderer.enabled = true;
        }
    }

    void TurnOffChildren()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<ParticleSystem>().renderer.enabled = false;
        }
    }
}
