using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Created by David Dunnings
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

    //GameObject references 
    public GameObject Player;
    List<GameObject> _players = new List<GameObject>();
    
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
            //create a new player
            _players.Add(GameObject.Instantiate(Player, new Vector2(0, 0), Quaternion.identity) as GameObject);          
               
            //if i is div by 2 then next controller
            if(i % 2 == 0 && i != 0)
            {
                controlIndex++;
            }

            //set up player and controller indexes
            _players[i].GetComponent<CharMovement>().playerIndex = i;
            _players[i].GetComponent<CharMovement>().controllerIndex = controlIndex;
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
                    //if that player has joined
                    if(_players[i].GetComponent<CharMovement>().alive)
                    {
                        //highlight UI
                        playerUIElements[i].GetComponent<Image>().color = Color.white;
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
                        //if player hasnt joined
                        if (!_players[i].GetComponent<CharMovement>().alive)
                        {
                            //destroy instance
                            GameObject.Destroy(_players[i]);
                        }
                    }

                }


                break;
            case GameStates.game:
                worldCanvas.SetActive(true);
                menuCanvas.SetActive(false);
                uiCanvas.SetActive(false);
                break;
            case GameStates.paused:
                break;
            default:
                break;
        }
		
	}
	

}
