using UnityEngine;
using System.Collections;

/// <summary>
/// If an object is in the trigger the spawn point is removed from the list in Get Spawn Points
/// This stops things from spawning ontop of each other
/// </summary>
public class SpawnPoints : MonoBehaviour // If an object is in the trigger the spawn point is removed from the list in Get Spawn Points
{
    void OnTriggerEnter(Collider col) // This stops things from spawning ontop of each other
    {
        GetSpawnPoints.GetInstance.RespawnPoints.Remove(gameObject);
    }

    void OnTriggerExit(Collider col)
    {
        GetSpawnPoints.GetInstance.RespawnPoints.Add(gameObject);
    }
}
