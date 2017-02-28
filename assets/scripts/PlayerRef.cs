using UnityEngine;
using System.Collections;

public class PlayerRef : MonoBehaviour 
{
    public int pOne = 1 , pTwo = 2, pThree = 3, pFour = 4; // Player numbers when players pick
    public string colorOne, colorTwo, colorThree, colorFour; // Bools for team color

    public static PlayerRef pInstance = null;

    public static PlayerRef GetInstance
    {
        get
        {
            pInstance = pInstance != null ? pInstance : GameObject.Find("PlayerRef").GetComponent<PlayerRef>();
            return pInstance;
        }
    }

    public void StoreCharaterPick(int playerNumber, int option)
    {
        if (playerNumber == 1)
            pOne = option;
        else if (playerNumber == 2)
            pTwo = option;
        else if (playerNumber == 3)
            pThree = option;
        else
            pFour = option;
    }

    public void RevertCharacterPick(int playerNumber)
    {
        int option = 0;
        if (playerNumber == 1)
            pOne = option;
        else if (playerNumber == 2)
            pTwo = option;
        else if (playerNumber == 3)
            pThree = option;
        else
            pFour = option;
    }

    public void StoreTeamPick(int playerNumber, string teamColor)
    {
        if (playerNumber == 1)
            colorOne = teamColor;
        else if (playerNumber == 2)
            colorTwo = teamColor;
        else if (playerNumber == 3)
            colorThree = teamColor;
        else
            colorFour = teamColor;
    }

    public void RevertTeamPick(int playerNumber)
    {
        string teamColor = "";
        if (playerNumber == 1)
            colorOne = teamColor;
        else if (playerNumber == 2)
            colorTwo = teamColor;
        else if (playerNumber == 3)
            colorThree = teamColor;
        else
            colorFour = teamColor;
    }

    public void GetCharacterInformation(int playerNumer, int setNumber, string color, Color blue, Color pink, GameObject ring)
    {
        playerNumer = setNumber;
        if(color.Contains("blue"))
            ring.renderer.material.SetColor("_color", blue);
        else
           ring.renderer.material.SetColor("_color", pink);
    }
}
