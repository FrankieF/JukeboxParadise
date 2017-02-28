using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XboxCtrlrInput;
using XInputDotNetPure;

public class ControllerInput : MonoBehaviour
{
    [Header("Controller Numbers")]
    public bool headerControllerNumber;
    public int blueCount, pinkCount; // The number of players per team
    public string[] characterNames = {"Johnny", "Sophia", "Ginger", "Louis"};
    public List<string> usedNames = new List<string>();

    [Header("Movement")]
    public bool headerMovement;
    public bool moved = false;
    public int menuOption = 2, previousPosition, characterOption1, characterOption2, characterOption3, characterOption4,
               previousCharacterOption1,previousCharacterOption2, previousCharacterOption3, previousCharacterOption4;
    public int zero = 0, menuMax = 2, characterMax = 3;
    public GameObject highLightText; // Used to highlight the text for the player to select
    public Vector3 moveLocation; // Position for the highLightText to move too
    public Vector2 currentInput, playerOneInput, playerTwoInput, playerThreeInput, playerFourInput; // Inputs for the players specifically and for the main menu

    [Header("UI")]
    public bool headerUI;
    public bool joinedOne, joinedTwo, joinedThree, joinedFour;
    public UIButton play, gallery, quit; // Used to set the states of the individual buttons
    public UILabel playerOneText, playerTwoText, playerThreeText, playerFourText;
    public UIWidget characterSelect, startMenu, team1, controller1, team2, controller2, team3, controller3,  team4, controller4, preloader, ready, pressStart, creditScreen, credits;
    public UIWidget[] menuOptions, teamSelect1, teamSelect2, teamSelect3, teamSelect4; // References for the moveLocation
    public UISprite playerOneRight, playerOneLeft, playerTwoRight, playerTwoLeft, playerThreeRight, playerThreeLeft, playerFourRight, playerFourLeft,
                    aButtonOne, aButtonTwo, aButtonThree, aButtonFour, playerOneBackGround, playerTwoBackgrund, playerThreeBackground, playerFourBackground,
                    shinOne, shinTwo, shinThree, shinFour;
    public UISprite[] characterOptions1, characterOptions2, characterOptions3, characterOptions4; // Used for picking characters
    public Color greyColor, blueColor, pinkColor, particle, particleBlue, particleLightBlue, particlePink, particleLightPink; // Used to change background after team select

    [Header("Coroutines")]
    public bool headerCoro;
    public bool playerOneTeam, playerTwoTeam, playerThreeTeam, playerFourTeam, teamPickedOne, teamPickedTwo, teamPickedThree, teamPickedFour;
    public float selectTime = .25f, vibrationTimeOne, vibrationTimeTwo, vibrationTimeThree, vibrationTimeFour; // Time to wait for selections inside of coroutines
    public string playerOneCharCoro = "PlayerOneCharacterSelect", playerTwoCharCoro = "PlayerTwoCharacterSelect",
                  playerThreeCharCoro = "PlayerThreeCharacterSelect", playerFourCharCoro = "PlayerFourCharacterSelect",
                  teamColorOne, teamColorTwo, teamColorThree, teamColorFour;
        
    public enum MenuStates {None, StartScreen, DoMain, CharacterSelect, CreditScreen, Preloader}
    public MenuStates currentState = MenuStates.None;
    public MenuStates targetState = MenuStates.None;
    public MenuStates previousState = MenuStates.None;

    void SetState(MenuStates state)
    {
        targetState = state;
    }

    [Header("LEVEL REMEMBER TO UPDATE")]
    public bool headerLevel;
    public string level;
    public GameObject[] players;

    private PlayerIndex[] playerLookup = new PlayerIndex[5];

    void Start()
    {
        //PLAYS Front End Music :) 
        AudioController.Play("MUSIC_FrontEnd");
        DontDestroyOnLoad(GameObject.Find("Plane"));

        playerLookup [0] = PlayerIndex.One;
        playerLookup [1] = PlayerIndex.One;
        playerLookup [2] = PlayerIndex.Two;
        playerLookup [3] = PlayerIndex.Three;
        playerLookup [4] = PlayerIndex.Four;
    }
    
    void Update()
    {
        if (currentState != targetState)
        {
            StopCoroutine(currentState.ToString());
            previousState = currentState;
            currentState = targetState;
            StartCoroutine(currentState.ToString());
        }
    }

    IEnumerator None()
    {
        float waitTime = 6f;

        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            SetState(MenuStates.StartScreen);
        }
    }

    IEnumerator StartScreen()
    {
        ResetCoroutines();
        AudioController.Play("CHAR_Announcer_Welcome");

        while (true)
        {
            if(GetStartButton())
            {
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_StartButtonPressDown"); 
                startMenu.GetComponent<TweenPosition>().PlayForward();
                MoveButtons(false); // Moves the buttons and record in
                SetState(MenuStates.DoMain);
            }
            yield return null;
        }
    }

    IEnumerator DoMain()
    {
        if (previousState == MenuStates.CharacterSelect)
        {
            ResetCoroutines(); // Stops player specific coroutines
        }
        else if (previousState == MenuStates.CreditScreen)
        {
            yield return new WaitForSeconds(.5f);
            creditScreen.gameObject.SetActive(false);
        }

        highLightText.SetActive(true);

        while(true)
        {
            currentInput = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX), XCI.GetAxis(XboxAxis.LeftStickY));
            GetAnalogMovementMain();
            GetAButton();
            GetBButton();

            if(currentInput.y != 0)
                yield return new WaitForSeconds(selectTime);

            yield return null;
        }
    }

    IEnumerator CreditScreen()
    {
        while (true)
        {
            GetBButton();
            yield return null;
        }
    }

    IEnumerator CharacterSelect()
    {
        StartCoroutine(playerOneCharCoro); // Starts the coroutines to get player input for picking characters
        StartCoroutine(playerTwoCharCoro);
        StartCoroutine(playerThreeCharCoro);
        StartCoroutine(playerFourCharCoro);

        while (true)
        {
            AllPlayersReady();
            //GetBButton();

            if(ready.gameObject.activeInHierarchy == true) // All players have picked a character
            {
                if(GetStartButton())
                {
                    //PLAYS BUTTON PRESS DOWN SFX :) 
                    AudioController.Play("UI_StartButtonPressDown"); 
                    StopCoroutines();
                    SetState(MenuStates.Preloader);
                }
            }
            yield return null;
        }
    }

    IEnumerator Preloader()
    {
        float preloadTime = 2f;
        bool loaded = false;

        ready.gameObject.SetActive(false);
        preloader.gameObject.SetActive(true);

        while (true)
        {
            //Application.LoadLevelAsync("level");

            if(loaded == false)
            {
                yield return new WaitForSeconds(preloadTime); // Loading is finished and it shows press start
                loaded = true;
                pressStart.gameObject.SetActive(true);
            }

            if(GetStartButton())
            {
                TurnOffPlayers();
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_StartButtonPressDown");
                AudioController.Stop("MUSIC_FrontEnd"); 
                Application.LoadLevel(level); // Loads level
            }

            yield return null;
        }
    }

    IEnumerator PlayerOneCharacterSelect()
    {
        while (true)
        {
            if(!joinedOne)
            {
                GetAButton(1, aButtonOne, ref joinedOne);
                GetBButton();
            }
            else if(!teamPickedOne)
            {
                previousCharacterOption1 = characterOption1; // Updates the character shown
                playerOneInput = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, 1), XCI.GetAxis(XboxAxis.LeftStickY, 1)); // New input
                GetCharacterSelection( playerOneTeam, playerOneInput, ref characterOption1, controller1, teamSelect1, playerOneRight, playerOneLeft); // Checks weather the character shown should change and if the player picks a team
                UpdateCharacterSelection(characterOption1, previousCharacterOption1, characterOptions1, playerOneText); // Updates the character selection with the picture at the current int
                GetAButton(1, team1, characterOptions1, characterOption1, characterOptions2, characterOptions3, characterOptions4,
                           ref playerOneTeam, controller1, teamSelect1, ref teamPickedOne, ref teamColorOne, ref playerOneBackGround,
                           playerOneLeft, playerOneRight, shinOne, vibrationTimeOne); // Picks a character for the player and picks a team
                GetBButton(1, playerOneTeam, aButtonOne, ref joinedOne);
                GetBButton(1, ref playerOneTeam, team1, teamSelect1, controller1, characterOptions1, characterOption1,
                           characterOptions2, characterOptions3, characterOptions4); // Goes back to the main menu or back to character selection

                StopVibration(1, vibrationTimeOne);
                vibrationTimeOne -= Time.deltaTime;

                if(playerOneInput.x != zero)
                    yield return new WaitForSeconds(selectTime);
            }
            else
            {
                GetBButton(1, ref teamPickedOne, teamColorOne, ref playerOneBackGround,
                           team1, playerOneLeft, playerOneRight); // Removes player from team
            }
            yield return null;
        }
    }

    IEnumerator PlayerTwoCharacterSelect()
    {
        while (true)
        {
            if(!joinedTwo)
            {
                GetAButton(2, aButtonTwo, ref joinedTwo);
            }
            else if(!teamPickedTwo)
            {
                previousCharacterOption2 = characterOption2; // Updates the character shown
                playerTwoInput = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, 2), XCI.GetAxis(XboxAxis.LeftStickY, 2)); // New input
                GetCharacterSelection(playerTwoTeam, playerTwoInput, ref characterOption2, controller2, teamSelect2, playerTwoRight, playerTwoLeft); // Checks weather the character shown should change and if the player picks a team
                UpdateCharacterSelection(characterOption2, previousCharacterOption2, characterOptions2, playerTwoText); // Updates the character selection with the picture at the current int
                GetAButton(2, team2, characterOptions2, characterOption2, characterOptions1, characterOptions3, characterOptions4,
                           ref playerTwoTeam, controller2, teamSelect2, ref teamPickedTwo, ref teamColorTwo, ref playerTwoBackgrund,
                           playerTwoLeft, playerTwoRight, shinTwo, vibrationTimeTwo); // Picks a character for the player and picks a team
                GetBButton(2, playerTwoTeam, aButtonTwo, ref joinedTwo);
                GetBButton(2, ref playerTwoTeam, team2, teamSelect2, controller2, characterOptions2, characterOption2,
                           characterOptions1, characterOptions3, characterOptions4); // Goes back to the main menu or back to character selection

                StopVibration(2, vibrationTimeTwo);
                vibrationTimeTwo -= Time.deltaTime;

                if(playerTwoInput.x != zero)
                    yield return new WaitForSeconds(selectTime);
            }
            else
            {
                GetBButton(2, ref teamPickedTwo, teamColorTwo, ref playerTwoBackgrund, team2, playerTwoLeft, playerTwoRight); // Removes player from team
            }
            yield return null;
        }
    }

    IEnumerator PlayerThreeCharacterSelect()
    {
        while (true)
        {
            if(!joinedThree)
            {
                GetAButton(3, aButtonThree, ref joinedThree);
            }
            else if(!teamPickedThree)
            {
                previousCharacterOption3 = characterOption3; // Updates the character shown
                playerThreeInput = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, 3), XCI.GetAxis(XboxAxis.LeftStickY, 3)); // New input
                GetCharacterSelection(playerThreeTeam, playerThreeInput, ref characterOption3, controller3, teamSelect3, playerThreeRight, playerThreeLeft); // Checks weather the character shown should change and if the player picks a team
                UpdateCharacterSelection(characterOption3, previousCharacterOption3, characterOptions3, playerThreeText); // Updates the character selection with the picture at the current int
                GetAButton(3, team3, characterOptions3, characterOption3, characterOptions1, characterOptions2, characterOptions4,
                           ref playerThreeTeam, controller3, teamSelect3, ref teamPickedThree, ref teamColorThree, ref playerThreeBackground,
                           playerThreeLeft, playerThreeRight, shinThree, vibrationTimeThree); // Picks a character for the player and picks a team
                GetBButton(3, playerThreeTeam, aButtonThree, ref joinedThree);
                GetBButton(3, ref playerThreeTeam, team3, teamSelect3, controller3, characterOptions3, characterOption3,
                           characterOptions1, characterOptions2, characterOptions4); // Goes back to the main menu or back to character selection

                StopVibration(3, vibrationTimeThree);
                vibrationTimeThree -= Time.deltaTime;

                if(playerThreeInput.x != zero)
                    yield return new WaitForSeconds(selectTime);
            }
            else
            {
                GetBButton(3, ref teamPickedThree, teamColorThree, ref playerThreeBackground, team3, playerThreeLeft, playerThreeRight); // Removes player from team
            }
            yield return null;
        }
    }

    IEnumerator PlayerFourCharacterSelect()
    {
        while (true)
        {
            if(!joinedFour)
            {
                GetAButton(4, aButtonFour, ref joinedFour);
            }
            else if(!teamPickedFour)
            {
                previousCharacterOption4 = characterOption4; // Updates the character shown
                playerFourInput = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, 4), XCI.GetAxis(XboxAxis.LeftStickY, 4)); // New input
                GetCharacterSelection(playerFourTeam, playerFourInput, ref characterOption4, controller4, teamSelect4, playerFourRight, playerFourLeft); // Checks weather the character shown should change and if the player picks a team
                UpdateCharacterSelection(characterOption4, previousCharacterOption4, characterOptions4, playerFourText); // Updates the character selection with the picture at the current int
                GetAButton(4, team4, characterOptions4, characterOption4, characterOptions1, characterOptions2, characterOptions3,
                            ref playerFourTeam, controller4, teamSelect4, ref teamPickedFour, ref teamColorFour, ref playerFourBackground,
                           playerFourLeft, playerFourRight, shinFour, vibrationTimeFour); // Picks a character for the player and picks a team
                GetBButton(4, playerFourTeam, aButtonFour,ref joinedFour);
                GetBButton(4, ref playerFourTeam, team4, teamSelect4, controller4, characterOptions4, characterOption4,
                           characterOptions1, characterOptions2, characterOptions3); // Goes back to the main menu or back to character selection

                StopVibration(4, vibrationTimeFour);
                vibrationTimeFour -= Time.deltaTime;

                if(playerFourInput.x != zero)
                    yield return new WaitForSeconds(selectTime);
            }
            else
            {
                GetBButton(4, ref teamPickedFour, teamColorFour, ref playerFourBackground, team4, playerFourLeft, playerFourRight); // Removes player from team
            }
            yield return null;
        }
    }

    void GetAnalogMovementMain()
    {
        if (currentInput.y > zero)
        {
            if(menuOption != menuMax)
                menuOption++;
        }
        else if (currentInput.y < zero)
        {
            if(menuOption != zero)
                menuOption--;            
        }
        highLightText.transform.position = menuOptions [menuOption].transform.position;
        float x = highLightText.transform.position.x;
        x -= .1f;
        highLightText.transform.position = new Vector3(x, highLightText.transform.position.y, highLightText.transform.position.z);
        DetectPosition(menuOption);
        previousPosition = menuOption;        
    }

    void GetCharacterSelection(bool playerTeam, Vector2 playerInput, ref int option, UIWidget controller, UIWidget[] teamSelection, UISprite playerArrowRight, UISprite playerArrowLeft) // Checks for player input
    {
        if (!playerTeam) // If the player has not selected a team then they will cycle through the characters
        {
            if (playerInput.x > zero)
            {
                if (option != characterMax)
                    option++;
                else
                    option = zero;
                playerArrowRight.GetComponent<TweenScale>().ResetToBeginning();
                playerArrowRight.GetComponent<TweenScale>().enabled = true;
            } 
            else if (playerInput.x < 0)
            {
                if (option != zero)
                    option--;
                else
                    option = characterMax;
                playerArrowLeft.GetComponent<TweenScale>().ResetToBeginning();
                playerArrowLeft.GetComponent<TweenScale>().enabled = true;
            }
        }
        else // If a player has selected a team they will be able to pick the pink or blue team
        {
            if(playerInput.x < zero) 
            {
                if(controller.transform.position != teamSelection[zero].transform.position)
                    controller.transform.position = teamSelection[zero].transform.position;
            }
            else if (playerInput.x > zero)
            {
                if(controller.transform.position != teamSelection[2].transform.position)
                    controller.transform.position = teamSelection[2].transform.position;
            }
        }
    }

    void UpdateCharacterSelection(int characterOption, int previousOption, UISprite[] characters, UILabel characterName) // Updates player input
    {
        if (characterOption != previousOption)
        {
            characters [previousOption].gameObject.SetActive(false);
            characters [characterOption].gameObject.SetActive(true);
            characterName.text = characterNames[characterOption];
        }
    }

    void DetectPosition(int number) // This makes the buttons on the main menu change color when the player hover over them
    {
        int one = 1, two = 2;

        if (number != previousPosition)
        {
            if (number == zero)
            {
                gallery.OnHover(false);
                play.OnHover(false);
                quit.OnHover(true);
            }
            else if (number == one)
            {
                quit.OnHover(false);
                play.OnHover(false);
                gallery.OnHover(true);
            }
            else if (number == two)
            {
                quit.OnHover(false);
                gallery.OnHover(false);
                play.OnHover(true);
            }

            //Plays hover SFX
            AudioController.Play("UI_ButtonHover"); 
        }
    }

    public void GetAButton()
    {
        if (GetAButtonDown())
        {
            if (menuOption == menuMax)
            {
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_ButtonPressDown"); 
                highLightText.SetActive(false);
                MoveButtons(true);
                Invoke("MoveCharacterSelectForward", .5f);
                SetState(MenuStates.CharacterSelect);
            } 
            else if (menuOption == 1)
            {
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_ButtonPressDown"); 
                highLightText.SetActive(false);
                creditScreen.gameObject.SetActive(true);
                credits.GetComponent<TweenPosition>().enabled = true;
                MoveButtons(true);
                SetState(MenuStates.CreditScreen);
            }
            else if (menuOption == zero)
            {
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_ButtonPressDown"); 
                Quit();
            }
        }
    }

    public void GetAButton(int playerNumber, UISprite AButtonCover, ref bool joined)
    {
        if (XCI.GetButtonDown(XboxButton.A, playerNumber))
        {
            AButtonCover.GetComponent<TweenColor>().enabled = true;
            AButtonCover.GetComponent<TweenAlpha>().enabled = true;
            joined = true;
        }
    }

    public void GetAButton(int playerNumber, UIWidget widget, UISprite[] characters, int option, UISprite[] characters2, UISprite[] characters3,
                           UISprite[] characters4, ref bool teamNumber, UIWidget controller, UIWidget[] teamSelection, ref bool teamPick, ref string teamColor,
                           ref UISprite playerBackground, UISprite leftArrow, UISprite rightArrow, UISprite shin, float vibrationTime)
    {
        if (XCI.GetButtonDown(XboxButton.A, playerNumber))
        {
            if (!teamNumber)
            {
                if(!HasName(characters[option].name))
                   {
                    //PLAYS BUTTON PRESS DOWN SFX :) 
                    AudioController.Play("UI_ButtonPressDown"); 
                    //characters [option].gameObject.SetActive(false);
                    widget.gameObject.SetActive(true);
                    teamNumber = true;
                    SetPlayerNumber(playerNumber, option);
                    FadeCharacters(true, characters[option].name, characters2, characters3, characters4);
                    playCharacterSelect(option);
                    usedNames.Add(characters[option].name);
                    GamePad.SetVibration(playerLookup[playerNumber], 3f, 3f);
                    vibrationTime = .1f;
                }
                else
                {
                    // Plays audio to tell player that cannot be picked
                    AudioController.Play("UI_ButtonPressDown"); 
                    GamePad.SetVibration(playerLookup[playerNumber], 3f, 3f);
                    vibrationTime = .1f;
                }

            } 
            else if (teamNumber)
            {
                shin.GetComponent<TweenPosition>().ResetToBeginning();
                if(controller.transform.position == teamSelection[zero].transform.position)
                {
                    //PLAYS BUTTON PRESS DOWN SFX :) 
                    AudioController.Play("UI_ButtonPressDown"); 
                    teamPick = true;
                    SetPlayerBlue(option);
                    playerBackground.color = blueColor;
                    PlayTeamPickedBlue();
                    SetPlayerTeamNumber(playerNumber, blueCount);
                    blueCount++;
                }
                else if (controller.transform.position == teamSelection[2].transform.position)
                {
                    //PLAYS BUTTON PRESS DOWN SFX :) 
                    AudioController.Play("UI_ButtonPressDown"); 
                    teamPick = true;
                    SetPlayerPink(option);
                    playerBackground.color = pinkColor;
                    PlayTeamPickedPink();
                    SetPlayerTeamNumber(playerNumber, pinkCount);
                    pinkCount++;

                }
                else return;

                shin.GetComponent<TweenPosition>().PlayForward();
                widget.gameObject.SetActive(false);
                leftArrow.gameObject.SetActive(false);
                rightArrow.gameObject.SetActive(false);
            }
        }
    }

    public void GetBButton()
    {
        if (GetBButtonDown())
        {
            if (currentState == MenuStates.CharacterSelect)
            {
                if(!joinedOne && !joinedTwo && !joinedThree && !joinedFour)
                {
                    //PLAYS BUTTON PRESS DOWN SFX :) 
                    AudioController.Play("UI_ButtonPressDown"); 
                    MoveButtons(false);
                    MoveCharacterSelectBackward();
                    ResetCoroutines();
                    ResetTeamSelect();
                    SetState(MenuStates.DoMain);
                }
            }
            else if (currentState == MenuStates.DoMain)
            {
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_ButtonPressDown"); 
                highLightText.gameObject.SetActive(false);
                MoveButtons(true);
                startMenu.GetComponent<TweenPosition>().PlayReverse();
                SetState(MenuStates.StartScreen);
            }
            else if (currentState == MenuStates.CreditScreen) 
            {
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_ButtonPressDown"); 
                credits.GetComponent<TweenPosition>().ResetToBeginning();
                MoveButtons(false);
                SetState(MenuStates.DoMain);
            }
        }
    }

    public void GetBButton(int playerNumber, bool playerTeam, UISprite aButtonCover, ref bool joined)
    {
        if (XCI.GetButtonDown(XboxButton.B, playerNumber))
        {
            if(!playerTeam)
            {
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_ButtonPressDown");
                aButtonCover.GetComponent<TweenColor>().enabled = true;
                aButtonCover.GetComponent<TweenColor>().PlayReverse();                
                joined = false;
            }
        }
    }

    public void GetBButton(int playerNumber, ref bool teamPicked, string teamColor, ref UISprite playerBackground,
                           UIWidget widget, UISprite leftArrow, UISprite rightArrow)
    {
        if (XCI.GetButton(XboxButton.B, playerNumber))
        {
            //PLAYS BUTTON PRESS DOWN SFX :) 
            AudioController.Play("UI_ButtonPressDown");
            teamPicked = false;

            if(playerBackground.color == blueColor)
            {
                blueCount--;
                SetPlayerTeamNumber(playerNumber, blueCount);
            }
            else
            {
                pinkCount--;
                SetPlayerTeamNumber(playerNumber, pinkCount);
            }

            playerBackground.color = greyColor;
            widget.gameObject.SetActive(true);
            leftArrow.gameObject.SetActive(true);
            rightArrow.gameObject.SetActive(true);
        }
    }

    public void GetBButton(int playerNumber, ref bool teamNumber, UIWidget widget, UIWidget[] teamSelect, UIWidget controller, UISprite[] characters, int option,
                           UISprite[] characters1, UISprite[] characters2, UISprite[] characters3)
    {
        if (XCI.GetButtonDown(XboxButton.B, playerNumber))
        {
            if(teamNumber)
            {
                //PLAYS BUTTON PRESS DOWN SFX :) 
                AudioController.Play("UI_ButtonPressDown"); 
                widget.gameObject.SetActive(false);
                controller.transform.position = teamSelect[1].transform.position;
                characters[option].gameObject.SetActive(true);
                teamNumber = false;
                FadeCharacters(false, characters[option].name, characters1, characters2, characters3);
                ChangePlayerNumber(option);
                usedNames.Remove(characters[option].name);
            }
        }
    }

    void MoveButtons(bool forward) // This moves the buttons and record into and out of the screen
    {
        if (forward)
        {
            foreach (var button in menuOptions)
            {
                button.GetComponent<TweenPosition>().PlayReverse();
            }
        } 
        else
            foreach (var button in menuOptions)
            {
                button.GetComponent<TweenPosition>().PlayForward();
            }
    }

    void MoveCharacterSelectForward()
    {
        characterSelect.GetComponent<TweenPosition>().PlayForward(); // Moves characters into the screen
    }

    void MoveCharacterSelectBackward()
    {
        characterSelect.GetComponent<TweenPosition>().PlayReverse(); // Moves characters out of the screen
    }

    void FadeCharacters()
    {
        Color color = new Color(0.0f, 0.0f, 0.0f);
        Color aColor = new Color(255f, 255f, 255f);
        foreach (var c in teamSelect1)
        {
            if(c.color == color)
                c.color = aColor;
        }

        foreach (var c in teamSelect2)
        {
            if(c.color == color)
                c.color = aColor;
        }

        foreach (var c in teamSelect3)
        {
            if(c.color == color)
                c.color = aColor;
        }

        foreach (var c in teamSelect4)
        {
            if(c.color == color)
                c.color = aColor;
        }
    }

    void FadeCharacters(bool picked, string characterPicked, UISprite[] characters1, UISprite[] characters2, UISprite[] characters3)
    {
        if (picked)
        {
            Color color = new Color(0.0f, 0.0f, 0.0f); // When characters are picked they are turned to a black silhoutte

            foreach (var character in characters1)
            {
                if (character.name.Contains(characterPicked))
                    character.color = color;
            }
            foreach (var character in characters2)
            {
                if (character.name.Contains(characterPicked))
                    character.color = color;
            }
            foreach (var character in characters3)
            {
                if (character.name.Contains(characterPicked))
                    character.color = color;
            }
        } 
        else
        {
            Color color = new Color(255f, 255f, 255f); // Restores color if someone deselects a character

            foreach (var character in characters1)
            {
                if (character.name.Contains(characterPicked))
                    character.color = color;
            }
            foreach (var character in characters2)
            {
                if (character.name.Contains(characterPicked))
                    character.color = color;
            }
            foreach (var character in characters3)
            {
                if (character.name.Contains(characterPicked))
                    character.color = color;
            }
        }
    }

    void AllPlayersReady()
    {
        int count = 0;

        // Checks for which players joined the games and picked teams
        count += (CheckPlayer(joinedOne, playerOneTeam, teamPickedOne) + CheckPlayer(joinedTwo, playerTwoTeam, teamPickedTwo) + 
                  CheckPlayer(joinedThree, playerThreeTeam, teamPickedThree) + CheckPlayer(joinedFour, playerFourTeam, teamPickedFour));

        ready.gameObject.SetActive(count >= 6 && BothTeamsPicked()? true : false);
    }

    int CheckPlayer(bool join, bool team, bool pick)
    {
        if (join)
        {
            if (team)
            {
                if (pick)
                    return 3;
                else
                    return -20;
            }
            else
                return -20;
        }
        else if (!joinedOne)
            return 0;
        else
            return 0;
    }

    bool BothTeamsPicked()
    {
        return blueCount > 0 && pinkCount > 0 ? true : false;
    }

    void ResetTeamSelect()
    {
        playerOneTeam = false; // Sets all teams picks to false
        playerTwoTeam = false;
        playerThreeTeam = false;
        playerFourTeam = false;

        controller1.transform.position = teamSelect1[1].transform.position; // Resets team select position
        controller2.transform.position = teamSelect2[1].transform.position;
        controller3.transform.position = teamSelect3[1].transform.position;
        controller4.transform.position = teamSelect4[1].transform.position;

        team1.gameObject.SetActive(false); // Turns team select off
        team2.gameObject.SetActive(false);
        team3.gameObject.SetActive(false);
        team4.gameObject.SetActive(false);

        FadeCharacters();
    }

    void ResetCoroutines()
    {
        StopCoroutines();
        playerOneTeam = false;
        playerTwoTeam = false;
        playerThreeTeam = false;
        playerFourTeam = false;
    }

    void StopCoroutines()
    {
        StopCoroutine(playerOneCharCoro);
        StopCoroutine(playerTwoCharCoro);
        StopCoroutine(playerThreeCharCoro);
        StopCoroutine(playerFourCharCoro);
    }

    bool GetStartButton()
    {
        if (XCI.GetButtonDown(XboxButton.Start, 1) || XCI.GetButtonDown(XboxButton.Start, 2) ||
            XCI.GetButtonDown(XboxButton.Start, 3) || XCI.GetButtonDown(XboxButton.Start, 4))
            return true;
        else
            return false;
    }

    bool GetAButtonDown()
    {
        if (XCI.GetButtonDown(XboxButton.A, 1) || XCI.GetButtonDown(XboxButton.A, 2) ||
            XCI.GetButtonDown(XboxButton.A, 3) || XCI.GetButtonDown(XboxButton.A, 4))
            return true;
        else
            return false;
    }

    bool GetBButtonDown()
    {
        if (XCI.GetButtonDown(XboxButton.B, 1) || XCI.GetButtonDown(XboxButton.B, 2) ||
            XCI.GetButtonDown(XboxButton.B, 3) || XCI.GetButtonDown(XboxButton.B, 4))
            return true;
        else
            return false;
    }

    void SetPlayerNumber(int playerNumber, int option)
    {
        string number = (option + 1).ToString();
        var player = players.First((p) => (p.name.Contains(number)));
        {
            player.GetComponent<PlayerController>().playerNumber = playerNumber;
            player.GetComponent<PlayerController>().inGame = true;
            player.GetComponent<PlayerController>().p.text += playerNumber.ToString();
        }
    }

    void ChangePlayerNumber(int option)
    {
        string number = (option + 1).ToString();
        var player = players.First((p) => (p.name.Contains(number)));
        {
            player.GetComponent<PlayerController>().inGame = false;
        }
    }

    void SetPlayerBlue(int option)
    {
        string number = (option + 1).ToString();
        var player = players.First((p) => (p.name.Contains(number)));
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            pc.ring.renderer.material.SetColor("_color", player.GetComponent<PlayerController>().blueColor);
            pc.color = "blue";
            pc.spinParticle = player.transform.FindChild("Particles").transform.FindChild("Spin_Blue").transform.FindChild("SpinSwirl").transform.FindChild("groundGlow_BLUE").GetComponent<ParticleSystem>();
            pc.pivot = player.transform.FindChild("Particles").transform.FindChild("Spin_Blue").transform.FindChild("SpinSwirl").transform.FindChild("groundGlow_BLUE").transform.FindChild("PivotBlue").gameObject;
            pc.spinHitParticle = player.transform.FindChild("Particles").transform.FindChild("SpinHit_Blue").GetComponent<ParticleSystem>();

            GameObject[] parts = pc.particlesToChange;
            foreach(var t in parts)
            {
                if(t.name.Contains("Ingr"))
                    t.GetComponent<ParticleSystem>().startColor = particleBlue;
                else if(t.name.Contains("frie"))
                    t.GetComponent<ParticleSystem>().startColor = particleBlue;
                else if(t.name.Contains("Norm"))
                    t.GetComponent<ParticleSystem>().startColor = particleBlue;
                else if(t.name.Contains("Daze"))
                    t.GetComponent<ParticleSystem>().startColor = particleBlue;                
            }
        }
    }

    void SetPlayerPink(int option)
    {
        string number = (option + 1).ToString();
        var player = players.First((p) => (p.name.Contains(number)));
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            pc.ring.renderer.material.SetColor("_color", player.GetComponent<PlayerController>().pinkColor);
            pc.color = "pink";
            pc.spinParticle = player.transform.FindChild("Particles").transform.FindChild("Spin_Pink").transform.FindChild("SpinSwirl").transform.FindChild("groundGlow_PINK").GetComponent<ParticleSystem>();
            pc.pivot = player.transform.FindChild("Particles").transform.FindChild("Spin_Pink").transform.FindChild("SpinSwirl").transform.FindChild("groundGlow_PINK").transform.FindChild("Pivot").gameObject;
            pc.spinHitParticle = player.transform.FindChild("Particles").transform.FindChild("SpinHit_Pink").GetComponent<ParticleSystem>();

            GameObject[] parts = pc.particlesToChange;

            foreach(var t in parts)
            {
                if(t.name.Contains("Ingr"))
                    t.GetComponent<ParticleSystem>().startColor = particlePink;
                else if(t.name.Contains("frie"))
                    t.GetComponent<ParticleSystem>().startColor = particlePink;
                else if(t.name.Contains("Norm"))
                    t.GetComponent<ParticleSystem>().startColor = particlePink;
                else if(t.name.Contains("Daze"))
                    t.GetComponent<ParticleSystem>().startColor = particlePink;
            }
        }
    }

    void SetPlayerTeamNumber(int playerNumber, int color)
    {
        string number = playerNumber.ToString();
        var player = players.First((p) => (p.name.Contains(number)));
        {
            player.GetComponent<PlayerController>().teamNumber = color;
        }
    }

    bool HasName(string name)
    {
        for (int i = 0; i < usedNames.Count; i++)
        {
            if(usedNames[i].Contains(name))
                return true;
        }
        return false;
    }

    void TurnOffPlayers()
    {
        for(int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(players[i].GetComponent<PlayerController>().inGame ? true : false);
        }
    }

    void StopVibration(int playerNumber, float vibrationTime)
    {
        if (vibrationTime <= 0)
        {
            GamePad.SetVibration(playerLookup[playerNumber], 0f, 0f);
        }
    }
    
    void Quit()
    {
        Application.Quit();
    }


    ///////////Audio 
    void playCharacterSelect(int characternumber)
    {
        characternumber += 1;
        AudioController.Play(GameObject.Find("Player " + characternumber.ToString()).GetComponent<PlayerController>().charcterSelectAudio);
    }

    void PlayTeamPickedBlue()
    {
        AudioController.Play("CHAR_Announcer_BlueTeam");
    }

    void PlayTeamPickedPink()
    {
        AudioController.Play("CHAR_Announcer_PinkTeam");
    }
}
