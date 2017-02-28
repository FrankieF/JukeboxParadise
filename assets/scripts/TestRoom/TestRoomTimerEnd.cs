using UnityEngine;
using System.Collections;

public class TestRoomTimerEnd : MonoBehaviour {

	TestRoomTimerStart timer;

	GameObject trigger;

	public float timePassed;

	void Awake()
	{
		trigger = GameObject.Find ("Start");

		timer = trigger.GetComponent<TestRoomTimerStart>();
	}
	
	void Update ()
	{
		if(timer.start == true)
			timePassed += Time.deltaTime;

		//Debug.Log ("End: " +timer.start);
	}

	void OnGUI()
	{
		if(timer.start == true)
			GUILayout.Label("TIME: " + timePassed);
		else 
		{
			GUI.Label(new Rect(600f, 30f, 70f, 50f),"TIME: " + timePassed);
			
		}
	}

	void OnTriggerEnter()
	{
		timer.start = false;
		//Debug.Log ("TriggerEnter: " + timer.start);
	}
}
