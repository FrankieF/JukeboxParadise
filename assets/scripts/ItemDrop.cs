using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDrop<T>: MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    private static object _lock = new Object();

    public  static T itemDropInstance
    {
        get
        {
            if(applicationIsQuiting)
            {
                Debug.Log("Unity is shuting down and will not create a singleton");

                return null;
            }
            lock(_lock)
            {
                if(_instance == null)
                {
                    _instance = (T) FindObjectOfType(typeof(T));

                    if(FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("Found more than 1 Singleton");

                        return _instance;
                    }

                    if(_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();

                        singleton.name = "singleton name is: " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("Singelton created with " + typeof(T) + " and " + singleton);
                    }

                    else 
                    {
                        Debug.Log("Singleton already created by " + _instance.gameObject.name);
                    }
                }


            }
            return _instance;
//            return (_instance?_instance:(_instance = new ItemDrop<T>()));
        }
    }

    private static bool applicationIsQuiting = false;

    void OnDestroy()
    {
        applicationIsQuiting = true;
    }
    
    private List<T> itemList = new List<T>();

    private ItemDrop()
    {

    }

    public T addItemQueue(T t)
    {
        itemList.Add(t);

        return t;
    }

    public List<T> getItemsQueued()
    {
        return itemList;
    }
}