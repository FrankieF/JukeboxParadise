#define DEBUG

using UnityEngine;
using System;

public class DebugUtils
{
	public static void Assert(bool condition, string message = "")
	{
		if (!condition) 
		{
            Debug.LogError(message);
		}
	}

    public static void Log(string message)
    {
        Debug.Log(message);
    }
}