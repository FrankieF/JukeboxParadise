using UnityEngine;
using System.Collections;

public class TestRoom : MonoBehaviour 
{

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Application.LoadLevel("Test Room");
		}

	}
}