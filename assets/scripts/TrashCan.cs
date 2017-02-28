using UnityEngine;
using System.Collections;

public class TrashCan : MonoBehaviour
{
    public GameObject lid;
    public GameObject openPos;
    Vector3 closePos;
    public ParticleSystem target;

    public float itemCount;

    void Start()
    {
        closePos = transform.position;
    }

    void OnTriggerStay(Collider collider)
    {
        if(collider.CompareTag("Player") && collider.GetComponent<PlayerController>().hasObject)
        {
            OpenLid();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("Player") && collider.GetComponent<PlayerController>().hasObject == false)
        {
            CloseLid();
        }
    }

    void OpenLid()
    {
        float open = .05f;

        lid.transform.position = Vector3.Lerp(transform.position, openPos.transform.position, open);
    }

    void CloseLid()
    {
        float close = .05f;

        lid.transform.position = Vector3.Lerp(transform.position, closePos, close);
    }
}
