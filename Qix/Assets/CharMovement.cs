using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharMovement : MonoBehaviour
{

    public float moveSpeed = 1;
    // Use this for initialization
    List<node> allTheNodes = new List<node>();
    public GameObject nodeMarker;
    bool vertical;
    bool validUp, validDown, validLeft, validRight;

    node previousNode;

    void Start()
    {
        nodeMarker = Resources.Load("Knob") as GameObject;
        node nodeA = new node();
        nodeA.position = new Vector2(0, 0);
        nodeA.directions[0] = true;
        nodeA.directions[3] = true;
        GameObject markerA = Instantiate(nodeMarker) as GameObject;
        markerA.transform.position = nodeA.position;

        node nodeB = new node();
        nodeB.position = new Vector2(-7.5f, 0);
        nodeB.directions[0] = true;
        nodeB.directions[1] = true;
        GameObject markerB = Instantiate(nodeMarker) as GameObject;
        markerB.transform.position = nodeB.position;

        node nodeC = new node();
        nodeC.position = new Vector2(-7.5f, 7.5f);
        nodeC.directions[2] = true;
        nodeC.directions[1] = true;
        GameObject markerC = Instantiate(nodeMarker) as GameObject;
        markerC.transform.position = nodeC.position;

        node nodeD = new node();
        nodeD.position = new Vector3(0, 7.5f, 0);
        nodeD.directions[1] = true;
        nodeD.directions[2] = true;
        nodeD.directions[3] = true;
        GameObject markerD = Instantiate(nodeMarker) as GameObject;
        markerD.transform.position = nodeD.position;

        node nodeE = new node();
        nodeE.position = new Vector3(7.5f, 7.5f, 0);
        nodeE.directions[3] = true;
        GameObject markerE = Instantiate(nodeMarker) as GameObject;
        markerE.transform.position = nodeE.position;

        previousNode = nodeA;

        allTheNodes.Add(nodeA);
        allTheNodes.Add(nodeB);
        allTheNodes.Add(nodeC);
        allTheNodes.Add(nodeD);
        allTheNodes.Add(nodeE);

    }

    public class node
    {
        public Vector3 position;
        //0 = up
        //1 = right
        //2 = down
        //3 = left
        public bool[] directions = new bool[4];
    }

    void hitNode(node inputNode)
    {
        //What are the connecting nodes
        validUp = inputNode.directions[0];
        validRight = inputNode.directions[1];
        validDown = inputNode.directions[2];
        validLeft = inputNode.directions[3];

    }

    // Update is called once per frame
    void Update()
    {

        //If we're on a node, and we can move somewhere else
        //If a button is pressed to move in the diection of the existing line
        //Set the current line to the one we should be moving down


        //Check if current tile is node
        for (int i = 0; i < allTheNodes.Count; i++)
        {
            
            if (transform.position == allTheNodes[i].position)
            {
                hitNode(allTheNodes[i]);
            }
        }

        if (Input.GetKey(KeyCode.W))
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
        if (Input.GetKey(KeyCode.S))
        {
            if (validDown)
            { 
                transform.Translate(0, -1 * moveSpeed, 0);
                validLeft = false;
                validRight = false;
                validUp = true;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (validLeft)
            {
                transform.Translate(-1 * moveSpeed, 0, 0);
                validUp = false;
                validDown = false;
                validRight = true;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (validRight)
            {
                transform.Translate(1 * moveSpeed, 0, 0);
                validUp = false;
                validDown = false;
                validLeft = true;
            }
        }
    }
}
