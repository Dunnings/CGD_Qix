using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System.Collections.Generic;

public class CharMovement : MonoBehaviour
{
    //Movement Speed
    public float moveSpeed = 1;
    //Construction Path
	List<Node> constructionPath = new List<Node>();
    //Can I move in a direction
    public bool validUp, validDown, validLeft, validRight;
    //Am I alive
    public bool alive = true;
    public bool joined = false;
    //Am I constructing a path
    public bool constructing = false;
    //Controller indexes
	public int playerIndex = 0, controllerIndex = 0;
    //States of gamepad
	GamePadState state, prevState;
    //Controller axes
    public List<KeyValuePair<int, int>> axis = new List<KeyValuePair<int, int>>();
    public List<Node> CheckedNodes = new List<Node>();

    //Fuse
    public GameObject fuse;
    private float fuseWait = 1.0f;
    private float lastMoveTime = 0f;
    private int fusePathPosition = 0;
    private float fuseSpeed = 0.02f;
    private float fuseTimeLastHitNewNode = 0f;
	
	//Input enum
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
        //Hit node start position
        HitNode(WorldGenerator.Instance.grid[0, 0].m_node);

        fuse.SetActive(false);

		SetLocation ();
    }

    void HitNode(Node inputNode)
    {
        //If this node is not the current node
        if (inputNode != currentNode)
        {
            //Current node is now this node
            currentNode = inputNode;
            //If current node state is inactive and we are constructing
            if (inputNode.state == NodeState.inactive && constructing)
            {
                //Set this node to constructing
                inputNode.state = NodeState.construction;
                //Paint this node
                WorldGenerator.Instance.PaintConstruction((int)inputNode.position.x, (int)inputNode.position.y);
                //Add this node to the construction path
                constructionPath.Add(inputNode);
            }
            else
            {
                //What are the connecting nodes
                validUp = inputNode.directions[0];
                validRight = inputNode.directions[1];
                validDown = inputNode.directions[2];
                validLeft = inputNode.directions[3];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!alive)
        {
            if(constructionPath.Count > 0)
            {
                for (int i = 0; i < constructionPath.Count; i++)
                {
                    if(constructionPath[i].state == NodeState.construction) {
                        constructionPath[i].state = NodeState.inactive;
                        WorldGenerator.Instance.PaintInactive((int)constructionPath[i].position.x, (int)constructionPath[i].position.y);
                    }
                }
                constructionPath.Clear();
                fusePathPosition = 0;
            }

            gameObject.SetActive(false);
            return;
        }


        if (!GameManager.instance.noController)
        {
            prevState = state;
            state = GamePad.GetState(InputManager.GetState(controllerIndex));
        }

        switch (GameManager.instance.m_state)
        {
            #region Menu
            case GameStates.menu:
                //if not alive and controllers are connected
                if (!joined && !GameManager.instance.noController)
                    joined = InputManager.SetUpPlayers(playerIndex, prevState, state);
                //else if not spawned and keyboard
                else if (!joined && GameManager.instance.noController)
                    joined = Input.GetKey(KeyCode.Return);
                break;
            #endregion
            #region Game
            case GameStates.game:
                //If we're on a node, and we can move somewhere else
                //If a button is pressed to move in the direction of the existing line
                //Set the current line to the one we should be moving down
                if (InputManager.ActionHeld(playerIndex, prevState, state) ||
                    Input.GetKey(KeyCode.Space))
                {
                    constructing = true;
                }

                else if (constructing && currentNode.state == NodeState.active)
                {
                    //Touched edge
                    constructing = false;
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
                        //calcFloodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y, ref areaR, 4);
                        //CheckedNodes.Clear();

                        ////if the area to the right is greater
                        //if (areaR > (150*75)-areaR)
                        //{
                        //    //flood fill to the left
                        //    FloodFill((int)constructionPath[1].position.x - 1, (int)constructionPath[1].position.y);
                        //}
                        //else
                        //{
                        //    //else flood fill to the right
                        //    FloodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y);
                        //}
                        if(CanPathToQix((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y))
                        {
                            FloodFill((int)constructionPath[1].position.x - 1, (int)constructionPath[1].position.y);
                        }
                        else
                        {
                            FloodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y);
                        }
                    }


                    //Going left or right
                    else if ((int)constructionPath[1].position.x > (int)constructionPath[0].position.x ||
                        (int)constructionPath[1].position.x < (int)constructionPath[0].position.x)
                    {
                        int areaU = 0;

                        ////calculate the area to the left and to the right
                        //calcFloodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y + 1, ref areaU, 4);
                        //CheckedNodes.Clear();

                        ////if the area to the right is greater
                        //if (areaU > (150 * 75) - areaU)
                        //{
                        //    //flood fill to the left
                        //    FloodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y - 1);
                        //}
                        //else
                        //{
                        //    //else flood fill to the right
                        //    FloodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y + 1);
                        //}

                        if (CanPathToQix((int)constructionPath[1].position.x, (int)constructionPath[1].position.y + 1))
                        {
                            FloodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y - 1);
                        }
                        else
                        {
                            FloodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y - 1);
                        }
                    }
                                      
                    //clear the path 
                    constructionPath.Clear();
                    fusePathPosition = 0;
                }



                if (InputManager.UpHeld(playerIndex, prevState, state))
                {
                    lastMoveTime = Time.time;
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
                    lastMoveTime = Time.time;
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
                    lastMoveTime = Time.time;
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
                    lastMoveTime = Time.time;
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

                if (constructing && constructionPath.Count == 0)
                {
                    constructionPath.Add(currentNode);
                }

                if (Mathf.RoundToInt(transform.position.x + 0.5f) > currentNode.position.x)
                {
                    //Moved right one node
                    Node n = WorldGenerator.Instance.grid[(int)currentNode.position.x + 1, (int)currentNode.position.y].m_node;
                    HitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.x + 0.5f) < currentNode.position.x)
                {
                    //Moved left one node
                    Node n = WorldGenerator.Instance.grid[(int)currentNode.position.x - 1, (int)currentNode.position.y].m_node;
                    HitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.y + 0.5f) > currentNode.position.y)
                {
                    //Moved up one node
                    Node n = WorldGenerator.Instance.grid[(int)currentNode.position.x, (int)currentNode.position.y + 1].m_node;
                    HitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.y + 0.5f) < currentNode.position.y)
                {
                    //Moved left one node
                    Node n = WorldGenerator.Instance.grid[(int)currentNode.position.x, (int)currentNode.position.y - 1].m_node;
                    HitNode(n);
                }

                #region Fuse
                if (Time.time - lastMoveTime > fuseWait && constructionPath.Count > 1)
                {
                    if (!fuse.activeSelf)
                    {
                        fuse.SetActive(true);
                    }
                    fuse.transform.position = Vector3.Lerp(constructionPath[fusePathPosition].position, constructionPath[fusePathPosition + 1].position, Time.time - fuseTimeLastHitNewNode / fuseSpeed);
                    if (Time.time - fuseTimeLastHitNewNode > fuseSpeed)
                    {
                        if (fusePathPosition + 2 >= constructionPath.Count)
                        {
                            alive = false;
                            fuse.SetActive(false);
                            return;
                        }
                        WorldGenerator.Instance.PaintBurnt((int)constructionPath[fusePathPosition + 1].position.x, (int)constructionPath[fusePathPosition + 1].position.y);
                        fuseTimeLastHitNewNode = Time.time;
                        fusePathPosition++;
                    }
                }
                else
                {
                    if (fuse.activeSelf)
                    {
                        fuse.SetActive(false);
                    }
                }
                #endregion

                break;
            #endregion
            #region default
            default:
                break;
                #endregion
        }

        
    }

	//loop through the list of inputs until a valid one is found
	//when the first valid movement is found, it is applied and then will not apply another movement
	void ApplyMoveInput ()
	{
		SetLocation ();

		for (int i = 0; i < inputStack.Count; i++)
		{
			bool breakIt = false;

			switch (inputStack[i])
			{
			case MoveInput.UP:
				if (validUp || (constructing && location.Contains(GridLoc.DOWN)))
				{

					//if vertical movement then allow movement
					transform.Translate(0, 1 * moveSpeed, 0);
					validLeft = false;
					validRight = false;
					validDown = true;
					validUp = true;

					if (constructing)
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
				if (validDown || (constructing && location.Contains(GridLoc.UP)))
				{
					transform.Translate(0, -1 * moveSpeed, 0);
					validLeft = false;
					validRight = false;
					validUp = true;
					validDown = true;

					if (constructing)
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
				if (validLeft || (constructing && location.Contains(GridLoc.RIGHT)))
				{
					transform.Translate(-1 * moveSpeed, 0, 0);
					validUp = false;
					validDown = false;
					validRight = true;
					validLeft = true;

					if (constructing)
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
				if (validRight || (constructing && location.Contains(GridLoc.LEFT)))
				{
					transform.Translate(1 * moveSpeed, 0, 0);
					validUp = false;
					validDown = false;
					validLeft = true;
					validRight = true;

					if (constructing)
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

    bool CanPathToQix(int x, int y)
    {
        Vector2 QixPos = new Vector2(75, 30);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(x, y), QixPos - new Vector2(x, y));
        Debug.DrawLine(new Vector3(x, y, 10f), new Vector3(QixPos.x, QixPos.y, 10f), Color.blue);
        int count = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            if (constructionPath.Contains(hits[i].collider.transform.GetComponent<GridElement>().m_node))
            {
                count++;
            }
        }
        return count % 2 != 0;
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
