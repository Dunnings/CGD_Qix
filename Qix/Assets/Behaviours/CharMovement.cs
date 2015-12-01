using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XInputDotNetPure;
using System.Collections.Generic;
using System;

public class CharMovement : MonoBehaviour
{
    //Movement Speed
    public float moveSpeed = 1;
    //Construction Path
    List<Node> constructionPath = new List<Node>();
    List<Node> constructionPathCorners = new List<Node>();
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

    private Vector3 lastDir = Vector3.zero;
    public GameObject scoreUI;

    //Fuse
    public GameObject fuse;
    private float fuseWait = 1.0f;
    private float lastMoveTime = 0f;
    private int fusePathPosition = 0;
    private float fuseSpeed = 0.02f;
    private float fuseTimeLastHitNewNode = 0f;

    public double score = 0.0f;
	
	//Input enum
	public enum MoveInput {UP, DOWN, LEFT, RIGHT, NULL};
	//input stack contains the input(s) currently being held down
	//it works as a stack and updates once an input toggles (from held to released etc)
	List<MoveInput> inputStack = new List<MoveInput>();

	public enum GridLoc {UP, DOWN, LEFT, RIGHT, MID};

	public List<GridLoc> location = new List<GridLoc>();
    
	public MoveInput lastInput;

    Node currentNode;
    Node previousNode;

    public AudioClip fuseSound;

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
            previousNode = currentNode;
            //Current node is now this node
            currentNode = inputNode;
            //If current node state is inactive and we are constructing
            if (inputNode.state == NodeState.inactive && constructing)
            {
                //Set this node to constructing
                inputNode.state = NodeState.construction;
                inputNode.owner = playerIndex;
                //Paint this node
                WorldGenerator.Instance.PaintConstruction((int)inputNode.position.x, (int)inputNode.position.y, playerIndex);
                //Add this node to the construction path
                constructionPath.Add(inputNode);

				//GameObject node = new GameObject();
				//node.transform.position = new Vector3(inputNode.position.x, inputNode.position.y, 0f);
				//node.tag = "Node";
				
				//node.AddComponent<BoxCollider2D>();
				//node.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0.5f);
				//node.GetComponent<BoxCollider2D>().isTrigger = true;

				//node.AddComponent<Node>();
				//node.GetComponent<Node>().Equals(inputNode);


				//
            }
            else
            {
                //What are the connecting nodes
                validUp = inputNode.directions[0];
                validRight = inputNode.directions[1];
                validDown = inputNode.directions[2];
                validLeft = inputNode.directions[3];
            }
            //if (lastInput == MoveInput.UP || lastInput == MoveInput.DOWN)
            //{
            //    transform.position = new Vector3(transform.position.x - (transform.position.x % 0.5f), transform.position.y, 0f);
            //}
            //else if(lastInput == MoveInput.LEFT || lastInput == MoveInput.RIGHT)
            //{
            //    transform.position = new Vector3(transform.position.x, transform.position.y - (transform.position.y % 0.5f), 0f);
            //}
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
                constructionPathCorners.Clear();
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
                        if (i != 0 && i != constructionPath.Count - 1)
                        {
                            WorldGenerator.Instance.PaintActive((int)constructionPath[i].position.x, (int)constructionPath[i].position.y, playerIndex);
                        }

                    }

                    //CORNER GENERATION
                    constructionPathCorners.Add(constructionPath[0]);
                    for (int i = 2; i < constructionPath.Count; i++)
                    {
                        //A B C D
                        //C - D = direction
                        //C - B = prevDirection
                        Vector3 direction = (constructionPath[i - 1].position - constructionPath[i].position).normalized;
                        Vector3 prevDirection = (constructionPath[i - 2].position - constructionPath[i - 1].position).normalized;
                        if (prevDirection != direction)
                        {
                            constructionPathCorners.Add(constructionPath[i-1]);
                        }
                    }
                    constructionPathCorners.Add(constructionPath[constructionPath.Count - 1]);
                    //Find direction
                    var a = constructionPathCorners[0];
                    var b = constructionPathCorners[constructionPathCorners.Count - 1];
                    Vector3 dir = (a.position - b.position).normalized;

                    Vector3 up = new Vector3(0f, 1f, 0f);
                    Vector3 down = new Vector3(0f, -1f, 0f);
                    Vector3 left = new Vector3(1f, 0f, 0f);
                    Vector3 right = new Vector3(-1f, 0f, 0f);

                    if (dir == up)
                    {
                        constructionPathCorners[0].directions[2] = false;
                        constructionPathCorners[constructionPathCorners.Count - 1].directions[0] = false;
                    }
                    if (dir == down)
                    {
                        constructionPathCorners[0].directions[0] = false;
                        constructionPathCorners[constructionPathCorners.Count - 1].directions[2] = false;
                    }
                    if (dir == left)
                    {
                        constructionPathCorners[0].directions[3] = false;
                        constructionPathCorners[constructionPathCorners.Count - 1].directions[1] = false;
                    }
                    if (dir == right)
                    {
                        constructionPathCorners[0].directions[1] = false;
                        constructionPathCorners[constructionPathCorners.Count - 1].directions[3] = false;
                    }

                    //Going up or down
                    if ((int)constructionPath[1].position.y > (int)constructionPath[0].position.y ||
                        (int)constructionPath[1].position.y < (int)constructionPath[0].position.y)
                    {
                        int area = 0;

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
                        if(CanPathToQix(constructionPath[1].position.x + 1, constructionPath[1].position.y))
                        {
                            FloodFill((int)constructionPath[1].position.x + 1, (int)constructionPath[1].position.y, ref area);
                            UpdateScore(area);
                        }
                        else
                        {
                            FloodFill((int)constructionPath[1].position.x - 1, (int)constructionPath[1].position.y, ref area);
                            UpdateScore(area);
                        }
                    }


                    //Going left or right
                    else if ((int)constructionPath[1].position.x > (int)constructionPath[0].position.x ||
                        (int)constructionPath[1].position.x < (int)constructionPath[0].position.x)
                    {
                        int area = 0;

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
                        Debug.Log(constructionPath[0].position + " _ " + constructionPath[1].position);
                        if (CanPathToQix(constructionPath[1].position.x, constructionPath[1].position.y + 1))
                        {
                            FloodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y + 1, ref area);
                            UpdateScore(area);
                        }
                        else
                        {
                            FloodFill((int)constructionPath[1].position.x, (int)constructionPath[1].position.y - 1, ref area);
                            UpdateScore(area);
                        }
                    }
					
                    //clear the path 
                    constructionPath.Clear();
                    constructionPathCorners.Clear();
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
                    //AudioManager.instance.RandomizeSfx(fuseSound, fuseSound);
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
				if (validUp || (constructing && !validUp))
				{
                    try {
                        if (constructing && WorldGenerator.Instance.grid[(int)currentNode.position.x, (int)currentNode.position.y + 2].m_node.state == NodeState.construction)
                        {
                            break;
                        }
                    }
                    catch (IndexOutOfRangeException ex)
                    {

                    }

                    if(constructing && constructionPath.Count <= 1 && validUp)
                    {
                            break;
                    }

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
                

                if (validDown || (constructing && !validDown))
				{
                        try
                        {
                            if (constructing && WorldGenerator.Instance.grid[(int)currentNode.position.x, (int)currentNode.position.y - 2].m_node.state == NodeState.construction)
                            {
                                break;
                            }
                        }
                        catch (IndexOutOfRangeException ex)
                        {

                        }
                        if (constructing && constructionPath.Count <= 1 && validDown)
                        {
                            break;
                        }
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
				if (validLeft || (constructing && !validLeft))
				{
                        try {
                            if (constructing && WorldGenerator.Instance.grid[(int)currentNode.position.x - 2, (int)currentNode.position.y].m_node.state == NodeState.construction)
                            {
                                break;
                            }
                        }
                        catch(IndexOutOfRangeException ex)
                        {

                        }

                        if (constructing && constructionPath.Count <= 1 && validLeft)
                        {
                            break;
                        }
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
				if (validRight || (constructing && !validRight))
				{
                        try {
                            if (constructing && WorldGenerator.Instance.grid[(int)currentNode.position.x + 2, (int)currentNode.position.y].m_node.state == NodeState.construction)
                            {
                                Debug.Log((int)currentNode.position.x + 2 + "  _  " + (int)currentNode.position.y);
                                break;
                            }
                        }
                        catch (IndexOutOfRangeException ex)
                        {

                        }
                        if (constructing && constructionPath.Count <= 1 && validRight)
                        {
                            break;
                        }


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
    bool CanPathToQix(float x, float y)
    {
        Vector2 QixPos = new Vector2(75, 37);
        int count = 0;
        for (int i = 1; i < constructionPathCorners.Count; i++)
        {
            Debug.DrawLine(new Vector2(QixPos.x, QixPos.y), new Vector2(x, y), Color.blue, 60f);
            Debug.DrawLine(new Vector2(constructionPathCorners[i].position.x, constructionPathCorners[i].position.y), new Vector2(constructionPathCorners[i - 1].position.x, constructionPathCorners[i - 1].position.y), Color.blue, 60f);
            if (Intersect(new Point(QixPos.x, QixPos.y), new Point(x,y), new Point(constructionPathCorners[i].position.x, constructionPathCorners[i].position.y), new Point(constructionPathCorners[i-1].position.x, constructionPathCorners[i-1].position.y)))
            {
                count++;
            }
        }
        Debug.Log("Intersections: "+ count);
        return count % 2 != 0;
    }

    void FloodFill(int x, int y, ref int area)
    {
        if (x < 0 || x > 149 || y < 1 || y > 74)
        {
            return;
        }
        if ((WorldGenerator.Instance.grid[x, y].m_node.state == NodeState.active))
        {
            return;
        }
                
        WorldGenerator.Instance.grid[x, y].m_node.state = NodeState.active;

        WorldGenerator.Instance.PaintActive(x, y, playerIndex);
        area++;

        FloodFill(x + 1, y, ref area);  
        FloodFill(x - 1, y, ref area);
        FloodFill(x, y + 1, ref area);
        FloodFill(x, y - 1, ref area);   
    }

    //finds where the character is in the grid and then adds
    void SetLocation()
    {
        bool notEdgeX = false;

        location.Clear();

        //find where in the grid the player is
        if (currentNode.position.x == 0)
        {
            location.Add(GridLoc.LEFT);
        }
        else if (currentNode.position.x == 149)
        {
            location.Add(GridLoc.RIGHT);
        }
        else
        {
            notEdgeX = true;
        }

        if (currentNode.position.y == 1)
        {
            location.Add(GridLoc.DOWN);
        }
        else if (currentNode.position.y == 74)
        {
            location.Add(GridLoc.UP);
        }
        else
        {
            if (notEdgeX)
            {
                location.Add(GridLoc.MID);
            }
        }
    }

    /// <summary>
    /// called after an area is filled
    /// </summary>
    /// <param name="areaFilled"></param>
    void UpdateScore(int areaFilled)
    {
        //calc percentage
        double result = ((double)areaFilled / 11250) * 100;
        //round to 0 decimal places 
        result = Math.Round(result, 0);
        //increment overall score counter
        score += result;

        Debug.Log(score);
        //set UI component 
        scoreUI.GetComponent<Text>().text = score + "%";

        GameManager.instance.overAllFill += score;
    }

    //bool Intersection(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    //{
    //    Debug.DrawLine(new Vector3(x1, y1, 0f), new Vector3(x2, y2, 0f), Color.blue, 60f);
    //    Debug.DrawLine(new Vector3(x3, y3, 0f), new Vector3(x4, y4, 0f), Color.green, 60f);
    //    float d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
    //    if (d == 0) return false;
    //    float xi = ((x3 - x4) * (x1 * y2 - y1 * x2) - (x1 - x2) * (x3 * y4 - y3 * x4)) / d;
    //    float yi = ((y3 - y4) * (x1 * y2 - y1 * x2) - (y1 - y2) * (x3 * y4 - y3 * x4)) / d;
    //    if (x3 == x4)
    //    {
    //        if (yi < Mathf.Min(y1, y2) || yi > Mathf.Max(y1, y2)) return false;
    //    }
    //    Vector2 p = new Vector2(xi, yi);
    //    if (xi < Mathf.Min(x1, x2) || xi > Mathf.Max(x1, x2)) return false;
    //    if (xi < Mathf.Min(x3, x4) || xi > Mathf.Max(x3, x4)) return false;
    //    return true;
    //}

    int orientation(Point p, Point q, Point r)
    {
        int val = (q.y - p.y) * (r.x - q.x) -
                  (q.x - p.x) * (r.y - q.y);

        if (val == 0) return 0;  // colinear

        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

    bool Intersect(Point p1, Point q1, Point p2, Point q2)
    {
        // Find the four orientations needed for general and
        // special cases
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1
        if (o1 == 0 && onSegment(p1, p2, q1)) return true;

        // p1, q1 and p2 are colinear and q2 lies on segment p1q1
        if (o2 == 0 && onSegment(p1, q2, q1)) return true;

        // p2, q2 and p1 are colinear and p1 lies on segment p2q2
        if (o3 == 0 && onSegment(p2, p1, q2)) return true;

        // p2, q2 and q1 are colinear and q1 lies on segment p2q2
        if (o4 == 0 && onSegment(p2, q1, q2)) return true;

        return false; // Doesn't fall in any of the above cases
    }

    // Given three colinear points p, q, r, the function checks if
    // point q lies on line segment 'pr'
    bool onSegment(Point p, Point q, Point r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;

        return false;
    }

    class Point
    {
        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public Point(float _x, float _y)
        {
            x = (int)_x;
            y = (int)_y;
        }
        public int x;
        public int y;
    }
}
