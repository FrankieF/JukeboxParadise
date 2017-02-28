using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HUD : MonoBehaviour
{
    public static HUD hudInstance = null;

    public int blueScore, pinkScore;

    [Header("Objects and Textures")]
    public bool headerObjNText;
    public bool teamOne;
    public float numberTimer = 99f, pinkTime, blueTime, boardTime = 0f;
    public GameObject cherry, objectToFill; // The coroutine uses this to change the shader
    public GameObject trayRight, trayLeft;
    public GameObject[] flips; // Gameobjects to be changed
    public GameObject[] teamOneLights; // The lights to show score
    public GameObject[] teamTwoLights;
    public List<string> currentFlips = new List<string>();
    public List<Texture> flipList = new List<Texture>(); // List of textures without blue or pink
    public List<Texture> blueIngredients = new List<Texture>(); // Textures of ingredients with blue
    public List<Texture> pinkIngredients = new List<Texture>(); // Textures of ingredients with pink
    public List<GameObject> left = new List<GameObject>(); // The left flips
    public List<GameObject> center = new List<GameObject>(); // Right flips
    public List<GameObject> right = new List<GameObject>(); // Center flips
    private List<GameObject> topList = new List<GameObject>(); // Reference to all the top objects
    private List<GameObject> bottomList = new List<GameObject>(); // Reference to all the bottom objects
    public UILabel timer, board, pinkTimer, blueTimer, timerPop;
    public UISprite[] popUpsLeftOne, popUpsLeftTwo, popUpsCenterOne, popUpsCenterTwo, popUpsRightOne, popUpsRightTwo; // List of the popups to pop on screen when the player scores   
    public Animator leftBlender, rightBlender;

    [Header("Flip States")]
    public bool headerStates;
    public enum FlipStates {Idle, Fill, Flip} // States to flip the pictures
    public FlipStates currentState = FlipStates.Idle;
    public FlipStates targetState;

    public void SetState(FlipStates state)
    {
        targetState = state;
    }

    public Animator leftAnimator;
    public Animator centerAnimator; // Used to animate the flipping
    public Animator rightAnimator;
    public ParticleSystem[] rightPart, leftPart; // When the player scores

    public static HUD GetInstance // Returns the hudInstance or creates a new one
    {
        get
        {
            hudInstance = hudInstance != null ? hudInstance : hudInstance = GameObject.Find("Main Camera").GetComponent<HUD>();
            return hudInstance;
        }
    }

    void Start()
    {
        CreateLists(topList, bottomList); // Adds objects to the lists
        StartingIngredients();
    }

    void Update()
    {
        if (targetState != currentState)
        {
            StopCoroutine(currentState.ToString());
            currentState = targetState;
            StartCoroutine(currentState.ToString());
        }
        SetScore();
    }

    IEnumerator Idle() // This is the default state
    {
        IngredientLogic.GetInstance.ChangePriority(true);

        while (true)
        {
            yield return null;
        }
    }

    IEnumerator Fill()
    {
        float fillTime = 1;
        int count = 0;
        GameObject changeFill = objectToFill;
        List<ParticleSystem> particles = new List<ParticleSystem>();

        if (teamOne)
        {
            foreach (var item in rightPart)
            {
                particles.Add(item);
            }
        }
        else
        {
            foreach(var item in leftPart)
            {
                particles.Add(item);
            }
        }

        while (true)
        {
            if(count < particles.Count)
            {
                particles[count].Play();
                count++;
            }

            fillTime -= Time.deltaTime * .25f;
            changeFill.renderer.material.SetFloat("_Fill", fillTime);
            if(changeFill.renderer.material.GetFloat("_Fill") < .5f)
                SetState(FlipStates.Idle);

            yield return null;
        }
    }
    IEnumerator Flip() // Used when flipping the pictures when round is over
    {
        float flipTime = .5f;
        leftAnimator.SetBool("flip", true);
        centerAnimator.SetBool("flip", true);
        rightAnimator.SetBool("flip", true);

        //Plays Flip SFX
        AudioController.Play ("UI_ScoreboardFlip"); 

        
        while (true)
        {
            yield return new WaitForSeconds(flipTime);

            UpdateIngredientList();
            GameModes.GetInstance.UpdateList();
            IngredientLogic.GetInstance.ChangePriority(false);
            leftAnimator.SetBool("flip", false);
            centerAnimator.SetBool("flip", false);
            rightAnimator.SetBool("flip", false);
            SetState(FlipStates.Idle);
        }
    }


    void CreateLists(List<GameObject> list1, List<GameObject> list2)
    {
        foreach (var go in flips) // Creates the lists for top and bottom objetcts
        {
            if(go.name.Contains("Top"))
                list1.Add(go);
            else if(go.name.Contains("Bottom"))
                list2.Add(go);
        }
    }

    public void SetTimer()
    {
        if (numberTimer > 0)
        {
            numberTimer -= Time.deltaTime;
            timer.text = numberTimer.ToString("00");

            if(boardTime <= 0) // When a message appears the timer counts down and then changes it back to show the score
                board.text = blueScore.ToString() + " - " + pinkScore.ToString();

            boardTime -= Time.deltaTime;

            if(numberTimer <= 10)
            {
                timerPop.gameObject.SetActive(true);
                timerPop.text = timer.text;
            }
        }
        else if (numberTimer <= 0)
        {
            timerPop.gameObject.SetActive(false);
            if(trayRight.GetComponent<Tray>().item == null && trayLeft.GetComponent<Tray>().item == null)
            {
                if(blueScore > pinkScore)
                    BlueTeamScores();
                else if(pinkScore > blueScore)
                    PinkTeamScores();
                else
                    GameModes.GetInstance.SetState(GameModes.GameModeStates.GameTied);
            }
        } 
    }

    public void CherryMode()
    {
        int zero = 0;
        string player = "Player", blue = "blue", pink = "pink";

        if (cherry.transform.parent.name.Contains(player))
        {
            if (blueTime > zero && pinkTime > zero)
            {
                if (cherry.transform.parent.GetComponent<PlayerController>().color == blue)
                {
                    blueTime -= Time.deltaTime;
                    ShowTime(blueTime, blueTimer);
                    blueTimer.gameObject.SetActive(true);
                    timerPop.gameObject.SetActive(true);
                    timer.text = blueTime.ToString("00");
                    board.text = "BLUE TEAM";
                    boardTime = 2f;
                    timerPop.text = timer.text;
                } 
                else if (cherry.transform.parent.GetComponent<PlayerController>().color == pink)
                {
                    pinkTime -= Time.deltaTime;
                    ShowTime(pinkTime, pinkTimer);
                    pinkTimer.gameObject.SetActive(true);
                    timerPop.gameObject.SetActive(true);
                    timer.text = pinkTime.ToString("00");
                    board.text = "PINK TEAM";
                    boardTime = 2f;
                    timerPop.text = timer.text;
                }
            } 
            else if(blueTime <= 0 || pinkTime <= 0)
            {
                if (cherry.transform.parent.GetComponent<PlayerController>().color == pink)
                {
                    PinkTeamScores();
                    cherry.SetActive(false);
                    pinkTimer.gameObject.SetActive(false);
                } 
                else if (cherry.transform.parent.GetComponent<PlayerController>().color == blue)
                {
                    IngredientLogic.GetInstance.ReturnCherry();
                    BlueTeamScores();
                    cherry.SetActive(false);
                    blueTimer.gameObject.SetActive(false);
                }
            }
        } else
        {
            blueTimer.gameObject.SetActive(false);
            pinkTimer.gameObject.SetActive(false);
            timer.text = "00";
            timerPop.gameObject.SetActive(false);
        }
    }

    void ShowTime(float timeRemaining, UILabel timer)
    {
        if (timeRemaining >= 10)
            timer.text = timeRemaining.ToString("00");
        else
            timer.text = timeRemaining.ToString("0");
    }

    public void StartingIngredients() // Gets list of ingredient textures for top and bottom
    {
        Texture tempTexture = null;
        
        currentFlips.Clear();
        
        foreach(var flipToChange in topList) // Sets top to a random texture
        {
            tempTexture = blueIngredients[Random.Range(0, blueIngredients.Count)];
            flipToChange.renderer.materials[1].mainTexture = tempTexture;
            currentFlips.Add(tempTexture.name);
        }
        
        foreach (var flipToChange in bottomList)
        {
            string reference = flipToChange.name.Substring(6, 3); // Stores the position of the item in the list
            for(int f = 0; f < topList.Count; f++)
            {
                if(topList[f].name.Contains(reference)) // When the item finds a top at the same position it changes its texture
                { 
                    string topTexture = topList[f].renderer.materials[1].mainTexture.ToString().Substring(0, 5);

                    var bottomTexture = pinkIngredients.First((pi) => (pi.name.Contains(topTexture)));
                    {
                        flipToChange.renderer.materials[0].mainTexture = bottomTexture;
                    }
                }
            }
        }
    }

    public void SwitchStartingIngredients()
    {
        Texture tempTexture = null;

        foreach (var flipToChange in topList)
        {
            string topTexture = flipToChange.renderer.materials[1].mainTexture.ToString().Substring(0, 5);
            tempTexture = flipList.First((fl) => (fl.name.Contains(topTexture)));
            {
                flipToChange.renderer.materials[1].mainTexture = tempTexture;
            }
        }

        foreach (var flipToChnage in bottomList)
        {
            string bottomTexture = flipToChnage.renderer.materials[0].mainTexture.ToString().Substring(0, 5);
            tempTexture = flipList.First((fl) => (fl.name.Contains(bottomTexture)));
            {
                flipToChnage.renderer.materials[0].mainTexture = tempTexture;
            }
        }
    }

    void UpdateIngredientList() // Makes a list of the ingredients on the hud
    {
        currentFlips.Clear();
        foreach(var f in topList)
        {
            currentFlips.Add(f.renderer.materials[1].mainTexture.name);
        }
    }

    public void ChangeBackTextures()
    {
        SetBackAndTopBack(ObjectToSet(left, "BackLeft"), ObjectToSet(left, "TopLeft")); // Changes the left textures
        SetBackAndTopBack(ObjectToSet(center, "BackCenter"), ObjectToSet(center, "TopCenter")); // Changes the center textures
        SetBackAndTopBack(ObjectToSet(right, "BackRight"), ObjectToSet(right, "TopRight")); // Changes the right textures
    }

    public void ChangeFrontTextures()
    {
        SetTopAndBottom(ObjectToSet(left, "BackLeft"), ObjectToSet(left, "TopLeft"), ObjectToSet(left, "BottomLeft")); // Changes the left textures
        SetTopAndBottom(ObjectToSet(center, "BackCenter"), ObjectToSet(center, "TopCenter"), ObjectToSet(center, "BottomCenter")); // Changes the center textures
        SetTopAndBottom(ObjectToSet(right, "BackRight"), ObjectToSet(right, "TopRight"), ObjectToSet(right, "BottomRight")); // Changes the right textures
    }

    /// <summary>
    /// Objects to set.
    /// </summary>
    /// <returns>The to set.</returns>
    /// <param name="list">List to cycle through.</param>
    /// <param name="position">Position (top, bottom, back).</param>
    GameObject ObjectToSet(List<GameObject> list, string position) // Finds an item in the list to be used as reference 
    {
        GameObject objectToChange = null;
        foreach (GameObject go in list)
        {
            if(go.name.Contains(position))
            {
                objectToChange = go;
                return objectToChange;
            }
        }
            return objectToChange;
    }

    /// <summary>
    /// Sets the top and bottom.
    /// </summary>
    /// <param name="backObject">Back object.</param>
    /// <param name="topObjectToSet">Top object to set.</param>
    /// <param name="bottomObjectToSet">Bottom object to set.</param>
    void SetTopAndBottom(GameObject backObject, GameObject topObjectToSet, GameObject bottomObjectToSet) // Changes the front pictures on the HUD
    {
        topObjectToSet.renderer.materials [1].mainTexture = backObject.renderer.material.mainTexture;
        bottomObjectToSet.renderer.material.mainTexture = topObjectToSet.renderer.materials[1].mainTexture;
    }

    /// <summary>
    /// Sets the back and top back.
    /// </summary>
    /// <param name="backObjectToSet">Back object to set.</param>
    /// <param name="topBackObjectToSet">Top back object to set.</param>
    void SetBackAndTopBack(GameObject backObjectToSet, GameObject topBackObjectToSet) // Changes the back pictures on the HUD for the flip
    {
        Texture textureChange = flipList [Random.Range(0, flipList.Count)]; 
        backObjectToSet.renderer.material.mainTexture = textureChange;
        topBackObjectToSet.renderer.materials[0].mainTexture = textureChange;
    }

    public void PlayPopupOne(GameObject location, string name)
    {
        string sLeft = "Left"; // Used to check where the item was scored
        string sRight = "Righ";
        string sCenter = "Cent";
        string popLocation = location.transform.parent.name.Substring(16, 4); // Cuts the words off so only left, center or right is left
        GameObject popupContainer = null; // The popup container will be stored here so the tweens can be accessed

        // popLocation is compared to the strings just created
        // If one of the strings matches popLocation it will go through the list of popups at that location
        // When a popup is found with the name passed into this function it stores the parent of the popups into a game object
        // The popup is set true and the tweens are set to play

        if (popLocation.Contains(sLeft))
        {
            foreach(var pop in popUpsLeftOne)
            {
                if(pop.name.Contains(name))
                {
                    popupContainer = GameObject.Find("Left PopUp Container One");
                    pop.gameObject.SetActive(true);
                    popupContainer.GetComponent<TweenScale>().PlayForward();
                    popupContainer.GetComponent<TweenAlpha>().PlayForward();
                }
            }
        }
        else if (popLocation.Contains(sCenter))
        {
            foreach(var pop in popUpsCenterOne)
            {
                if(pop.name.Contains(name))
                {
                    popupContainer = GameObject.Find("Center PopUp Container One");
                    pop.gameObject.SetActive(true);
                    popupContainer.GetComponent<TweenScale>().PlayForward();
                    popupContainer.GetComponent<TweenAlpha>().PlayForward();
                }
            }
        }
        else if (popLocation.Contains(sRight))
        {
            foreach(var pop in popUpsRightOne)
            {
                if(pop.name.Contains(name))
                {
                    popupContainer = GameObject.Find("Right PopUp Container One");
                    pop.gameObject.SetActive(true);
                    popupContainer.GetComponent<TweenScale>().PlayForward();
                    popupContainer.GetComponent<TweenAlpha>().PlayForward();
                }
            }
        }
    }

    public void PlayPopupTwo(GameObject location, string name)
    {
        string sLeft = "Left"; // Used to check where the item was scored
        string sRight = "Righ";
        string sCenter = "Cent";
        string popLocation = location.transform.parent.name.Substring(16, 4); // Cuts the words off so only left, center or right is left
        GameObject popupContainer = null; // The popup container will be stored here so the tweens can be accessed
        
        // popLocation is compared to the strings just created
        // If one of the strings matches popLocation it will go through the list of popups at that location
        // When a popup is found with the name passed into this function it stores the parent of the popups into a game object
        // The popup is set true and the tweens are set to play
        
        if (popLocation.Contains(sLeft))
        {
            foreach(var pop in popUpsLeftTwo)
            {
                if(pop.name.Contains(name))
                {
                    popupContainer = GameObject.Find("Left PopUp Container Two");
                    pop.gameObject.SetActive(true);
                    popupContainer.GetComponent<TweenScale>().PlayForward();
                    popupContainer.GetComponent<TweenAlpha>().PlayForward();
                }
            }
        }
        else if (popLocation.Contains(sCenter))
        {
            foreach(var pop in popUpsCenterTwo)
            {
                if(pop.name.Contains(name))
                {
                    popupContainer = GameObject.Find("Center PopUp Container Two");
                    pop.gameObject.SetActive(true);
                    popupContainer.GetComponent<TweenScale>().PlayForward();
                    popupContainer.GetComponent<TweenAlpha>().PlayForward();
                }
            }
        }
        else if (popLocation.Contains(sRight))
        {
            foreach(var pop in popUpsRightTwo)
            {
                if(pop.name.Contains(name))
                {
                    popupContainer = GameObject.Find("Right PopUp Container Two");
                    pop.gameObject.SetActive(true);
                    popupContainer.GetComponent<TweenScale>().PlayForward();
                    popupContainer.GetComponent<TweenAlpha>().PlayForward();
                }
            }
        }
    }

    void TurnOff() // Goes through the list of popups and turns all of them off
    {
        popUpsLeftOne.Select((p) => {
            p.gameObject.SetActive(false);
            return p;}).ToArray();

        popUpsCenterOne.Select((p) => {
            p.gameObject.SetActive(false);
            return p;}).ToArray();

        popUpsRightOne.Select((p) => {
            p.gameObject.SetActive(false);
            return p;}).ToArray();

        popUpsLeftTwo.Select((p) => {
            p.gameObject.SetActive(false);
            return p;}).ToArray();
        
        popUpsCenterTwo.Select((p) => {
            p.gameObject.SetActive(false);
            return p;}).ToArray();
        
        popUpsRightTwo.Select((p) => {
            p.gameObject.SetActive(false);
            return p;}).ToArray();
    }

    public void ChangeTexture(bool teamOne, string name) // Changes texture from black to blue or pink depending on the team
    {
        Texture temp; // Creates a texture
        if (teamOne)
        {
            foreach (var flip in bottomList)
            {
                if (flip.renderer.materials[0].mainTexture.name.Contains(name)) // Checks for the flip to change
                {
                    temp = pinkIngredients.First((t) => (t.name.Contains(name))); // Grabs texture that matches the flip but with the teams color
                    {
                        if(flip.renderer.materials[0].mainTexture != temp) // Checks to make sure the flip has not been changed
                        {
                            pinkScore++; // increases score for checking for overtime mode
                            flip.renderer.materials[0].mainTexture = temp; // changes texture
                            CheckIfBottomIngredientsCollected(bottomList); // Checks for round to be over
                            PlayPopupOne(flip, name); // Plays the popup of the ingredient
                            rightBlender.SetTrigger("rotate");
                            board.text = "Score!";
                            boardTime = 2f;
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var flip in topList)
            {
                if (flip.renderer.materials[1].mainTexture.name.Contains(name))
                {
                    temp = blueIngredients.First((t) => (t.name.Contains(name)));
                    {
                        if(flip.renderer.materials[1].mainTexture != temp)
                        {
                            blueScore++;
                            board.text = "Score!";
                            flip.renderer.materials[1].mainTexture = temp;
                            CheckIfTopIngredientsCollected(topList);
                            PlayPopupTwo(flip, name);
                            leftBlender.SetTrigger("rotate");
                            boardTime = 2f;
                            return;
                        }
                    }
                }
            }
        }
    }

    void CheckIfTopIngredientsCollected(List<GameObject> list) // When all 3 ingredients are collected the score is increased and the state changes
    {
        string blue = "Blue";
        bool item = list.All((i) => (i.renderer.materials [1].mainTexture.name.Contains(blue))); // Goes through list to see if all the flips are blue
        {
            if(item)
            {
                BlueTeamScores();
            }
        }
    }

    void CheckIfBottomIngredientsCollected(List<GameObject> list)
    {
        string pink = "Pink";
        bool item = list.All((i) => (i.renderer.materials [0].mainTexture.name.Contains(pink)));
        {
            if(item)
            {
                PinkTeamScores();
            }
        }
    }

    public void PinkTeamScores()
    {
        timerPop.gameObject.SetActive(false);
        objectToFill = teamOneLights[GameModes.GetInstance.teamOneScore]; // Makes the shader on the light move
        GameModes.GetInstance.roundNumber++; // Changes round
        GameModes.GetInstance.teamOneScore++; // Increases team score
        GameModes.GetInstance.round.text = "Round " + GameModes.GetInstance.roundNumber.ToString(); // Changes the round number on the GUI
        teamOne = true;
        IngredientLogic.GetInstance.ReturnIngredients(); // Puts all ingredients back to the plane
        GameModes.GetInstance.RemovePlayerPicks();
        GameModes.GetInstance.blue = false;
        pinkScore = 0; // Resets the round score for ingredient count
        blueScore = 0;
        board.text = "PINK TEAM";
        boardTime = .5f;
        GameModes.GetInstance.SetState(GameModes.GameModeStates.ChangeHUD); // Sets the new state of gamemodes to fade out into the new round
        //Plays the Winning SFX for Rounds 
        AudioController.Play ("UI_RoundWin_Pink");
    }
    
    void BlueTeamScores()
    {
        timerPop.gameObject.SetActive(false);
        objectToFill = teamTwoLights[GameModes.GetInstance.teamTwoScore]; // Makes the shader on the light move
        GameModes.GetInstance.roundNumber++; // Changes round
        GameModes.GetInstance.teamTwoScore++; // Increases team score
        GameModes.GetInstance.round.text = "Round " + GameModes.GetInstance.roundNumber.ToString(); // Changes the round number on the GUI
        teamOne = false;
        IngredientLogic.GetInstance.ReturnIngredients(); // Puts all ingredients back to the plane
        GameModes.GetInstance.RemovePlayerPicks();
        GameModes.GetInstance.blue = true;
        pinkScore = 0; // Resets the round score for ingredient count
        blueScore = 0;
        board.text = "BLUE TEAM";
        boardTime = .5f;
        GameModes.GetInstance.SetState(GameModes.GameModeStates.ChangeHUD); // Sets the new state of gamemodes to fade out into the new round
        //Plays the Winning SFX for Rounds 
        AudioController.Play ("UI_RoundWin_Blue");
    }

    public void FlashIngredients(bool blueTeam)
    {
        if (blueTeam)
        {
            //Plays the Score SFX when texture changes color 
            AudioController.Play ("UI_ScoreWin_Blue"); 
            string blue = "Blue";
            if (topList [0].renderer.materials[1].mainTexture.name.Contains(blue))
            {
                for (int i = 0; i < topList.Count; i++)
                {
                    for (int b = 0; b < flipList.Count; b++)
                    {
                        if (topList [i].renderer.materials [1].mainTexture.name.Contains(flipList [b].name))
                        {
                            topList [i].renderer.materials [1].mainTexture = flipList [b];
                        }
                    }
                }
            } 
            else
            {
                for (int i = 0; i < topList.Count; i++)
                {
                    for (int b = 0; b < blueIngredients.Count; b++)
                    {
                        if (blueIngredients[b].name.Contains(topList [i].renderer.materials [1].mainTexture.name))
                        {
                            topList [i].renderer.materials [1].mainTexture = blueIngredients [b];
                        }
                    }
                }
            }
        }
        else
        {
            //Plays the Score SFX when texture changes color 
            AudioController.Play ("UI_ScoreWin_Pink");
            string pink = "Pink";
            if(bottomList[0].renderer.material.mainTexture.name.Contains(pink))
            {
                for(int i = 0; i < bottomList.Count; i++)
                {
                    for(int p = 0; p < flipList.Count; p++)
                    {
                        if(bottomList[i].renderer.material.mainTexture.name.Contains(flipList[p].name))
                        {
                            bottomList[i].renderer.material.mainTexture = flipList[p];
                        }
                    }
                }
            }
            else
            {
                for(int i = 0; i < bottomList.Count; i++)
                {
                    for(int p = 0; p < pinkIngredients.Count; p++)
                    {
                        if(pinkIngredients[p].name.Contains(bottomList[i].renderer.material.mainTexture.name))
                        {
                            bottomList[i].renderer.material.mainTexture = pinkIngredients[p];
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Cheats for changing score and testing HUD
    /// </summary>
    void SetScore()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            PinkTeamScores();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            BlueTeamScores();
        }
    }

    void OnApplicationQuit()
    {
        hudInstance = null;
    }
}
