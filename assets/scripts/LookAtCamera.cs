using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour 
{
    public GameObject lookat;

    void Start()
    {
        StartCoroutine(Look());
    }

	void Update () 
    {
        transform.LookAt(lookat.transform.position);
	}

    public void StartCoro()
    {
        StartCoroutine(AutoTurnOff());
    }

    IEnumerator Look()
    {
        while (lookat == null)
        {
            if(GameObject.Find("LookAt") != null)
                lookat = GameObject.Find("LookAt");
            yield return null;
        }
        yield break;
    }

    IEnumerator AutoTurnOff()
    {
        float time = 0f, timeUp = 5f;;

        while (true)
        {
            time += Time.deltaTime;

            if(time >= timeUp)
            {
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }
}
