using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public Transform cameraPivot;
    public Transform zoomedIn;
    public Transform zoomedOut;
    public float moveDamping;
    public float zoomDamping;
    public float minDistance;
    public float maxDistance;

    void Start()
    {
        StartCoroutine(FollowPlayers());
    }

    IEnumerator FollowPlayers()
    {
        while (true)
        {
            var bounds = GetBounds();
            var targetPosition = Vector3.ClampMagnitude(bounds.center, maxDistance / 2);
            targetPosition.z = 0;
            targetPosition.y = 0;
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, targetPosition, moveDamping * Time.deltaTime);
            //Camera.main.transform.rotation = Quaternion.AngleAxis(0.0f, new Vector3(0.0f, 0.0f, 1.0f));

//            var percent = Mathf.InverseLerp(minDistance, maxDistance, bounds.size.x);
//            targetPosition = Vector3.Lerp(zoomedIn.localPosition, zoomedOut.localPosition, percent);
//            transform.localPosition = Vector3.Lerp(zoomedIn.localPosition, zoomedOut.localPosition, zoomDamping * Time.deltaTime);

            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        var bounds = GetBounds();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    Bounds GetBounds()
    {
        var bounds = new Bounds(CameraTarget.transforms[0].position, new Vector3(0, 2, 0));
        for (int i = 1; i < CameraTarget.transforms.Count; i++)
            bounds.Encapsulate(CameraTarget.transforms[i].position);
        return bounds;
    }
}
