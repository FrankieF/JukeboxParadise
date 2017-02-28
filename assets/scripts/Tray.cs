using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tray : MonoBehaviour
{
    public bool hasObject;
    public bool hit, cannotReceiveObject;
    public int hits = 0;
    public int increaseTrayPosition = 0;
    public int ingredientCount = 0;
    public float timeRemaining = 15f;
    private float slerpFinished = 1f;
    public string trayColor, hitReference;
    public Vector3 direction;
    public List<string> ingredients = new List<string>(); // Keeps track of the ingredients on the HUD

    public ParticleSystem takeIngredient, target, smokeParticle = new ParticleSystem(),
                          crackParticle = new ParticleSystem();
    public UISprite aButton;

    public GameObject item; // The object put on the tray
    public GameObject score;
    public GameObject lidObject, trayObject, lidTexture; // Used for turning off the lid and setting the position of item
    public GameObject startingPosition;
    public GameObject[] locations; // Where to move to
    public Vector3 rotateDirection; // Changes for each tray to make it rotate
    public Texture glass, glassBroken;

    public enum TrayStates{ None, Idle, Move, Rotate, Wait, Return, Broken, Max}
    public TrayStates previousState;
    public TrayStates currentState = TrayStates.None;
    public TrayStates targetState = TrayStates.Idle;

    public void SetState(TrayStates state)
    {
        targetState = state;
    }

    public Animator tray, lid; // The things that animate
    public string player = "Player";
    public PlayerController pc;

    // Sets start position to the first gameobject in locations
    void Start()
    {
        startingPosition = locations [0];
    }

    void Update()
    {
        if(currentState != targetState)
        {
            StopCoroutine(currentState.ToString());
            previousState = currentState;
            currentState = targetState;
            StartCoroutine(currentState.ToString());
        }
    }

    IEnumerator None()
    {
        while (true)
        {
            SetState(TrayStates.Idle);
        }
    }

    IEnumerator Idle()
    {
        float moveTime = 5f;
        float timeCount = 0f;
        float t = 0f;

        tray.SetBool("wrongIngredient", false);
        lid.SetBool("wrongIngredient", false);
        tray.SetBool("lostIngredient", false); // Stops the tray from reverting back to idle while moving
        lid.SetBool("lostIngredient", false);
        hasObject = false; // Allows the tray to get new ingredients
        lidObject.SetActive(false); // Turns the lid off in case of a glitch where the lid stays on

        while (true)
        {
            if(transform.position != locations[increaseTrayPosition].transform.position)
            {
                timeCount += Time.deltaTime;
                t = timeCount / moveTime;
                transform.position = Vector3.Slerp(transform.position, locations[increaseTrayPosition].transform.position, t);
            }
            yield return null;
        }
    }

    IEnumerator Move()
    {
        float t = 1;
        float totalTime = 5f;
        float timer = 0f;
        float turnSpeed = 15f;
        int hitCounter = 0;
        hits = 0;

        lidObject.SetActive(true);
        lid.SetBool("deposit", false);
        tray.SetBool("deposit", false);
        tray.SetBool("hasIngredient", true);
        lid.SetBool("hasIngredient", true);
        lid.SetBool("hit", false);

        //Plays Move Tray SFX
        AudioController.Play ("SFX_moveTray");
        while (true)
        {
            timer += Time.deltaTime;
            t = timer / totalTime;

            transform.position = Vector3.Slerp(startingPosition.transform.position,
                                                locations[4].transform.position, t);
            transform.Rotate(rotateDirection, Time.deltaTime * turnSpeed);

            if(t >= slerpFinished)
            {
                //Stops Move Tray SFX when tray stops moving
                AudioController.Stop ("SFX_moveTray"); 
                SetState(TrayStates.Wait);
            }
            if(hitCounter != hits) // Checks to see if the number of hits increased and calls the function
                LoseItem();
            hitCounter = hits; // Sets the number of hits

            yield return null;
        }
    }

    IEnumerator Wait()
    {
        tray.SetBool("deposit", true);
        if(lid.gameObject.activeSelf)
            lid.SetBool("deposit", true);

        while (true)
        {
            if(item != null)
            {
                takeIngredient.Play();
                item.transform.position = score.transform.position; // Puts the ingredient into the goal and resets its variables
                item.rigidbody.useGravity = true;
                item.rigidbody.isKinematic = false;
                item.transform.parent = GameObject.Find("Ingredients").transform;
                ChangeSizePositive(item);
                PlayerScoreCount(item.name); // Changes the player socres counter

                PlayerController pController = GameObject.Find(item.GetComponent<IngredientGeneric>().parentReference).GetComponent<PlayerController>();
                pController.scored++; // Increases the stat for scoring for the player

                item = null;// Stops item from referencing the ingredient
                ingredientCount++;
            }
            yield return new WaitForSeconds(.50f);
            lid.SetBool("deposit", false);
            lidObject.SetActive(false);
            yield return new WaitForSeconds(.4f);
            tray.SetBool("deposit", false);
            SetState(TrayStates.Return);
        }
    }

    IEnumerator Return()
    {
        float t = 0f, totalTime = 2f, timer = 0f, length = .4f;

        tray.SetBool("hasIngredient", false);
        tray.SetBool("wrongIngredient", false);
        lid.SetBool("hasIngredient", false);
        lid.SetBool("wrongIngredient", false);
        
        //Plays SFX move tray 
        AudioController.Play ("SFX_revertTray"); 
        while (t < length)
        {
            timer += Time.deltaTime;
            t = timer / totalTime;

            transform.position = Vector3.Slerp(transform.position,
                                               startingPosition.transform.position, t);
            yield return null;
        }
        //Stops playing the tray's movement sound effect 
        AudioController.Stop ("SFX_revertTray"); 
        SetState(TrayStates.Idle);
    }

    IEnumerator Broken()
    {
        float brokenTime = 4f;
        smokeParticle.Play();

        //Plays Broken Tray SFX
        AudioController.Play ("ENV_TrayBroken"); 

        while(true)
        {
            yield return new WaitForSeconds(brokenTime);
            cannotReceiveObject = false;
            smokeParticle.Stop();
            tray.SetBool("deposit", true);
            lid.SetBool("deposit", true);
            tray.SetBool("wrongIngredient", false);
            SetState(TrayStates.Idle);
        }
    }

    public void LoseItem()
    {
        if (hits > 0 && hits < 3)
        {
            //If the glass lid is hit twice, the glass breaking SFX plays 
            lidTexture.renderer.material.SetTexture("_Crack", glassBroken);
            crackParticle.Play();

        }
        else if (hits == 3)
        {
            //If the glass lid is hit twice, the glass breaking SFX plays 
            crackParticle.Play();
            lidTexture.renderer.material.SetTexture("_Crack", glass);
            lidObject.SetActive(false);
        }
        else if (hits >= 4)
        {
            float force = 700f;
            item.transform.parent = GameObject.Find("Ingredients").transform;
            item.rigidbody.useGravity = true;
            item.rigidbody.isKinematic = false;
            item.transform.position += (Vector3.up * 3);
            item.rigidbody.AddForce(direction * force, ForceMode.Force);
            ChangeSizePositive(item);
            item = null;
            hasObject = false;
            tray.SetBool("lostIngredient", true);
            lid.SetBool("lostIngredient", true);
            GameObject.Find(hitReference).GetComponent<PlayerController>().knockedOff++;
            SetState(TrayStates.Return);
        }
    }

    public void Reset()
    {
        ingredients.Clear();
        GetIngredients();
        if (lidObject.activeSelf)
        {
            lid.SetBool("lostIngredient", true);
            lid.SetBool("deposit", true);
            lidObject.SetActive(false);
        }
        tray.SetBool("lostIngredient", true);
        tray.SetBool("deposit", true);
        SetState(TrayStates.Idle);
    }

    void PlayerScoreCount(string name)
    {
        switch (name)
        {
            case "Player 1":
            {
                GameObject.Find("Player 1").GetComponent<PlayerController>().scored++;
                return;
            }
            case "Player 2":
            {
                GameObject.Find("Player 2").GetComponent<PlayerController>().scored++;
                return;
            }
            case "Player 3":
            {
                GameObject.Find("Player 3").GetComponent<PlayerController>().scored++;
                return;
            }
            case "Player 4":
            {GameObject.Find("Player 4").GetComponent<PlayerController>().scored++;
                return;}
        }
    }

    void GetIngredients()
    {
        foreach (var name in HUD.GetInstance.currentFlips){ingredients.Add(name);} // Adds the ingredients to the list
    }

    public bool CheckIngredient(string name)
    {
        foreach (var i in ingredients){if(i.Contains(name))return true;} // Checks if the ingredient is on the HUD or not
        return false;
    }

    void OnTriggerEnter(Collider collider)
    {
        string ingredient = "Ingredients";
       
        if (ShowAButton(collider))
        {
            aButton.gameObject.SetActive(true);
        }

        if(!hasObject && collider.CompareTag(ingredient) && collider.GetComponent<IngredientGeneric>().parentReference != "")
        {
            if(CheckIngredient(collider.name.Substring(0, 5)) && !cannotReceiveObject)
            {
                aButton.gameObject.SetActive(false);
                item = collider.gameObject;
                collider.GetComponent<IngredientGeneric>().aButton.gameObject.SetActive(false);
                ChangeSizeNegative(item);
                collider.transform.parent = transform;
                collider.GetComponent<Objects>().obtainable = false;
                collider.rigidbody.useGravity = false;
                collider.rigidbody.isKinematic = true;
                collider.transform.position = trayObject.transform.position;
//                ChangeSizeNegative(item);
                hasObject = true; 
                SetState(TrayStates.Move);

                //Ingredient hitting tray
                AudioController.Play ("SFX_trayPlaced"); 
            }
            else
            {   
                tray.SetBool("wrongIngredient", true); // Plays animation
                collider.GetComponent<IngredientGeneric>().ReturnToLocation(); // Moves the item back to the collection of items
                cannotReceiveObject = true;
                SetState(TrayStates.Broken);
            }
        }
    }

    void ChangeSizeNegative(GameObject item)
    {
        item.transform.localScale = Vector3.one;
    }

    void ChangeSizePositive(GameObject item)
    {
        string nut = "nut";
        Vector3 newScale = Vector3.zero;

        newScale = item.name.Contains(nut) ? new Vector3(1.75f, 1.75f, 1.75f) : new Vector3(1.5f, 1.5f, 1.5f);
        item.transform.localScale = newScale;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.name.Contains(player))
            aButton.gameObject.SetActive(false);
    }

    bool ShowAButton(Collider collider)
    {
        return collider.name.Contains(player) && collider.GetComponent<PlayerController>().hasObject &&
            collider.GetComponent<PlayerController>().color == trayColor ? true : false;
    }
}
