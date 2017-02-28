using UnityEngine;
using System.Collections;

public class CounterTop : MonoBehaviour
{
    void OnCollisionStay(Collision collision)
    {
        string ingredients = "Ingredients";
        if (collision.gameObject.tag == ingredients)
        {
            Vector3 direction = transform.position - collision.transform.position;
            direction.Normalize();
            collision.rigidbody.AddForce(direction * 150, ForceMode.Force);
        }
    }
}
