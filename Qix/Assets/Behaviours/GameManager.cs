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
    public GameObject overallPrecentText;

    private string creditsString = "CREDITS";
    private int creditsInt = 64;
    public int spawnedPlayers = 0;
    private int joinedPlayers = 0;
    public bool noController = false;

    //GameObject references 
    public List<GameObject> _players = new List<GameObject>();

    public AudioClip music;
    public AudioClip playerJoined;
 

    public double overAllFill = 0.0f;
    
    void SetUpPlayers()
    {
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
    }

	void Awake()
    {
        instance = this;
        //Get all Connected Game Controllers
        InputManager.GetControllers();

        SetUpPlayers();
        
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
                            AudioManager.instance.PlaySingle(playerJoined);
                            //get UI element color
                            Color get = playerUIElements[i].GetComponentInChildren<Image>().color;
                            //turn up alpha
                            get.a = 1.0f;
                            //set UI element to full alpha
                            playerUIElements[i].GetComponentInChildren<Image>().color = get;

                            creditsInt--;
                            menuCreditText.GetComponent<Text>().text = creditsString + "\n" + creditsInt;
                            gameCreditText.GetComponent<Text>().text = creditsString + "\n" + creditsInt;

                            ++spawnedPlayers;
                            ++joinedPlayers;
                        }
                    }
                }

                //space == boot game and make sure at least one player 
                //has joined the game
                if (Input.GetKeyUp(KeyCode.L)
                    && creditsInt< 64)
                {
                    worldCanvas.SetActive(true);
                    gameOverCanvas.SetActive(false);
                    menuCanvas.SetActive(false);
                    uiCanvas.SetActive(true);
                    //set game state
                    m_state = GameStates.game;

                    //start music
                    AudioManager.instance.PlayMusic(music);

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

                double score = 0.0f;

            

                overallPrecentText.GetComponent<Text>().text = "%" + overAllFill;


                //when the overall fill has reached 90 or more
                if (overAllFill >= 70.0f)
                {
                    //set state
                    m_state = GameStates.gameOver;                 

                    int winner = 0;
                    double lastScore = 0;
                    //loop through players and find highest score
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

                    //change UI
                    worldCanvas.SetActive(false);
                    gameOverCanvas.SetActive(true);
                    menuCanvas.SetActive(false);
                    uiCanvas.SetActive(false);

                }
                
                if(spawnedPlayers == 0)
                {
                    winnerText.GetComponent<Text>().text = "GAME OVER";

                    //change UI
                    worldCanvas.SetActive(false);
                    gameOverCanvas.SetActive(true);
                    menuCanvas.SetActive(false);
                    uiCanvas.SetActive(false);

                    m_state = GameStates.gameOver;
                }
                if (joinedPlayers > 1)
                {
                    if (joinedPlayers - spawnedPlayers == joinedPlayers - 1)
                    {
                        int index = 0;

                        foreach (GameObject player in _players)
                        {
                            if (player.activeSelf)
                            {
                                index = player.GetComponent<CharMovement>().playerIndex + 1;
                                winnerText.GetComponent<Text>().text = "Player " + index + " Wins!";
                            }
                        }
                        //change UI
                        worldCanvas.SetActive(false);
                        gameOverCanvas.SetActive(true);
                        menuCanvas.SetActive(false);
                        uiCanvas.SetActive(false);

                        m_state = GameStates.gameOver;

                    }
                }
                break;
            case GameStates.paused:
                break;
            case GameStates.gameOver:
                spawnedPlayers = 0;
                joinedPlayers = 0;
                overAllFill = 0;
                if(Input.GetKeyUp(KeyCode.L))
                {
                    Application.LoadLevel("Main");

                    //worldCanvas.SetActive(false);
                    //menuCanvas.SetActive(true);
                    //uiCanvas.SetActive(false);
                    //gameOverCanvas.SetActive(false);


                    //SetUpPlayers();
                    //for (int i = 0; i < playerUIElements.Count - 1; i++)
                    //{
                    //    //turn all joined player c
                    //    if (playerUIElements[i].activeSelf)
                    //    {
                    //        //get UI element color
                    //        Color get = playerUIElements[i].GetComponentInChildren<Image>().color;
                    //        //turn up alpha
                    //        get.a = 0.5f;
                    //        //set UI element to full alpha
                    //        playerUIElements[i].GetComponentInChildren<Image>().color = get;
                    //    }
                    //}
                    //foreach (GameObject player in _players)
                    //{
                    //    if (player.GetComponent<CharMovement>().joined)
                    //    {
                    //        player.GetComponent<CharMovement>().alive = true;
                    //        player.GetComponent<CharMovement>().score = 0;
                    //        player.transform.position = new Vector2(0.0f, 0.0f);
                    //        player.GetComponent<CharMovement>().joined = false;
                    //        player.GetComponent<CharMovement>().constructionPath.Clear();
                    //        player.GetComponent<CharMovement>().constructionPathCorners.Clear();
                    //    }

                    //    //player.SetActive(false);
                    //}


                    //creditsInt = 64;
                    //menuCreditText.GetComponent<Text>().text = creditsString + "\n" + creditsInt;
                    //gameCreditText.GetComponent<Text>().text = creditsString + "\n" + creditsInt;
                    
                    //WorldGenerator.Instance.Reset();
                    //m_state = GameStates.menu;

                }

                break;
            default:
                break;
        }
		
	}
}
