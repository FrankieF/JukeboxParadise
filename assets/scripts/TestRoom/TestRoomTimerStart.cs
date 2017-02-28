using UnityEngine;
using System.Collections;

public class TestRoomTimerStart: MonoBehaviour {

    public bool start = false;

	void OnTriggerExit()
    {
        start = true;
    }
}
