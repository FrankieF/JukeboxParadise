using UnityEngine;
using System.Collections;

public class PlayerCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if(col.name.Contains("Player"))
        {
            col.rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.name.Contains("Player"))
        {Debug.LogError("Player left");
            col.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
