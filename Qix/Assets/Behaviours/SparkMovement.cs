using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System.Collections.Generic;

public class SparkMovement : MonoBehaviour
{
    public bool clockwise;
    public float moveSpeed = 0.1f;
    // Use this for initialization
    public bool validUp, validDown, validLeft, validRight, alive = false;
    
    //Input enum, yum yum
    public enum MoveInput { UP, DOWN, LEFT, RIGHT, NULL };
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
        switch (GameManager.instance.m_state)
        {
            case GameStates.menu:
                //do sum shit
                break;
            case GameStates.game:
                #region da game bit

                if (clockwise)
                {
                    inputStack.Add(MoveInput.UP);
                    inputStack.Add(MoveInput.RIGHT);
                    inputStack.Add(MoveInput.DOWN);
                    inputStack.Add(MoveInput.LEFT);
                }
                else
                {
                    inputStack.Add(MoveInput.LEFT);
                    inputStack.Add(MoveInput.DOWN);
                    inputStack.Add(MoveInput.RIGHT);
                    inputStack.Add(MoveInput.UP);
                }

                //apply the stack in order & only if valid
                ApplyMoveInput();

                #region have we moved nodes?
                if (Mathf.RoundToInt(transform.position.x + 0.5f) > previousNode.position.x)
                {
                    //Moved right one node
                    Debug.Log("We've moved right one node!");
                    Node n = WorldGenerator.Instance.grid[(int)previousNode.position.x + 1, (int)previousNode.position.y].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.x + 0.5f) < previousNode.position.x)
                {
                    //Moved left one node
                    Debug.Log("We've moved LEFT one node!");
                    Node n = WorldGenerator.Instance.grid[(int)previousNode.position.x - 1, (int)previousNode.position.y].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.y + 0.5f) > previousNode.position.y)
                {
                    //Moved up one node
                    Debug.Log("We've moved UP one node!");
                    Node n = WorldGenerator.Instance.grid[(int)previousNode.position.x, (int)previousNode.position.y + 1].m_node;
                    hitNode(n);
                }
                if (Mathf.RoundToInt(transform.position.y + 0.5f) < previousNode.position.y)
                {
                    //Moved down one node
                    Debug.Log("We've moved DOWN one node!");
                    Node n = WorldGenerator.Instance.grid[(int)previousNode.position.x, (int)previousNode.position.y - 1].m_node;
                    hitNode(n);
                }
                AmmendValidInputs();
                #endregion
                break;
                #endregion
            case GameStates.paused:
                break;
            default:
                break;
        }
    }

    //loop through the list of inputs until a valid one is found
    //when the first valid movement is found, it is applied and then will notapply another movement
    void ApplyMoveInput()
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
                        validDown = false;
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
                        validUp = false;
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
                        validRight = false;
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
                        validLeft = false;
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

    void AmmendValidInputs()
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
}