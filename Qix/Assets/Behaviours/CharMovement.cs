using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System.Collections.Generic;

public class CharMovement : MonoBehaviour
{
    public float moveSpeed = 1;
    // Use this for initialization
    List<Node> allTheNodes = new List<Node>();
    public GameObject nodeMarker;
    public bool validUp, validDown, validLeft, validRight;
    GamePadState state;
    GamePadState prevState;
    public int playerIndex = 0;
    public int controllerIndex = 0;
    public bool alive = false;

    Node previousNode;

    List<Node> constructionPath = new List<Node>();

    public bool drawing = false;

    public AudioClip moveSound;

    void Start()
    {
        hitNode(WorldGenerator.Instance.grid[0, 0].m_node);
    }

    void hitNode(Node inputNode)
    {
        if (inputNode != previousNode)
        {
            previousNode = inputNode;
            if (inputNode.state == NodeState.inactive && drawing)
            {
                inputNode.state = NodeState.construction;
                WorldGenerator.Instance.PaintConstruction((int)inputNode.position.x, (int)inputNode.position.y);
                constructionPath.Add(inputNode);
                return;
            }
            //What are the connecting nodes
            validUp = inputNode.directions[0];
            validRight = inputNode.directions[1];
            validDown = inputNode.directions[2];
            validLeft = inputNode.directions[3];
        }
    }

    // Update is called once per frame
    void Update()
    {
        prevState = state;
        state = GamePad.GetState(InputManager.GetState(controllerIndex));

        switch (GameManager.instance.m_state)
        {            
            case GameStates.menu:
                if(!alive)
                alive = InputManager.SetUpPlayers(playerIndex, prevState, state);                
                break;
            case GameStates.game:
                //If we're on a node, and we can move somewhere else
                //If a button is pressed to move in the direction of the existing line
                //Set the current line to the one we should be moving down


                //Check if current tile is node
                //for (int i = 0; i < allTheNodes.Count; i++)
                //{

                //    if (transform.position == allTheNodes[i].position)
                //    {
                //        hitNode(allTheNodes[i]);
                //    }
                //}
                if (InputManager.ActionHeld(playerIndex, prevState, state) ||
                    Input.GetKey(KeyCode.Space))
                {
                    drawing = true;
                }
                else if (drawing && previousNode.state == NodeState.active)
                {
                    //Touched edge
                    drawing = false;
                    constructionPath.Add(previousNode);
                    for (int i = 0; i < constructionPath.Count; i++)
                    {
                        constructionPath[i].state = NodeState.active;

                        if (i > 0)
                        {
                            if (constructionPath[i - 1].position.x > constructionPath[i].position.x)
                            {
                                //Moved left
                                constructionPath[i - 1].directions[3] = true;
                                constructionPath[i].directions[1] = true;
                                Debug.Log(constructionPath[i].position.x + " " + constructionPath[i].position.y);
                            }
                            else if (constructionPath[i - 1].position.x < constructionPath[i].position.x)
                            {
                                //Moved right
                                constructionPath[i].directions[3] = true;
                                constructionPath[i - 1].directions[1] = true;
                            }
                            if (constructionPath[i - 1].position.y > constructionPath[i].position.y)
                            {
                                //Moved down
                                constructionPath[i - 1].directions[2] = true;
                                constructionPath[i].directions[0] = true;
                            }
                            else if (constructionPath[i - 1].position.y < constructionPath[i].position.y)
                            {
                                //Moved up
                                constructionPath[i].directions[2] = true;
                                constructionPath[i - 1].directions[0] = true;
                            }
                        }

                        WorldGenerator.Instance.PaintActive((int)constructionPath[i].position.x, (int)constructionPath[i].position.y);
                    }
                    constructionPath.Clear();
                }

                if (InputManager.UpHeld(playerIndex, prevState, state) ||
                    Input.GetKey(KeyCode.W))
                {
                    if (validUp)
                    {
                        //if vertical movement then allow movement
                        transform.Translate(0, 1 * moveSpeed, 0);
                        validLeft = false;
                        validRight = false;
                        validDown = true;
                    }
                    else if (drawing)
                    {
                        transform.Translate(0, 1 * moveSpeed, 0);
                        validLeft = true;
                        validRight = true;
                        validUp = true;
                        validDown = false;
                    }
                }
                if (InputManager.DownHeld(playerIndex, prevState, state) ||
                    Input.GetKey(KeyCode.S))
                {
                    if (validDown)
                    {
                        transform.Translate(0, -1 * moveSpeed, 0);
                        validLeft = false;
                        validRight = false;
                        validUp = true;
                    }
                    else if (drawing)
                    {
                        transform.Translate(0, -1 * moveSpeed, 0);
                        validLeft = true;
                        validRight = true;
                        validDown = true;
                        validUp = false;
                    }
                }
                if (InputManager.LeftHeld(playerIndex, prevState, state)
                    || Input.GetKey(KeyCode.A))
                {
                    if (validLeft)
                    {
                        transform.Translate(-1 * moveSpeed, 0, 0);
                        validUp = false;
                        validDown = false;
                        validRight = true;
                    }
                    else if (drawing)
                    {
                        transform.Translate(-1 * moveSpeed, 0, 0);
                        validLeft = true;
                        validRight = false;
                        validDown = true;
                        validUp = true;
                    }
                }
                if (InputManager.RightHeld(playerIndex, prevState, state)
                    || Input.GetKey(KeyCode.D))
                {
                    if (validRight)
                    {
                        transform.Translate(1 * moveSpeed, 0, 0);
                        validUp = false;
                        validDown = false;
                        validLeft = true;
                    }
                    else if (drawing)
                    {
                        transform.Translate(1 * moveSpeed, 0, 0);
                        validLeft = true;
                        validRight = true;
                        validDown = true;
                        validUp = false;
                    }
                }

                if (drawing && constructionPath.Count == 0)
                {
                    constructionPath.Add(previousNode);
                }

                if (Mathf.RoundToInt(transform.position.x + 0.5f) > previousNode.position.x)
                {
                    //Moved right one node
                    Node n = WorldGenerator.Instance.grid[(int)previousNode.position.x + 1, (int)previousNode.position.y].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.x + 0.5f) < previousNode.position.x)
                {
                    //Moved left one node
                    Node n = WorldGenerator.Instance.grid[(int)previousNode.position.x - 1, (int)previousNode.position.y].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.y + 0.5f) > previousNode.position.y)
                {
                    //Moved up one node
                    Node n = WorldGenerator.Instance.grid[(int)previousNode.position.x, (int)previousNode.position.y + 1].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.y + 0.5f) < previousNode.position.y)
                {
                    //Moved left one node
                    Node n = WorldGenerator.Instance.grid[(int)previousNode.position.x, (int)previousNode.position.y - 1].m_node;
                    hitNode(n);
                }
                break;
            case GameStates.paused:
                break;
            default:
                break;
        }
    }
       
}
