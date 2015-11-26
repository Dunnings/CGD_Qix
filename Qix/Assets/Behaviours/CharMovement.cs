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

    Node previousNode;

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
        prevState = state;
        state = GamePad.GetState(InputManager.GetState(controllerIndex));

        if (Input.GetKeyDown(KeyCode.Space))
        {
        }
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

        if (InputManager.UpHeld(playerIndex, prevState, state))
        {            
            if (validUp)
            {
                //if vertical movement then allow movement
                transform.Translate(0, 1 * moveSpeed, 0);
                validLeft = false;
                validRight = false;
                validDown = true;
            }
        }


        if (InputManager.DownHeld(playerIndex, prevState, state))
        {
            if (validDown)
            {
                transform.Translate(0, -1 * moveSpeed, 0);
                validLeft = false;
                validRight = false;
                validUp = true;
            }
        }

        if (InputManager.LeftHeld(playerIndex, prevState, state))
        {
            if (validLeft)
            {
                transform.Translate(-1 * moveSpeed, 0, 0);
                validUp = false;
                validDown = false;
                validRight = true;
            }
        }
        if (InputManager.RightHeld(playerIndex, prevState, state))
        {
            if (validRight)
            {
                transform.Translate(1 * moveSpeed, 0, 0);
                validUp = false;
                validDown = false;
                validLeft = true;
            }
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
    }
}
