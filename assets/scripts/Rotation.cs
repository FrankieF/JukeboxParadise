using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour
{
    public float rotateSpeed = .35f;

    public bool shouldRotate = true;
    public bool rotate;
    public Vector3 point = Vector3.zero;
    Vector3 up = Vector3.up;
    Vector3 forward = -Vector3.forward;
    public Vector3 pos;

    void Start()
    {
        pos = transform.position;

        if (rotate)
        {
            point = pos;
            up = forward;
        }

    }

    void Update()
    {
        if(shouldRotate && Time.timeScale != 0)
        {
            transform.RotateAround(point, up, rotateSpeed);
        }
        
    }
}
