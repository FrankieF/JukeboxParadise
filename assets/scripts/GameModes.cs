using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XboxCtrlrInput;

public class GameModes : MonoBehaviour
{
    public static GameModes gmInstance = null; // The reference for the other classes

    [Header("Frame Rate")]
    public bool headerFrame;
    public int fps = 60;

    [Header("Game Modes")] // These are used to check which game mode the players chose
    public bool headerGameModes;
    public string winningColor, player = "Player"; // Used to set the players to animations on win screen

    public int blueTeam, pinkTeam, roundNumber = 1, picCheck = 0; // Keeps track of the teams

    public UIWidget pauseMenu, cherryLine, blueTeamWins, pinkTeamWins, fade; // The pause menu
    public UISprite leftArrow, rightArrow;
    public UILabel ready, round, goo, line, tie; // Played before each round
    public UILabel[] readySetGo;

    [Header("Game Mode States")]
    public bool headerStates;
    public enum GameModeStates {None, TeamBattleMode, ShowText, OverTimeMode, ChangeHUD, FadeRounds, GameTied, FadeToCherry, GameOver, WinScreen, ShowStatsOne, ShowStatsTwo, Max}
    public GameModeStates currentState = GameModeStates.None;
    public GameModeStates previousState = GameModeStates.None;
    public GameModeStates targetState = GameModeStates.None;
    public void SetState(GameModeStates state)
    {
        targetState = state;
    }

    [Header("References")]
    public bool headerRefs;
    public bool blue; // Used to flash ingredients during camera zoom in
    public List<GameObject> startingPoints = new List<GameObject>(); // Where to put the players each round
    public GameObject winObject;
    public GameObject[] trays, winSpawn, loseSpawn, pictures, players;
    public string roundNum, roundAnnouncer = "CHAR_Announcer_Round";
    public Transform mainCamera, cameraPivot, originalCamera, winCamera, record, startPos, endPos;
    public Camera picCamera;

    [Header("Win Conditions")]
    public bool headerWin;
    public int win = 3, teamOneScore = 0, teamTwoScore = 0; // The players score

    [Header("Stats")]
    public bool headerStats;
    public UISprite statMenuOne, statMenuTwo;

    public static GameModes GetInstance // Returns instance or creates new one
    {
        get
        {
            gmInstance = gmInstance != null ? gmInstance : gmInstance = GameObject.Find("Main Camera").GetComponent<GameModes>();
            return gmInstance;
        }
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = fps;
        Screen.showCursor = false;
    }

    void Start()
    {
        StartCoroutine(currentState.ToString());
        players = GameObject.FindGameObjectsWithTag(player);
        GetActivePlayers();
        SetPlayers(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != targetState)
        {
            StopCoroutine(currentState.ToString());
            previousState = currentState;
            currentState = targetState;
            StartCoroutine(currentState.ToString());
        }
        CreateItem();
        ShowPicture();
        ChangePlayers();
    }

    public float noneTime = 4f;
    IEnumerator None()
    {
        //Plays Announcer for Round 1
        AudioController.Play ("CHAR_Announcer_Round1"); 
        backgroundMusic();

        while(noneTime >= 0f)
        {
            noneTime -= Time.deltaTime;
            yield return null;
        }
        GetPlayersTray();
        StartCoroutine(RecordMovement());
        SetState(GameModeStates.TeamBattleMode);
    }

    IEnumerator ShowText()
    {
        float time = 3f;

        foreach (UILabel widget in readySetGo)
        {
            widget.GetComponent<TweenPosition>().ResetToBeginning();
        }
        
        line.GetComponent<TweenPosition>().PlayForward();
        round.GetComponent<TweenPosition>().PlayForward(); // Plays the text before the round starts
        ready.GetComponent<TweenPosition>().PlayForward();
        goo.GetComponent<TweenPosition>().PlayForward();
        
        //Plays 'Round x' 'Ready' 'Go' for Round 2, 3, 4 and 5 
        AudioController.Play (roundNum, 1f, 0f,0f);
        backgroundMusic();

        SwitchTrays(true); // Trays come back

        while (true)
        {
            yield return new WaitForSeconds(time);
            SetState(GameModeStates.TeamBattleMode);
        }
    }

    IEnumerator TeamBattleMode()
    {
        if (previousState == GameModeStates.None)
        {
            HUD.GetInstance.SwitchStartingIngredients();
            UpdateList();
            IngredientLogic.GetInstance.ChangePriority(true);
        }

        HUD.GetInstance.SetState(HUD.FlipStates.Flip);
        HUD.GetInstance.numberTimer = 99f; // Resets time
        
        SetPlayers(false); // Allows players to move
        AudioController.Stop (roundNum, 10f); 
        while(true)
        {
            HUD.GetInstance.SetTimer(); // Time starts counting down
            IngredientLogic.GetInstance.PoolItems(); // Ingredients begin to appear

            yield return null;
        }
    }

    IEnumerator OverTimeMode()
    {
        bool startPressed = false;

        cherryLine.gameObject.SetActive(true); // Cherry text on screen
        cherryLine.GetComponent<TweenPosition>().PlayForward();
        cherryLine.GetComponent<TweenAlpha>().PlayForward();
        HUD.GetInstance.numberTimer = 00f; // How long the time should be for overtime
        HUD.GetInstance.pinkTime = 15f;
        HUD.GetInstance.blueTime = 15f;
        IngredientLogic.GetInstance.DropCherry();
        SwitchTrays(false);
        SetPlayers(true); // Freezes the players and puts them back in their original positions

        //Plays the background music
        overtimeMusic (); 

        while(true)
        {
            if(!startPressed)
            {
                if(GetAButton())
                {
                    startPressed = true;
                    cherryLine.GetComponent<TweenPosition>().PlayReverse();
                    SetPlayers(false);
                }
                yield return null;
            }
            else
            {
                HUD.GetInstance.CherryMode();
                IngredientLogic.GetInstance.RespawnCherry();
                yield return null;
            }
        }
    }

    IEnumerator ChangeHUD()
    {
        bool cameraMoved = false; // Used to lerp the camera
        float time = 0f, duration = 10f, t = 0f, cameraCheck = 1.2f, waitTime = 3f; // The time variables to lerp
        Transform cameraPosition = GameObject.Find("HUDCameraPosition").transform; // Where to move the camera

        while (true)
        {
            if(!cameraMoved)
            {
                time += Time.deltaTime;
                t = time / duration;

                mainCamera.position = Vector3.Lerp(mainCamera.position, cameraPosition.position, t);
                mainCamera.rotation = Quaternion.Lerp(mainCamera.rotation, cameraPosition.rotation, t);

                if(time >= cameraCheck)
                    cameraMoved = true;
            }
            else
            {
                HUD.GetInstance.SetState(HUD.FlipStates.Fill);
                StartCoroutine("Flash");
                yield return new WaitForSeconds(waitTime);
                StopCoroutine("Flash");
                SetState(GameModeStates.FadeRounds);
            }

            yield return null;
        }
    }

    IEnumerator Flash()
    {
        float flashTime = .3f;
        while(true)
        {
            HUD.GetInstance.FlashIngredients(blue);
            yield return new WaitForSeconds(flashTime);
            HUD.GetInstance.FlashIngredients(blue);
            yield return new WaitForSeconds(flashTime);
            HUD.GetInstance.FlashIngredients(blue);
            yield return new WaitForSeconds(flashTime);
            HUD.GetInstance.FlashIngredients(blue);
            yield return new WaitForSeconds(flashTime);
            HUD.GetInstance.FlashIngredients(blue);
            yield return new WaitForSeconds(flashTime);
            HUD.GetInstance.FlashIngredients(blue);
            yield return new WaitForSeconds(flashTime);
            HUD.GetInstance.FlashIngredients(blue);
            yield return new WaitForSeconds(flashTime);
        }
    }

    IEnumerator FadeRounds()
    {
        float second = 1f, movePlayerTime = .2f;
        IngredientLogic.GetInstance.ReturnCherry(); // Puts Cherry back and hides it
        cherryLine.gameObject.SetActive(false); // Turns off the Cherry mode text
        HUD.GetInstance.numberTimer = 00f;
        fade.gameObject.SetActive(true);
        fade.GetComponent<TweenAlpha>().PlayForward(); // Fades the screen to black

        //Adds Number for Character Announcing Rounds 
        roundNum = roundAnnouncer + roundNumber.ToString ();
        //Stops Background Music 
        AudioController.Stop ("MUSIC_Base",  2f); 

        while (true)
        {
            yield return new WaitForSeconds(second);
            SetPlayers(true); // Resets player postion
            yield return new WaitForSeconds(movePlayerTime);
            fade.GetComponent<TweenAlpha>().PlayReverse(); // Fades back in after black screen
            ResetCamera();
            CheckScore();
        }
    }

    IEnumerator  GameTied()
    {
        float time = 3f; 

        IngredientLogic.GetInstance.ReturnIngredients(); // Returns ingredients to the plane
        RemovePlayerPicks(); // Clears ingredients from players list

        tie.GetComponent<TweenPosition>().ResetToBeginning(); // Resets everything on the tie graphic
        tie.gameObject.SetActive(true); // Turns the tie graphic on

        while(true)
        {
            yield return new WaitForSeconds(time);
                tie.GetComponent<TweenPosition>().PlayForward(); // Moves the tie graphic off the screen
                SetState(GameModeStates.FadeToCherry);
        }
    }

    IEnumerator FadeToCherry()
    {
        float second = 1f; // How long to wait for the fade

        tie.gameObject.SetActive(false); // Turns tie off
        fade.gameObject.SetActive(true);
        fade.GetComponent<TweenAlpha>().PlayForward(); // Fades the screen black
       
        //overtimeMusic (); //Plays Overtime background music 
        AudioController.Play ("CHAR_Announcer_Overtime"); 
        while(true)
        {
            yield return new WaitForSeconds(second);
            SetPlayers(true);
            fade.GetComponent<TweenAlpha>().PlayReverse();
            SetState(GameModeStates.OverTimeMode);
        }
    }

    IEnumerator WinScreen()
    {
        winObject.SetActive(true); // Message to display winning team
        Camera.main.transform.position = winCamera.position;
        Camera.main.transform.rotation = winCamera.rotation;
        fade.GetComponent<TweenAlpha>().PlayReverse();
        if(winningColor.Contains("pi"))
            HUD.GetInstance.board.text = "Pink Team Wins!";
        else
            HUD.GetInstance.board.text = "Blue Team Wins!";
        SetPlayers(true);
        ChangePlayersToWinState();
        // Plays ending music
        gameoverMusic(); 

        Camera.main.transform.rotation = winCamera.rotation; // Updates camera rotation

        while (true)
        {
            if(GetStartButton() || GetAButton())
                SetState(GameModeStates.ShowStatsOne);
            yield return null;
        }

    }

    IEnumerator ShowStatsOne()
    {
        StatMenu.GetInstance.UpdateStats();

        statMenuOne.gameObject.SetActive(true);

        while(true)
        {
            if(GetAButton() || GetStartButton())
                SetState(GameModeStates.ShowStatsTwo);
            yield return null;
        }
    }

    IEnumerator ShowStatsTwo()
    {
        string mainMenu = "MainMenu"; // Level to load

        statMenuOne.gameObject.SetActive(false);
        statMenuTwo.gameObject.SetActive(true);

        while(true)
        {
            if(GetAButton() || GetStartButton())
            {
                ClearPlayers();
                Application.LoadLevel(mainMenu);
            }
            else if(GetBButton())
            {
                statMenuTwo.gameObject.SetActive(false);
                SetState(GameModeStates.ShowStatsOne);
            }
            
            yield return null;
        }
    }

    IEnumerator Max()
    {
        while(true)
        {
            yield return null;
        }
    }

    void GetActivePlayers()
    {
//        GameObject[] players = GameObject.FindGameObjectsWithTag(player);
        for (int i = 1; i < players.Length; i++)
        {
            players[i].SetActive(players[i].GetComponent<PlayerController>().inGame ? true : false);
        }
    }

    void ChangePlayers()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players [i].name.Contains(" 1"))
                {
                    players [i].transform.position = GameObject.Find("Player1Location").transform.position;
                    players [i].GetComponent<PlayerController>().SetState(PlayerController.PlayerStates.None);
                } else if (players [i].name.Contains(" 2"))
                {
                    players [i].transform.position = GameObject.Find("Player2Location").transform.position;
                    players [i].GetComponent<PlayerController>().SetState(PlayerController.PlayerStates.None);
                }
            }
        } else if (Input.GetKeyDown(KeyCode.W))
        {
            for(int i = 0; i <players.Length; i++)
            {
                if (players [i].name.Contains(" 1"))
                {
                    players [i].transform.position = (Vector3.zero + Vector3.forward);
                    players [i].GetComponent<PlayerController>().SetState(PlayerController.PlayerStates.Idle);
                } 
                else if (players [i].name.Contains(" 2"))
                {
                    players [i].transform.position = (Vector3.zero + -Vector3.forward);
                    players [i].GetComponent<PlayerController>().SetState(PlayerController.PlayerStates.Idle);
                }
            }
        }

    }

    void SetPlayers(bool freeze)
    {
//        GameObject[] players = GameObject.FindGameObjectsWithTag(player);
        for (int i  = 0; i < players.Length; i++)
        {
            PlayerController pc = players[i].GetComponent<PlayerController>();
            if(freeze)
            {
                pc.SetState(PlayerController.PlayerStates.None); // Stops players from moving before round begins
                SetPosition(players[i], pc.color);
                GameObject num = pc.pNumber;
                num.SetActive(true);
                num.GetComponentInChildren<LookAtCamera>().StartCoro();
                pc.ResetStates();
            }
            else
            {
                pc.SetState(PlayerController.PlayerStates.Idle); // Allows players to move
            }            
        }
    }

    void SetPosition(GameObject player, string color)
    {
        if (color.Contains("blue"))
            player.transform.position = GetSpawnPoints.GetInstance.TeamSpawnBlue[player.GetComponent<PlayerController>().teamNumber].transform.position;
        else
            player.transform.position = GetSpawnPoints.GetInstance.TeamSpawnPink[player.GetComponent<PlayerController>().teamNumber].transform.position;
    }

    void ChangePlayersToWinState()
    {
//        GameObject[] players = GameObject.FindGameObjectsWithTag(player);
        int wSpawn = 0;
        int wLose= 0;

        for (int i = 0; i < players.Length; i++)
        {
            PlayerController pc = players[i].GetComponent<PlayerController>();
            if(pc.color.Contains(winningColor))
            {
                pc.ResetStates();
                pc.SetState(PlayerController.PlayerStates.Dance);
                players[i].transform.position = winSpawn[wSpawn].transform.position;
                Vector3 rotation = transform.position - Vector3.zero;
                players[i].transform.rotation = Quaternion.LookRotation(rotation);
                wSpawn++;

                if(players[i].name.Contains(" 2"))
                {
                    float f = 45f;
                    transform.rotation = new Quaternion(f, transform.rotation.y, transform.rotation.z, transform.rotation.w); 
                }
            }
            else
            {
                pc.SetState(PlayerController.PlayerStates.None);
                pc.animator.SetBool("loopDaze", true);
                players[i].transform.position = loseSpawn[wLose].transform.position;
                Vector3 rotation = transform.position - Vector3.zero;
                players[i].transform.rotation = Quaternion.LookRotation(rotation);
                wLose++;
            }
        }
    }

   public void ClearPlayers()
    {
        Destroy(GameObject.Find("Plane"));
    }

    void GetPlayersTray()
    {
//        GameObject[] players = GameObject.FindGameObjectsWithTag(player);
        for(int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerController>().GetTray(players[i].GetComponent<PlayerController>().color);
        }
    }

    public void RemovePlayerPicks()
    {
//        GameObject[] players = GameObject.FindGameObjectsWithTag(player);
        for (int i = 0; i < 0; i++)
        {
            players[i].GetComponent<PlayerController>().ClearPicks();
        }
    }
    
    void SwitchTrays(bool turnOn) // Turns the trays on or off depending on Game mode
    {
        if (turnOn)
        {
            trays.Select((t) => {
                t.SetActive(true);
                return t;}).ToList();

        }
        else
        {
            trays.Select((t) => {
                t.SetActive(false);
                return t;}).ToList();
        }
    }

    public void UpdateList()
    {
        trays.Select((t) => {t.GetComponentInChildren<Tray>().Reset(); return t;}).ToList(); // Puts the tray in idle and updates which ingredients to check for
    }

    public void ResetCamera()
    {
        mainCamera.position = originalCamera.position;
        mainCamera.rotation = originalCamera.rotation;
    }

    public void CheckScore() // If a team reaches 10 points they will win the game
    {           

        if (teamOneScore >= win)
        {
            winObject = pinkTeamWins.gameObject;
            winningColor = "pink";
            SetState(GameModeStates.GameOver);
            AudioController.Play ("CHAR_Announcer_PinkWins"); 
            SetState(GameModeStates.WinScreen);
        }
        else if (teamTwoScore >= win)
        {
            winObject = blueTeamWins.gameObject;
            winningColor = "blue";
            SetState(GameModeStates.GameOver);
            AudioController.Play ("CHAR_Announcer_BlueWins"); 
            SetState(GameModeStates.WinScreen);
        }
        else
        {
            SetState(GameModeStates.ShowText);
        }
    }

    IEnumerator RecordMovement()
    {
        float t = 0f, time = 0f, byTime = .1f, duration = 5f;

        while (true)
        {
            if(time < 12f)
            {
                time += Time.deltaTime;
                t = time * byTime;
                record.position = Vector3.Lerp(startPos.position, endPos.position, t);
            }
            else
            {
                record.position = startPos.position;
                time = 0f;
                yield return new WaitForSeconds(duration);
            }

            yield return null;
        }
    }

    void ShowPicture()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !pictures [picCheck].activeInHierarchy)
        {
            PauseMenu.GetInstance.Pause();
            picCamera.enabled = true;
            pictures[picCheck].SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && pictures [picCheck].activeInHierarchy)
        {
            PauseMenu.GetInstance.Unpause();
            picCamera.enabled = false;
            pictures[picCheck].SetActive(false);
            picCheck++;
        }
    }

    bool GetStartButton()
    {
        if (XCI.GetButtonDown(XboxButton.Start, 1) || XCI.GetButtonDown(XboxButton.Start, 2) ||
            XCI.GetButtonDown(XboxButton.Start, 3) || XCI.GetButtonDown(XboxButton.Start, 4))
            return true;
        else
            return false;
    }

    bool GetAButton()
    {
        if (XCI.GetButtonDown(XboxButton.A, 1) || XCI.GetButtonDown(XboxButton.A, 2) ||
            XCI.GetButtonDown(XboxButton.A, 3) || XCI.GetButtonDown(XboxButton.A, 4))
            return true;
        else
            return false;
    }
    bool GetBButton()
    {
        if (XCI.GetButtonDown(XboxButton.B, 1) || XCI.GetButtonDown(XboxButton.B, 2) ||
            XCI.GetButtonDown(XboxButton.B, 3) || XCI.GetButtonDown(XboxButton.B, 4))
            return true;
        else
            return false;
    }


    void OnApplicationQuit()
    {
        gmInstance = null;
    }

    #region spawn object
    // // // //// // // //// // // //// // // //
    public GameObject strawberry, banana, iceCream, peanut, cookie, sprinkles;
    GameObject obj = null;
    void CreateItem()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            HUD.GetInstance.numberTimer = 2f;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            obj = cookie;
            obj = Instantiate(obj, GetSpawnPoints.GetInstance.RespawnPoints[Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position,
                              obj.transform.rotation) as GameObject;
            obj.transform.parent = GameObject.Find("Ingredients").transform;
            return;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            obj = iceCream;
            obj = Instantiate(obj, GetSpawnPoints.GetInstance.RespawnPoints[Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position,
                              obj.transform.rotation) as GameObject;
            obj.transform.parent = GameObject.Find("Ingredients").transform;
            return;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            obj = banana;
            obj = Instantiate(obj, GetSpawnPoints.GetInstance.RespawnPoints[Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position,
                              obj.transform.rotation) as GameObject;
            obj.transform.parent = GameObject.Find("Ingredients").transform;
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            obj = strawberry;
            obj = Instantiate(obj, GetSpawnPoints.GetInstance.RespawnPoints[Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position,
                              obj.transform.rotation) as GameObject;
            obj.transform.parent = GameObject.Find("Ingredients").transform;
            return;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            obj = peanut;
            obj = Instantiate(obj, GetSpawnPoints.GetInstance.RespawnPoints[Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position,
                              obj.transform.rotation) as GameObject;
            obj.transform.parent = GameObject.Find("Ingredients").transform;
            return;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            obj = GameObject.Find("SprinklesIngredient");
            obj = Instantiate(obj, GetSpawnPoints.GetInstance.RespawnPoints[Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position,
                              obj.transform.rotation) as GameObject;
            obj.transform.parent = GameObject.Find("Ingredients").transform;
            return;
        }
    }
    // // // //// // // //// // // //// // // //
    
    #endregion

    #region Background Music 
    
    
    
    void backgroundMusic()
    {  
        AudioController.Play ("MUSIC_Base");
    }
    
    void overtimeMusic()
    { //Stops Background Music 
        AudioController.Stop ("MUSIC_Base",  2f); 
        AudioController.Play ("MUSIC_Overtime"); 
    }
//    void ChangeBackgroundMusic()
//    {
////        AudioController.Stop("Music_Base", 1f);
//        AudioController.Play("Music_Base", 1f, 1f, 0);
//    }

    void gameoverMusic()
    { //Stops Background Music 
        AudioController.Stop ("MUSIC_Base",  2f); 
        AudioController.Play ("MUSIC_Winner"); 
    }
    
    #endregion
}
