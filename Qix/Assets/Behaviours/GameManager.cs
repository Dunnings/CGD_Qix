﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Created by David Dunnings
//...But implemented by Daniel Weston ;p xoxo

public class GameManager : MonoBehaviour
{
    //singleton reference
    public static GameManager instance;

    //game state
    public GameStates m_state;

    //UI references
    public GameObject menuCanvas;
    public GameObject worldCanvas;
    public GameObject uiCanvas;
    public List<GameObject> playerUIElements;
    public GameObject menuCreditText;
    public GameObject gameCreditText;

    private string creditsString = "CREDITS";
    private int creditsInt = 64;

    //GameObject references 
    public List<GameObject> _players = new List<GameObject>();
    
	void Awake()
    {
        instance = this;
        //Get all Connected Game Controllers
        InputManager.GetControllers();

        //set up UI
        int length = InputManager._indexOfControllers.Count;
        int controlIndex = 0;
        for (int i = 0; i < length; i++)
        {
            //set up player icon in ui
            playerUIElements[i].SetActive(true);

            _players[i].SetActive(true);     

            //set up player and controller indexes
            _players[i].GetComponent<CharMovement>().playerIndex = i;
            _players[i].GetComponent<CharMovement>().controllerIndex = controlIndex;

            controlIndex++;
           
        }

        //set state to menu
        m_state = GameStates.menu;
	}
	
	void Update () 
    {
        switch (m_state)
        {
            case GameStates.menu:
                //set up UI
                worldCanvas.SetActive(true);
                menuCanvas.SetActive(true);
                uiCanvas.SetActive(false);

                //in char select loop over max amount of active players
                for (int i = 0; i < _players.Count; i++)
                {
                    //had to add this check in for otherwise sometime null reference occurs :s
                    if (_players[i].GetComponent<CharMovement>() == true &&
                        playerUIElements[i].GetComponentInChildren<Image>() == true)
                    {
                        //if that player has joined
                        if (_players[i].GetComponent<CharMovement>().alive &&
                            playerUIElements[i].GetComponentInChildren<Image>().color != Color.white)
                        {
                            //highlight UI
                            playerUIElements[i].GetComponentInChildren<Image>().color = Color.white;
                            creditsInt--;
                            menuCreditText.GetComponent<Text>().text = creditsString + "\n" + creditsInt;
                            gameCreditText.GetComponent<Text>().text = creditsString + "\n" + creditsInt;
                        }
                    }
                }

                //space == boot game
                if(Input.GetKey(KeyCode.Space))
                {
                    //set game state
                    m_state = GameStates.game;

                    //loop over all possible players
                    for (int i = 0; i < _players.Count; i++)
                    {
                        //if player hasn't joined
                        if (!_players[i].GetComponent<CharMovement>().alive)
                        {
                            //set instance inactive if not joined
                            _players[i].SetActive(false);
                        }
                    }
                }
                break;
            case GameStates.game:
                worldCanvas.SetActive(true);
                menuCanvas.SetActive(false);
                uiCanvas.SetActive(true);
                break;
            case GameStates.paused:
                break;
            default:
                break;
        }
		
	}
	

}
