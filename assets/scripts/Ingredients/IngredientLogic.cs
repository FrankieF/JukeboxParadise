using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class IngredientLogic : MonoBehaviour
{
    public static IngredientLogic igInstance = null;

    public float maxItems = 3;
    public int currentItems = 0; // The number of ingredients on the record 
    public GameObject cherry; // The cherry for overtime mode
    public List<GameObject> items = new List<GameObject>(); // Lists are for the ingredients and spawn points
    public List<GameObject> usedItems = new List<GameObject>();

    private float time = 3f;
    private float usedTime = 3f;
    private string ingredientName, iTag = "Ingredients";



    public static IngredientLogic GetInstance // Returns the igInstance of creates a new one
    {
        get
        {
            igInstance = igInstance != null ? igInstance : igInstance = GameObject.Find("Ingredients").GetComponent<IngredientLogic>();
            return igInstance;
        }
    }

    void Awake()
    {
        CreateIngredients();
        items.Clear();  // Clears list to prevent bugs when game is replayed
        PopulateList(items); // Fils the two lists
    }

    void CreateIngredients() // Creates all the variables to create the ingredients, then creates the ingredients
    {
        GameObject location = GameObject.Find("PlaneIngredients");
        GameObject ingParent = GameObject.Find("Ingredients");
        GameObject[] ingredientsToCreate = GameObject.FindGameObjectsWithTag(iTag);
        Types.GetInstance.Create(ingredientsToCreate, 3, location, ingParent); 

    }

    public List<GameObject> PopulateList(List<GameObject> list) // Gets list of ingredients by using the ingredients tags
    {
        GameObject[] gitems = GameObject.FindGameObjectsWithTag(iTag);
        
        for (int i = 0; i < gitems.Length; i++)
        {
            list.Add(gitems [i]);
        }
        list.Select((l) => // Goes through the list and sets SetActive to false
                    {l.SetActive(false);return l;}
                    ).ToList();

        return list;
    }

    void Update()
    {
        ResetIngredientList();
        time -= Time.deltaTime;
    }

    public void PoolItems()
    {
        if (time <= 0 && currentItems < maxItems)
        {
            ingredientName = DropItem(); // Picks an ingredient
            GameObject item = newObject(ingredientName); // Creates ingredient
            if(item != null)
            {
                item.SetActive(true); // Sets ingreident active
                item.transform.position = GetSpawnPoints.GetInstance.RespawnPoints [Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position; // Spawns ingredient
                item.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f); // Resets rotation
                item.GetComponent<Objects>().spawnParticle.Play();
                items.Remove(item); // Removes ingredient from list
                currentItems++;
                time = 3f;
            }
        }
        
        else if (currentItems >= maxItems)
        {
            time = 5f; // Sets time to wait before creating a new ingredient, this is so ingredients do not spawn automatically when the current is less than max
        }
    }

    string DropItem()
    {
        float total = 0;
        float _priority = 0;
        string _chance = null;

        for (int i = 0; i < items.Count; i ++) // Adds up ingredients priorities
        {
            total += items [i].GetComponent<IngredientGeneric>().priority;
        }

        total = Random.Range(0, total); // Sets total to random number

        for (int i = 0; i < items.Count; i++)
        {
            _priority += items [i].GetComponent<IngredientGeneric>().priority; // Objects priority is added to _priority to check against total

            if (_priority >= total)
            {
                _chance = items [i].name; 
                break;
            }
        }

        return _chance;
    }

    GameObject newObject(string message)
    {
        GameObject temp = null;

        if (message != null && message.Length >= 1)
        {
            // Goes through the list and looks for an item with the selected chance
            // If the chance is equal to the chance the item is added to the list 
            // An item is randomly selected from the list
            var item = items.First((i) => (i.name == message));
            {
                temp = item;
            }
        }
        return temp;
    }

    void ResetIngredientList() // Holds ingredient before putting it back into the list
    {
        if(usedItems.Count > 0)
        {
            if(usedTime > 0)
            {
                usedTime -= Time.deltaTime;
            }
            else
            {
                items.Add(usedItems[0]);
                usedItems.RemoveAt(0);
                usedTime = 3f;
            }
        }
    }

    public void ChangePriority(bool increase) // Changes the chance of an ingredient spawning
    {
        int up = 10, down = 3;
        if (increase)
        {
            for (int i = 0; i < items.Count; i++)
            {
                for (int f = 0; f < HUD.GetInstance.currentFlips.Count; f++)
                {
                    if (items [i].name.Contains(HUD.GetInstance.currentFlips [f].Substring(0, 5)))
                        items [i].GetComponent<IngredientGeneric>().ChangePriority(up);
                }
            }
        } 
        else
        {
            for (int i = 0; i < items.Count; i++)
            {                 
                if (items [i].GetComponent<IngredientGeneric>().priority > down)
                    items [i].GetComponent<IngredientGeneric>().ChangePriority(down);
            }
        }
    }

    public void DropCherry()
    {
        ReturnIngredients();
        cherry.SetActive(true);
        cherry.transform.position = GameObject.Find("Center").transform.position + Vector3.up;
        cherry.rigidbody.useGravity = true;
    }

    public void RespawnCherry() // When the Cherry is knocked off the record in cherry mode it will be respawned
    {
        if (!cherry.activeInHierarchy && HUD.GetInstance.blueTime > 0 && HUD.GetInstance.pinkTime > 0)
        {
            cherry.SetActive(true); // Turns the cherry on
            cherry.transform.position = GetSpawnPoints.GetInstance.RespawnPoints[Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position; // Moves the cherry to the record
            cherry.transform.localRotation = new Quaternion();
        }
    }

    public void ReturnIngredients()
    {
        GameObject ingParent = GameObject.Find("Ingredients");
        GameObject[] activeIngredients = GameObject.FindGameObjectsWithTag(iTag);
        string player = "Player";
        for (int i = 0; i < activeIngredients.Length; i++)
        {
            if(activeIngredients[i].transform.parent.name.Contains(player))
            {
                activeIngredients[i].transform.parent = ingParent.transform;
                activeIngredients[i].rigidbody.useGravity = true;
                activeIngredients[i].rigidbody.isKinematic = false;
                activeIngredients[i].collider.isTrigger = false;
            }
            activeIngredients[i].GetComponent<IngredientGeneric>().ReturnToLocation();
        }
        currentItems = 0;
    }

    public void ReturnCherry()
    {
        if (cherry.transform.parent.name.Contains("Pla"))
        {
            PlayerController pc = cherry.transform.parent.gameObject.GetComponent<PlayerController>(); // Gets the player holding the cherry
            ChangePlayer(pc);
        }

        cherry.rigidbody.useGravity = false;
        cherry.SetActive(false);
        cherry.transform.parent = GameObject.Find("Ingredients").transform;
        cherry.collider.isTrigger = false;
        cherry.rigidbody.isKinematic = false;
        cherry.GetComponent<Objects>().shouldRotate = false;
        cherry.GetComponent<Objects>().obtainable = false;
        cherry.transform.position = Vector3.zero + Vector3.up * 2;
        cherry.transform.localRotation = new Quaternion();
    }

    void ChangePlayer(PlayerController pc)
    {
        pc.hasObject = false;
        pc.animator.SetBool("ingredient", false);
        pc.animator.SetBool("returnToIdle", true);
        pc.pickObject = pc.pickRef;
    }

    void OnApplicationQuit()
    {
        igInstance = null;
    }
}
