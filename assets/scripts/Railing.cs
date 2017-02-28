using UnityEngine;
using System.Collections;

public class Railing : MonoBehaviour 
{
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Ingredients"))
        {
            Vector3 push = transform.position - col.transform.position;
            push.Normalize();
            col.rigidbody.AddForce(-push * 100, ForceMode.Force);
        }
    }
}
