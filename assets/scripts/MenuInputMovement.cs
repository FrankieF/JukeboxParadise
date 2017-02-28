using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class MenuInputMovement : MonoBehaviour 
{
    public int menuOption = 1;
    public GameObject highLightText; // Used to highlight the text for the player to select
    public GameObject menu; // The parent of the menu options
    public GameObject moveOffScreenPosition; // Where the object moves too
    public GameObject[] menuOptions; // References for the moveLocation
    public GameObject characterSelect; // Character Selection screen
    public GameObject creditsMenu;
    public GameObject quitGame;
    public Vector3 moveLocation; // Position for the highLightText to move too
	
	// Update is called once per frame
	void Update () 
    {
        GetPlayerInput();
        highLightText.transform.position = moveLocation;
	}

    void GetPlayerInput()
    {
        if (XCI.GetAxisRaw(XboxAxis.LeftStickY) > 0)
        {
            if(menuOption != 2)
                menuOption++;
        }
        else if (XCI.GetAxisRaw(XboxAxis.LeftStickY) < 0)
        {
            if(menuOption != 0)
                menuOption--;

        }
        moveLocation = menuOptions [menuOption].transform.position;

        GetAButton();
    }

    void GetAButton()
    {
        if (XCI.GetButtonDown(XboxButton.A))
        {
            if(highLightText.transform.position == menuOptions[0].transform.position)
            {
                Application.Quit();
            }
            if(highLightText.transform.position == menuOptions[1].transform.position)
            {
//                menu.transform.position = moveOffScreenPosition;
            }
            if(highLightText.transform.position == menuOptions[2].transform.position)
            {
//                menu.transform.position = moveOffScreenPosition;
            }
        }
    }
}
