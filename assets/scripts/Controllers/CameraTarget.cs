using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraTarget : MonoBehaviour
{
    public static List<Transform> transforms = new List<Transform>();

    void OnEnable()
    {
        if (!transforms.Contains(transform))
            transforms.Add(transform);
    }

    void OnDisable()
    {
        if (transforms.Contains(transform))
            transforms.Remove(transform);
    }
}
