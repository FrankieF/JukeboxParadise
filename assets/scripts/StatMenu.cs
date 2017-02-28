using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatMenu : MonoBehaviour 
{
    public static StatMenu smInstance = null;

    public static StatMenu GetInstance
    {
        get
        {
            smInstance = smInstance != null ? smInstance : GameObject.Find("Main Camera").GetComponent<StatMenu>();
            return smInstance;
        }
    }

    public UILabel  playerOne, playerTwo, playerThree, playerFour, // The names for the players
                    punchesOne, punchesTwo, punchesThree, punchesFour, // The punch scores
                    hitsOne, hitsTwo, hitsThree, hitsFour, // The hit scores
                    counterOne, counterTwo, counterThree, counterFour, // Times a player counters
                    dazeOne, dazeTwo, dazeThree, dazeFour, // The number of times players daze other players
                    scoreOne, scoreTwo, scoreThree, scoreFour, // How many times the player scores
                    knockOne, knockTwo, knockThree, knockFour, // How many times they knock an ingredient off
                    p1p1, p1p2, p1p3, p1p4, // Player one punch totals
                    p2p1, p2p2, p2p3, p2p4, // Player two punch totals
                    p3p1, p3p2, p3p3, p3p4, // Player three punch totals
                    p4p1, p4p2, p4p3, p4p4; // Player four punch totals

    private PlayerController pc1, pc2, pc3, pc4; // The references to players

    void Start()
    {
        if (GameObject.Find("Player 1"))
            pc1 = GameObject.Find("Player 1").GetComponent<PlayerController>();
        if (GameObject.Find("Player 2"))
            pc2 = GameObject.Find("Player 2").GetComponent<PlayerController>();
        if (GameObject.Find("Player 3"))
            pc3 = GameObject.Find("Player 3").GetComponent<PlayerController>();
        if (GameObject.Find("Player 4"))
            pc4 = GameObject.Find("Player 4").GetComponent<PlayerController>();
    }

    public void UpdateStats()
    {
        if (pc1)
            GetPlayerOne();
        if (pc2)
            GetPlayerTwo();
        if (pc3)
            GetPlayerThree();
        if (pc4)
            GetPlayerFour();
    }

    void GetPlayerOne()
    {
        playerOne.effectColor = pc1.color.Contains("bl") ? pc1.blueColor : pc1.pinkColor;
        punchesOne.text = pc1.punches.ToString();
        hitsOne.text = pc1.hits.ToString();
        counterOne.text = pc1.counters.ToString();
        dazeOne.text = pc1.dazes.ToString();
        scoreOne.text = pc1.scored.ToString();
        knockOne.text = pc1.knockedOff.ToString(); 
        p1p1.text = pc1.johnnyHit.ToString();
        p1p2.text = pc1.sophiaHit.ToString();
        p1p3.text = pc1.gingerHit.ToString();
        p1p4.text = pc1.louisHit.ToString();
    }

    void GetPlayerTwo()
    {
        playerTwo.effectColor = pc2.color.Contains("bl") ? pc2.blueColor : pc2.pinkColor;
        punchesTwo.text = pc2.punches.ToString();
        hitsTwo.text = pc2.hits.ToString();
        counterTwo.text = pc2.counters.ToString();
        dazeTwo.text = pc2.dazes.ToString();
        scoreTwo.text = pc2.scored.ToString();
        knockTwo.text = pc2.knockedOff.ToString();
        p2p1.text = pc2.johnnyHit.ToString();
        p2p2.text = pc2.sophiaHit.ToString();
        p2p3.text = pc2.gingerHit.ToString();
        p2p4.text = pc2.louisHit.ToString();

    }

    void GetPlayerThree()
    {
        playerThree.effectColor = pc3.color.Contains("bl") ? pc3.blueColor : pc3.pinkColor;
        punchesThree.text = pc3.punches.ToString();
        hitsThree.text = pc3.hits.ToString();
        counterThree.text = pc3.counters.ToString();
        dazeThree.text = pc3.dazes.ToString();
        scoreThree.text = pc3.scored.ToString();
        knockThree.text = pc3.knockedOff.ToString();
        p3p1.text = pc3.johnnyHit.ToString();
        p3p2.text = pc3.sophiaHit.ToString();
        p3p3.text = pc3.gingerHit.ToString();
        p3p4.text = pc3.louisHit.ToString();
    }

    void GetPlayerFour()
    {
        playerFour.effectColor = pc4.color.Contains("bl") ? pc4.blueColor : pc4.pinkColor;
        punchesFour.text = pc4.punches.ToString();
        hitsFour.text = pc4.hits.ToString();
        counterFour.text = pc4.counters.ToString();
        dazeFour.text = pc4.dazes.ToString();
        scoreFour.text = pc4.scored.ToString();
        knockFour.text = pc4.knockedOff.ToString();
        p4p1.text = pc4.johnnyHit.ToString();
        p4p2.text = pc4.sophiaHit.ToString();
        p4p3.text = pc4.gingerHit.ToString();
        p4p4.text = pc4.louisHit.ToString();
    }
}
