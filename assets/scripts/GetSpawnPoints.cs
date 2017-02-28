using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetSpawnPoints : MonoBehaviour // This is used to get the spawn points every class references these spawn points
{
    static GetSpawnPoints spInstance = null;

    public List<GameObject> RespawnPoints = new List<GameObject>(); // This list is used for everything to repsawn
    public List<GameObject> TeamSpawnBlue = new List<GameObject>();
    public List<GameObject> TeamSpawnPink = new List<GameObject>();
    

    public static GetSpawnPoints GetInstance // Checks if there is already an instance or creates one from the main camera
    {
        get
        {
            spInstance = spInstance != null ? spInstance : spInstance = GameObject.Find("Main Camera").GetComponent<GetSpawnPoints>();
            return spInstance;
        }
    }

    void Awake() // Creates the list and clears the list if there is anything in it
    {
        RespawnPoints.Clear();
        AddSpawnPoints(RespawnPoints);
    }

    List<GameObject> AddSpawnPoints(List<GameObject> list) // Adds spawn points to the list
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("RespawnPoints");

        foreach(GameObject go in points)
        {
            list.Add(go); 
        }
        return list;
    }

    void OnApplicationQuit() // Sets the instance back to null when the program is finished running
    {
        spInstance = null;
    }
}
