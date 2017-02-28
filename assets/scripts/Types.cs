using UnityEngine;
using System.Collections;

public class Types : MonoBehaviour
{
    static Types tInstance = null; // Creates the instance to be used outside of the class

    public static Types GetInstance // Creates a new instance if there is not one or returns the one already created
    {
        get
        {
            tInstance = tInstance != null ? tInstance : tInstance = GameObject.Find("Main Camera").GetComponent<Types>(); 
            return tInstance;
        }
    }

    public void Create(GameObject[] creations, int instances, GameObject location, GameObject parent) // Goes through array and creates an object instances times
    {
        Vector3 spread = Vector3.zero;         
        GameObject creation = null; // Object to instantiate

        foreach(GameObject c in creations)
        {
            for(int i = 0; i < instances; i++)
            {
                spread = new Vector3(Random.Range(-10.0f, 10.0f), 2.0f, Random.Range(-10.0f, 10.0f)); // Puts objects in random spots on plane to stop objects from spawning on same spot
                creation = Instantiate(c, (location.transform.position + spread),
                                       c.transform.rotation) as GameObject;
                creation.transform.parent = parent.transform; // Assigns object parent for checks in other classes
            }
        }
    }

    public void TestingCreate(GameObject[] creations, int instances, string name, GameObject location, GameObject parent)
    {
        Vector3 spread = Vector3.zero;         
        GameObject creation = null; // Object to instantiate
        
        foreach(GameObject c in creations)
        {
            if(c.name.Contains(name))
            {
                for(int i = 0; i < instances; i++)
                {
                    spread = new Vector3(Random.Range(-10.0f, 10.0f), 2.0f, Random.Range(-10.0f, 10.0f)); // Puts objects in random spots on plane to stop objects from spawning on same spot
                    creation = Instantiate(c, (location.transform.position + spread),
                                           c.transform.rotation) as GameObject;
                    creation.transform.parent = parent.transform; // Assigns object parent for checks in other classes
                }
            }
        }
    }

    void OnApplicationQuit() // Resets instance back to null
    {
        tInstance = null;
    }
}
