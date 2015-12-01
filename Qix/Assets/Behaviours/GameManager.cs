using UnityEngine;
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
    public GameObject gameOverCanvas; 

    public List<GameObject> playerUIElements;
    public List<GameObject> playerIconUIElements;

    public GameObject menuCreditText;
    public GameObject gameCreditText;
    public GameObject winnerText;

    private string creditsString = "CREDITS";
    private int creditsInt = 64;
    public bool noController = false;

    //GameObject references 
    public List<GameObject> _players = new List<GameObject>();

    public AudioClip music;

    public double overAllFill = 0.0f;
    
	void Awake()
    {
        instance = this;
        //Get all Connected Game Controllers
        InputManager.GetControllers();

        //set up UI
        int length = InputManager._indexOfControllers.Count;

        //if there are connected devices start with controllers
        if (length > 0)
        {
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
        }
        //else use keyboard
        else
        {
            noController = true;
            //set up player icon in ui
            playerUIElements[0].SetActive(true);

            _players[0].SetActive(true);

            ////set up player and controller indexes
            _players[0].GetComponent<CharMovement>().playerIndex = 0;
            _players[0].GetComponent<CharMovement>().controllerIndex = 0;
        }
        worldCanvas.SetActive(true);
        menuCanvas.SetActive(true);
        uiCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        //set state to menu
        m_state = GameStates.menu;
	}
	
	void Update () 
    {
        switch (m_state)
        {
            case GameStates.menu:
                //set up UI
                //worldCanvas.SetActive(true);
                //menuCanvas.SetActive(true);
                //uiCanvas.SetActive(false);

                //in char select loop over max amount of active players
                for (int i = 0; i < _players.Count; i++)
                {
                    //had to add this check in for otherwise sometime null reference occurs :s
                    if (_players[i].GetComponent<CharMovement>() == true &&
                        playerUIElements[i].GetComponentInChildren<Image>() == true)
                    {
                        //if that player has joined
                        if (_players[i].GetComponent<CharMovement>().joined &&
                            playerUIElements[i].GetComponentInChildren<Image>().color != Color.white)
                        {
                            //get UI element color
                            Color get = playerUIElements[i].GetComponentInChildren<Image>().color;
                            //turn up alpha
                            get.a = 1.0f;
                            //set UI element to full alpha
                            playerUIElements[i].GetComponentInChildren<Image>().color = get;

                            creditsInt--;
                            menuCreditText.GetComponent<Text>().text = creditsString + "\n" + creditsInt;
                            gameCreditText.GetComponent<Text>().text = creditsString + "\n" + creditsInt;
                        }
                    }
                }

                //space == boot game and make sure at least one player 
                //has joined the game
                if(Input.GetKey(KeyCode.L)
                    && creditsInt< 64)
                {
                    worldCanvas.SetActive(true);
                    gameOverCanvas.SetActive(false);
                    menuCanvas.SetActive(false);
                    uiCanvas.SetActive(true);
                    //set game state
                    m_state = GameStates.game;

                    //start music
                    //AudioManager.instance.PlayMusic(music);

                    //loop over all possible players
                    for (int i = 0; i < _players.Count; i++)
                    {
                        if (_players[i].activeSelf)
                        {
                            //if player hasn't joined
                            if (!_players[i].GetComponent<CharMovement>().joined)
                            {
                                //set instance inactive if not joined
                                _players[i].SetActive(false);
                            }
                            else
                            {
                                playerIconUIElements[i].SetActive(true);
                            }
                        }
                    }
                }
                break;
            case GameStates.game:

                //when the overall fill has reached 90 or more
                if (overAllFill >= 10.0f)
                {
                    //set state
                    m_state = GameStates.gameOver;

                    //change UI
                    worldCanvas.SetActive(false);
                    gameOverCanvas.SetActive(true);
                    menuCanvas.SetActive(false);
                    uiCanvas.SetActive(false);

                    int winner = 0;
                    double lastScore = 0;
                    
                    foreach (GameObject player in _players)
                    {
                        if(player.GetComponent<CharMovement>().joined)
                        {
                            if(player.GetComponent<CharMovement>().score > lastScore)
                            {
                                lastScore = player.GetComponent<CharMovement>().score;
                                winner = player.GetComponent<CharMovement>().playerIndex;
                            }
                        }
                    }
                    winner++;

                    //set winner text
                    winnerText.GetComponent<Text>().text = "Player " + winner + " Wins!";

                    //for (int i = 0; i < playerUIElements.Count-1; i ++ )
                    //{
                    //    if (playerUIElements[i].activeSelf)
                    //    {
                    //        get UI element color
                    //        Color get = playerUIElements[i].GetComponentInChildren<Image>().color;
                    //        turn up alpha
                    //        get.a = 0.5f;
                    //        set UI element to full alpha
                    //        playerUIElements[i].GetComponentInChildren<Image>().color = get;
                    //    }
                    //}

                    

                }                
                break;
            case GameStates.paused:
                break;
            case GameStates.gameOver:

                if(Input.GetKey(KeyCode.L))
                {
                    worldCanvas.SetActive(true);
                    menuCanvas.SetActive(true);
                    uiCanvas.SetActive(false);
                    gameOverCanvas.SetActive(false);
                    
                    foreach (GameObject player in _players)
                    {
                        if(player.activeSelf)
                        {
                            player.GetComponent<CharMovement>().joined = false;
                        }
                    }

                }

                break;
            default:
                break;
        }
		
	}
}
