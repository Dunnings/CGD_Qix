﻿using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System.Collections.Generic;

public class CharMovement : MonoBehaviour
{
    public float moveSpeed = 1;
    // Use this for initialization
	List<Node> allTheNodes = new List<Node>(), constructionPath = new List<Node>();
    public GameObject nodeMarker;
	public bool validUp, validDown, validLeft, validRight, alive = false, drawing = false;
	public int playerIndex = 0, controllerIndex = 0;
	GamePadState state, prevState;
    public List<KeyValuePair<int, int>> axis = new List<KeyValuePair<int, int>>();

    List<Node> CheckedNodes = new List<Node>();
	
	//Input enum, yum yum
	public enum MoveInput {UP, DOWN, LEFT, RIGHT, NULL};
	//input stack contains the input(s) currently being held down
	//it works as a stack and updates once an input toggles (from held to released etc)
	List<MoveInput> inputStack = new List<MoveInput>();

	public MoveInput lastInput;

    Node previousNode;

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
        if (!GameManager.instance.noController)
        {
            prevState = state;
            state = GamePad.GetState(InputManager.GetState(controllerIndex));
        }

        switch (GameManager.instance.m_state)
        {            
            case GameStates.menu:
                //if not alive and controllers are connected
                if (!alive && !GameManager.instance.noController)
                    alive = InputManager.SetUpPlayers(playerIndex, prevState, state);
                //else if not alive and keyboard
                else if (!alive && GameManager.instance.noController)
                    alive = Input.GetKey(KeyCode.Return);
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
                        //perform flood fill
                        
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

                    ////if the end point of the paths y is greater than the starting path pos
                    //if ((int)constructionPath[constructionPath.Count - 1].position.y > (int)constructionPath[0].position.y &&
                    //    (int)constructionPath[constructionPath.Count - 1].position.x == (int)constructionPath[0].position.x)
                    //{
                    //    //start filling from the 1 path pos up
                    //    floodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y + 1, 1);
                    //}
                    ////else if the end point of the paths y is less than the starting path pos
                    //else if ((int)constructionPath[constructionPath.Count - 1].position.y < (int)constructionPath[0].position.y &&
                    //    (int)constructionPath[constructionPath.Count - 1].position.x == (int)constructionPath[0].position.x)
                    //{
                    //    //start filling downwards
                    //    floodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y - 1, 1);
                    //}

                    ////x axis
                    //else if ((int)constructionPath[constructionPath.Count - 1].position.x > (int)constructionPath[0].position.x &&
                    //    (int)constructionPath[constructionPath.Count - 1].position.y == (int)constructionPath[0].position.y)
                    //{
                    //    floodFill((int)constructionPath[1].position.x+1, (int)constructionPath[1].position.y, 1);
                    //}
                    //else if ((int)constructionPath[constructionPath.Count - 1].position.x < (int)constructionPath[0].position.x &&
                    //    (int)constructionPath[constructionPath.Count - 1].position.y == (int)constructionPath[0].position.y)
                    //{
                    //    floodFill((int)constructionPath[1].position.x-1, (int)constructionPath[1].position.y, 1);
                    //}

                    
                    //else if ((int)constructionPath[constructionPath.Count - 1].position.y > (int)constructionPath[0].position.y &&
                    //    (int)constructionPath[constructionPath.Count - 1].position.x < (int)constructionPath[0].position.x)
                    //{
                    //    floodFill((int)constructionPath[1].position.x - 1, (int)constructionPath[1].position.y, 1);
                    //}
                    //else if ((int)constructionPath[constructionPath.Count - 1].position.y > (int)constructionPath[0].position.y &&
                    //(int)constructionPath[constructionPath.Count - 1].position.x > (int)constructionPath[0].position.x)
                    //{
                    //    floodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y + 1, 1);
                    //}


                    //else if ((int)constructionPath[constructionPath.Count - 1].position.y < (int)constructionPath[0].position.y &&
                    //   (int)constructionPath[constructionPath.Count - 1].position.x > (int)constructionPath[0].position.x)
                    //{
                    //    if ((int)constructionPath[constructionPath.Count - 2].x)
                    //    floodFill((int)constructionPath[1].position.x +1, (int)constructionPath[1].position.y-1, 1);
                    //}

                    //else if ((int)constructionPath[constructionPath.Count - 1].position.y > (int)constructionPath[0].position.y &&
                    //   (int)constructionPath[constructionPath.Count - 1].position.x > (int)constructionPath[0].position.x)
                    //{
                    //    floodFill((int)constructionPath[1].position.x -1, (int)constructionPath[1].position.y, 1);
                    //}

                    //else if ((int)constructionPath[constructionPath.Count - 1].position.y < (int)constructionPath[0].position.y &&
                    //  (int)constructionPath[constructionPath.Count - 1].position.x < (int)constructionPath[0].position.x)
                    //{
                    //    floodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y-1, 1);
                    //}

                    //Going up or down
                    if ((int)constructionPath[1].position.y > (int)constructionPath[0].position.y ||
                        (int)constructionPath[1].position.y < (int)constructionPath[0].position.y)
                    {
                        int areaR = 0;

                        //calculate the area to the left and to the right
                        calcFloodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y, ref areaR, 4);
                        CheckedNodes.Clear();

                        //if the area to the right is greater
                        if (areaR > (150*75)-areaR)
                        {
                            //flood fill to the left
                            FloodFill((int)constructionPath[1].position.x - 1, (int)constructionPath[1].position.y);
                        }
                        else
                        {
                            //else flood fill to the right
                            FloodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y);
                        }
                    }


                    //Going left or right
                    else if ((int)constructionPath[1].position.x > (int)constructionPath[0].position.x ||
                        (int)constructionPath[1].position.x < (int)constructionPath[0].position.x)
                    {
                        int areaU = 0;

                        //calculate the area to the left and to the right
                        calcFloodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y + 1, ref areaU, 4);
                        CheckedNodes.Clear();

                        //if the area to the right is greater
                        if (areaU > (150 * 75) - areaU)
                        {
                            //flood fill to the left
                            FloodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y - 1);
                        }
                        else
                        {
                            //else flood fill to the right
                            FloodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y + 1);
                        }
                    }
                                      
                    //clear the path 
                    constructionPath.Clear();
                }



                if (InputManager.UpHeld(playerIndex, prevState, state))
                {
					if (!inputStack.Contains(MoveInput.UP))
				    {
                		//add to stack
                		inputStack.Add(MoveInput.UP);
					}
                }
				else //if no longer being held, remove from the list
				{
					inputStack.Remove(MoveInput.UP);
				}

		
                if (InputManager.DownHeld(playerIndex, prevState, state))
                {
					if (!inputStack.Contains(MoveInput.DOWN))
					{
						//add to stack
						inputStack.Add(MoveInput.DOWN);
					}
				}
				else //if no longer being held, remove from the list
				{
					inputStack.Remove(MoveInput.DOWN);
				}
		
                if (InputManager.LeftHeld(playerIndex, prevState, state))
                {
					if (!inputStack.Contains(MoveInput.LEFT))
					{
						//add to stack
						inputStack.Add(MoveInput.LEFT);
					}
                }
				else //if no longer being held, remove from the list
				{
					inputStack.Remove(MoveInput.LEFT);
				}

                if (InputManager.RightHeld(playerIndex, prevState, state))
                {
					if (!inputStack.Contains(MoveInput.RIGHT))
					{
					//add to stack
					inputStack.Add(MoveInput.RIGHT);         
					}
                }
				else //if no longer being held, remove from the list
				{
					inputStack.Remove(MoveInput.RIGHT);
				}

				//apply the stack in order & only if valid
				ApplyMoveInput ();

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

	//loop through the list of inputs until a valid one is found
	//when the first valid movement is found, it is applied and then will not apply another movement
	void ApplyMoveInput ()
	{
		for (int i = 0; i < inputStack.Count; i++)
		{
			bool breakIt = false;

			switch (inputStack[i])
			{
			case MoveInput.UP:
				if (validUp)
				{
					//if vertical movement then allow movement
					transform.Translate(0, 1 * moveSpeed, 0);
					validLeft = false;
					validRight = false;
					validDown = true;

					if (drawing)
					{
						AmmendValidInputs ();
					}

					breakIt = true;
				}
				else if (drawing)
				{
					transform.Translate(0, 1 * moveSpeed, 0);
					validLeft = true;
					validRight = true;
					validUp = true;
					validDown = false;

					AmmendValidInputs ();

					lastInput = inputStack[i];
					breakIt = true;
				}
				break;
			case MoveInput.DOWN:
				if (validDown)
				{
					transform.Translate(0, -1 * moveSpeed, 0);
					validLeft = false;
					validRight = false;
					validUp = true;

					if (drawing)
					{
						AmmendValidInputs ();
					}

					breakIt = true;
				}
				else if (drawing)
				{
					transform.Translate(0, -1 * moveSpeed, 0);
					validLeft = true;
					validRight = true;
					validDown = true;
					validUp = false;

					AmmendValidInputs ();

					lastInput = inputStack[i];
					breakIt = true;
				}

				break;
			case MoveInput.LEFT:
				if (validLeft)
				{
					transform.Translate(-1 * moveSpeed, 0, 0);
					validUp = false;
					validDown = false;
					validRight = true;

					if (drawing)
					{
						AmmendValidInputs ();
					}

					breakIt = true;
				}
				else if (drawing)
				{
					transform.Translate(-1 * moveSpeed, 0, 0);
					validLeft = true;
					validRight = false;
					validDown = true;
					validUp = true;

					AmmendValidInputs ();

					lastInput = inputStack[i];
					breakIt = true;
				}

				break;
			case MoveInput.RIGHT:
				if (validRight)
				{
					transform.Translate(1 * moveSpeed, 0, 0);
					validUp = false;
					validDown = false;
					validLeft = true;

					if (drawing)
					{
						AmmendValidInputs ();
					}

					breakIt = true;
				}
				else if (drawing)
				{
					transform.Translate(1 * moveSpeed, 0, 0);

					validLeft = true;
					validRight = true;
					validDown = true;
					validUp = false;

					AmmendValidInputs ();

					lastInput = inputStack[i];
					breakIt = true;
				}          
				break;
			}

			if (breakIt)
			{
				break;
			}
		}
	}

	void AmmendValidInputs ()
	{
		//ammend valid inputs so that the player cannot go backwards
		switch (lastInput)
		{
		case MoveInput.UP:
			validDown = false;
			break;
		case MoveInput.DOWN:
			validUp = false;
			break;
		case MoveInput.LEFT:
			validRight = false;
			break;
		case MoveInput.RIGHT:
			validLeft = false;
			break;
		}
	}

    /// <summary>
    /// flood fill algorithm called after lines are completed
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="i"></param>
    void calcFloodFill(int x, int y, ref int area, int direction)
    {
        if(x < 1 || x > 148 || y < 1 || y > 74)
        {
            return;
        }
        if ((WorldGenerator.Instance.grid[x, y].m_node.state == NodeState.active))
        {
            return;            
        }

        if (CheckedNodes.Contains(WorldGenerator.Instance.grid[x, y].m_node))
        {
            return;
        }
        
        //axis.Add(new KeyValuePair<int, int>(x, y));
        //WorldGenerator.Instance.PaintActive(x,y);
        area++;

        CheckedNodes.Add(WorldGenerator.Instance.grid[x, y].m_node);

        //WorldGenerator.Instance.grid[x, y].m_node.state = NodeState.testing;


        //4 = none, 0=up,1=right,2=down,3=left
        if (direction != 3)
        {
            calcFloodFill(x + 1, y, ref area, 1);
        }
        if (direction != 1)
        {
            calcFloodFill(x - 1, y, ref area, 3);
        }
        if (direction != 2)
        {
            calcFloodFill(x, y + 1, ref area, 0);
        }
        if (direction != 0)
        {
            calcFloodFill(x, y - 1, ref area, 2);
        }
    }

    void FloodFill(int x, int y)
    {
        if (x < 1 || x > 148 || y < 1 || y > 74)
        {
            return;
        }
        if ((WorldGenerator.Instance.grid[x, y].m_node.state == NodeState.active))
        {
            return;
        }
                
        WorldGenerator.Instance.grid[x, y].m_node.state = NodeState.active;

        WorldGenerator.Instance.PaintActive(x, y);

        FloodFill(x + 1, y);
        FloodFill(x - 1, y);
        FloodFill(x, y + 1);
        FloodFill(x, y - 1);   
    }
}
