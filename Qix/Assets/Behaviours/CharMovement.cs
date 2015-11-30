using UnityEngine;
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
	
	//Input enum, yum yum
	public enum MoveInput {UP, DOWN, LEFT, RIGHT, NULL};
	//input stack contains the input(s) currently being held down
	//it works as a stack and updates once an input toggles (from held to released etc)
	List<MoveInput> inputStack = new List<MoveInput>();

	public enum GridLoc {UP, DOWN, LEFT, RIGHT, MID};

	public List<GridLoc> location = new List<GridLoc>();


	public MoveInput lastInput;

    Node currentNode;

    public AudioClip moveSound;

    void Start()
    {
        hitNode(WorldGenerator.Instance.grid[0, 0].m_node);

		SetLocation ();
    }

    void hitNode(Node inputNode)
    {
        if (inputNode != currentNode)
        {
            currentNode = inputNode;
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


                if (InputManager.ActionHeld(playerIndex, prevState, state) ||
                    Input.GetKey(KeyCode.Space))
                {
                    drawing = true;
                }

                else if (drawing && currentNode.state == NodeState.active)
                {
                    //Touched edge
                    drawing = false;
                    constructionPath.Add(currentNode);
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

                    //if the end point of the paths y is greater than the starting path pos
                    if ((int)constructionPath[constructionPath.Count - 1].position.y > (int)constructionPath[0].position.y)
                    {
                        //start filling from the 1 path pos up
                        floodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y + 1, 1);
                    }
                    //else if the end point of the paths y is less than the starting path pos
                    else if ((int)constructionPath[constructionPath.Count - 1].position.y < (int)constructionPath[0].position.y)
                    {
                        //start filling downwards
                        floodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y - 1, 1);
                    }

                    else if ((int)constructionPath[constructionPath.Count - 1].position.x > (int)constructionPath[0].position.x)
                    {
                        floodFill((int)constructionPath[1].position.x+1, (int)constructionPath[1].position.y, 1);
                    }
                    else if ((int)constructionPath[constructionPath.Count - 1].position.y < (int)constructionPath[0].position.x)
                    {
                        floodFill((int)constructionPath[1].position.x-1, (int)constructionPath[1].position.y, 1);
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
                    constructionPath.Add(currentNode);
                }

                if (Mathf.RoundToInt(transform.position.x + 0.5f) > currentNode.position.x)
                {
                    //Moved right one node
                    Node n = WorldGenerator.Instance.grid[(int)currentNode.position.x + 1, (int)currentNode.position.y].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.x + 0.5f) < currentNode.position.x)
                {
                    //Moved left one node
                    Node n = WorldGenerator.Instance.grid[(int)currentNode.position.x - 1, (int)currentNode.position.y].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.y + 0.5f) > currentNode.position.y)
                {
                    //Moved up one node
                    Node n = WorldGenerator.Instance.grid[(int)currentNode.position.x, (int)currentNode.position.y + 1].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.y + 0.5f) < currentNode.position.y)
                {
                    //Moved left one node
                    Node n = WorldGenerator.Instance.grid[(int)currentNode.position.x, (int)currentNode.position.y - 1].m_node;
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
	//when the first valid movement is found, it is applied and then will notapply another movement
	void ApplyMoveInput ()
	{
		SetLocation ();

		for (int i = 0; i < inputStack.Count; i++)
		{
			bool breakIt = false;

			switch (inputStack[i])
			{
			case MoveInput.UP:
				if (validUp || (drawing && location.Contains(GridLoc.DOWN)))
				{

					//if vertical movement then allow movement
					transform.Translate(0, 1 * moveSpeed, 0);
					validLeft = false;
					validRight = false;
					validDown = true;
					validUp = true;

					if (drawing)
					{
						validLeft = true;
						validRight = true;
						validDown = false;
						//AmmendValidInputs ();
					}

					breakIt = true;
				}
				break;
			case MoveInput.DOWN:
				if (validDown || (drawing && location.Contains(GridLoc.UP)))
				{
					transform.Translate(0, -1 * moveSpeed, 0);
					validLeft = false;
					validRight = false;
					validUp = true;
					validDown = true;

					if (drawing)
					{
						validLeft = true;
						validRight = true;
						validUp = false;
						//AmmendValidInputs ();
					}
					
					breakIt = true;
				}
				break;
			case MoveInput.LEFT:
				if (validLeft || (drawing && location.Contains(GridLoc.RIGHT)))
				{
					transform.Translate(-1 * moveSpeed, 0, 0);
					validUp = false;
					validDown = false;
					validRight = true;
					validLeft = true;

					if (drawing)
					{
						validUp = true;
						validRight = false;
						validDown = true;
						//AmmendValidInputs ();
					}
					
					breakIt = true;
				}
				break;
			case MoveInput.RIGHT:
				if (validRight || (drawing && location.Contains(GridLoc.LEFT)))
				{
					transform.Translate(1 * moveSpeed, 0, 0);
					validUp = false;
					validDown = false;
					validLeft = true;
					validRight = true;

					if (drawing)
					{
						validLeft = false;
						validUp = true;
						validDown = true;
						//AmmendValidInputs ();
					}
					
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
		//Ammend validity statements so that the player can leave the grid
		//if (location.Contains (GridLoc.UP) && drawing) 
	}

    /// <summary>
    /// flood fill algorithm called after lines are completed
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="i"></param>
    void floodFill(int x, int y, int i)
    {
        if(x < 0 || x > 149 || y < 0 || y > 74)
        {
            return;
        }
        if ((WorldGenerator.Instance.grid[x, y].m_node.state == NodeState.active))
        {
            return;            
        }
        
        
        //axis.Add(new KeyValuePair<int, int>(x, y));
        WorldGenerator.Instance.PaintActive(x,y);
        WorldGenerator.Instance.grid[x, y].m_node.state = NodeState.active;
        floodFill(x+1, y,i);
        floodFill(x-1, y,i);
        floodFill(x, y+1, i);
        floodFill(x, y-1, i);   
    }

	//finds where the character is in the grid and then adds
	void SetLocation ()
	{
		bool notEdgeX = false;

		location.Clear ();

		//find where in the grid the player is
		if (currentNode.position.x == 0) {
			location.Add (GridLoc.LEFT);
		} else if (currentNode.position.x == 149) {
			location.Add (GridLoc.RIGHT);
		} else {
			notEdgeX = true;
		}

		if (currentNode.position.y == 1) {
			location.Add (GridLoc.DOWN);
		} else if (currentNode.position.y == 74) {
			location.Add (GridLoc.UP);
		} else {
			if (notEdgeX)
			{
				location.Add(GridLoc.MID);
			}
		}
	}
}
