using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharMovement : MonoBehaviour
{
    public float moveSpeed = 1;
    // Use this for initialization
    List<Node> allTheNodes = new List<Node>();
    public GameObject nodeMarker;
    public bool validUp, validDown, validLeft, validRight;

    Node previousNode;

    List<Node> constructionPath = new List<Node>();

    public bool drawing = false;

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
        //If we're on a node, and we can move somewhere else
        //If a button is pressed to move in the diection of the existing line
        //Set the current line to the one we should be moving down


        //Check if current tile is node
        //for (int i = 0; i < allTheNodes.Count; i++)
        //{

        //    if (transform.position == allTheNodes[i].position)
        //    {
        //        hitNode(allTheNodes[i]);
        //    }
        //}
        if (Input.GetKey(KeyCode.Space))
        {
            drawing = true;
        }
        else if(drawing && previousNode.state == NodeState.active)
        {
            //Touched edge
            drawing = false;
            constructionPath.Add(previousNode);
            for (int i = 0; i < constructionPath.Count; i++)
            {
                constructionPath[i].state = NodeState.active;

                if(i > 0)
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
                
                WorldGenerator.Instance.PaintActive((int)constructionPath[i].position.x, (int)constructionPath[i].position.y);
            }
            int x1 = 0;
            int x2 = 0;
            int y1 = 0;
            int y2 = 0;
            bool fill = false;
            for (int i = 1; i < constructionPath.Count-1; i++)
            {
                if (constructionPath[constructionPath.Count - 1].directions[0])
                {
                    if (WorldGenerator.Instance.grid[(int)constructionPath[i].position.x - 1, (int)constructionPath[i].position.y].m_node.state == NodeState.active || WorldGenerator.Instance.grid[(int)constructionPath[i].position.x + 1, (int)constructionPath[i].position.y].m_node.state == NodeState.active)
                    {
                        continue;
                    }

                    //We need to go left and right of x-1 
                    x1 = (int)constructionPath[i].position.x - 1;
                    y1 = (int)constructionPath[i].position.y;
                    x2 = (int)constructionPath[i].position.x + 1;
                    y2 = (int)constructionPath[i].position.y;
                    fill = true;
                }
                else
                {
                    if (WorldGenerator.Instance.grid[(int)constructionPath[i].position.x, (int)constructionPath[i].position.y - 1].m_node.state == NodeState.active || WorldGenerator.Instance.grid[(int)constructionPath[i].position.x, (int)constructionPath[i].position.y + 1].m_node.state == NodeState.active)
                    {
                        continue;
                    }
                    //We need to go up and down of x-1
                    x1 = (int)constructionPath[i].position.x;
                    y1 = (int)constructionPath[i].position.y - 1;
                    x2 = (int)constructionPath[i].position.x;
                    y2 = (int)constructionPath[i].position.y + 1;
                    fill = true;
                }
            }
            if (fill)
            {
                StartCoroutine(WorldGenerator.Instance.PaintFill(x1, x2, y1, y2));
            }
            constructionPath.Clear();
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
            else if(drawing)
            {
                transform.Translate(0, 1 * moveSpeed, 0);
                validLeft = true;
                validRight = true;
                validUp = true;
                validDown = false;
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
            else if (drawing)
            {
                transform.Translate(0, -1 * moveSpeed, 0);
                validLeft = true;
                validRight = true;
                validDown = true;
                validUp = false;
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
            else if (drawing)
            {
                transform.Translate(-1 * moveSpeed, 0, 0);
                validLeft = true;
                validRight = false;
                validDown = true;
                validUp = true;
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
            else if (drawing)
            {
                transform.Translate(1 * moveSpeed, 0, 0);
                validLeft = true;
                validRight = true;
                validDown = true;
                validUp = false;
            }
        }

        if(drawing && constructionPath.Count == 0)
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
    }
}
