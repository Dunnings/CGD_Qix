/*
 * World generator spawns the game world baking sprites
 * into a single render texture
 * 
 * */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; 

//[ExecuteInEditMode]
public class WorldGenerator : MonoBehaviour 
{
    public static WorldGenerator Instance;

    public GameObject renderObject;
    public Texture2D blank;
    public Texture2D white;
    public Texture2D construction;
    public GridElement[,] grid = new GridElement[150,75];

    private RenderTexture renderTarget;
    private int mapWidth = 150;
    private int mapHeight = 75;

    public AudioClip completeSound;
    
	void Awake ()
    {
        Instance = this;
        InitialiseTextureUpdater();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GridElement gridElement = new GridElement();
                //gridElement.name = x + " - " + y;

                gridElement.m_pos = new Vector2(x, y);
                gridElement.m_node = new Node();
                gridElement.m_node.position = new Vector3(x, y, 0);
                if(x == 0 || x == mapWidth-1)
                {
                    gridElement.m_node.directions[0] = true;
                    gridElement.m_node.directions[2] = true;
                    gridElement.m_node.state = NodeState.active;
                }
                if (y == 1 || y == mapHeight-1)
                {
                    gridElement.m_node.directions[1] = true;
                    gridElement.m_node.directions[3] = true;
                    gridElement.m_node.state = NodeState.active;
                }
                if (x == 0 && y == 1)
                {
                    gridElement.m_node.directions[0] = true;
                    gridElement.m_node.directions[1] = true;
                    gridElement.m_node.directions[2] = false;
                    gridElement.m_node.directions[3] = false;
                }
                if (x == mapWidth - 1 && y == 1)
                {
                    gridElement.m_node.directions[0] = true;
                    gridElement.m_node.directions[1] = false;
                    gridElement.m_node.directions[2] = false;
                    gridElement.m_node.directions[3] = true;
                }
                if (x == 0 && y == mapHeight - 1)
                {
                    gridElement.m_node.directions[0] = false;
                    gridElement.m_node.directions[1] = true;
                    gridElement.m_node.directions[2] = true;
                    gridElement.m_node.directions[3] = false;
                }
                if (x == mapWidth - 1 && y == mapHeight - 1)
                {
                    gridElement.m_node.directions[0] = false;
                    gridElement.m_node.directions[1] = false;
                    gridElement.m_node.directions[2] = true;
                    gridElement.m_node.directions[3] = true;
                }
                grid[x, y] = gridElement;

                if (gridElement.m_node.state == NodeState.active)
                {

                    DrawTexture(x * 32, (mapHeight - y) * 32, white);
                }
                else
                {
                    DrawTexture(x * 32, (mapHeight - y) * 32, blank);
                }
            }            
        }
    }

    void Update()
    {
        //InputManager.SetUpPlayers();
    }

    public void InitialiseTextureUpdater()
    {
        renderTarget = new RenderTexture(mapWidth * 32, mapHeight * 32, 24, RenderTextureFormat.ARGB32);
        renderTarget.filterMode = FilterMode.Point;
        //renderTarget.name = "map";

        renderObject.GetComponent<RawImage>().texture = renderTarget;
    }

    public void DrawTexture(int x, int y, Texture2D texture)
    {
        Graphics.SetRenderTarget(renderTarget);
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, mapWidth * 32, mapHeight * 32, 0);
        //Graphics.DrawTexture(new Rect(x, y, Clear.width, Clear.height), Clear);
        Graphics.DrawTexture(new Rect(x, y, texture.width, texture.height), texture);
        GL.PopMatrix();
        Graphics.SetRenderTarget(null);
    }
    public void PaintConstruction(int x, int y)
    {
        DrawTexture(x * 32, (mapHeight - y) * 32, construction);
    }
    public void PaintActive(int x, int y)
    {
        DrawTexture(x * 32, (mapHeight - y) * 32, white);
        //AudioManager.instance.PlaySingle(completeSound);        
    }

    int totalTiles = 11250;

    public IEnumerator PaintFill(int x1, int y1, int x2, int y2)
    {
        yield return null;
        int count1 = UnsafeFloodFill3(x1, y1);
        if (count1 < totalTiles)
        {
            
            totalTiles -= UnsafeFloodFill3(x1, y1, true);
        }
        else
        {
            totalTiles -= UnsafeFloodFill3(x2, y2, true);
        }
    }

    int UnsafeFloodFill3(int x, int y, bool colour = false)
    {
        int returnAmount = 0;

        List<int> checkedValues = new List<int>();

        List<int> ptsx = new List<int>();
        ptsx.Add(x);
        List<int> ptsy = new List<int>();
        ptsy.Add(y);

        while (ptsx.Count > 0)
        {
            // check if x-1 is in bounds
            if (ptsx[0] - 1 > 0)
            {
                if (!checkedValues.Contains((ptsx[0] - 1 + ptsy[0]) * ptsy[0])){
                    //if grid element to the left is inactive
                    if (grid[ptsx[0] - 1, ptsy[0]].m_node.state == NodeState.inactive)
                    {
                        // add this element to the array
                        ptsx.Add(ptsx[0] - 1); ptsy.Add(ptsy[0]);
                        if (colour)
                        {
                            grid[ptsx[0] - 1, ptsy[0]].m_node.state = NodeState.active;
                            PaintActive(ptsx[0] - 1, ptsy[0]);
                        }
                        checkedValues.Add((ptsx[0] - 1 + ptsy[0]) * ptsy[0]);
                        returnAmount++;
                    }
                }
            }
            if (ptsy[0] - 1 >= 0) {
                if (!checkedValues.Contains((ptsx[0] + ptsy[0] - 1) * (ptsy[0] - 1))){
                    if (grid[ptsx[0], ptsy[0] - 1].m_node.state == NodeState.inactive)
                    {
                        ptsx.Add(ptsx[0]); ptsy.Add(ptsy[0] - 1);
                        if (colour)
                        {
                            grid[ptsx[0], ptsy[0] - 1].m_node.state = NodeState.active;
                            PaintActive(ptsx[0], ptsy[0] - 1);
                        }
                        checkedValues.Add((ptsx[0] + ptsy[0] - 1) * (ptsy[0] - 1));
                        returnAmount++;
                    }
                }
            }
            if (ptsx[0] + 1 <= mapWidth)
            {
                if (!checkedValues.Contains((ptsx[0] + 1 + ptsy[0]) * (ptsy[0])))
                {
                    if (grid[ptsx[0] + 1, ptsy[0]].m_node.state == NodeState.inactive)
                    {
                        ptsx.Add(ptsx[0] + 1); ptsy.Add(ptsy[0]);
                        if (colour)
                        {
                            grid[ptsx[0] + 1, ptsy[0]].m_node.state = NodeState.active;
                            PaintActive(ptsx[0] + 1, ptsy[0]);
                        }
                        checkedValues.Add((ptsx[0] + 1 + ptsy[0]) * (ptsy[0]));
                        returnAmount++;
                    }
                }
            }
            if (ptsy[0] + 1 <= mapHeight)
            {
                if (!checkedValues.Contains((ptsx[0] + ptsy[0] + 1) * (ptsy[0] + 1)))
                {
                    if (grid[ptsx[0], ptsy[0] + 1].m_node.state == NodeState.inactive)
                    {
                        ptsx.Add(ptsx[0]); ptsy.Add(ptsy[0] + 1);
                        if (colour)
                        {
                            grid[ptsx[0], ptsy[0] + 1].m_node.state = NodeState.active;
                            PaintActive(ptsx[0], ptsy[0] + 1);
                        }
                        checkedValues.Add((ptsx[0] + ptsy[0] + 1) * (ptsy[0] + 1));
                        returnAmount++;
                    }
                }
            }
            ptsx.RemoveAt(0);
            ptsy.RemoveAt(0);
        }

        return returnAmount;
    }

    //public int PaintAllInactiveNeighbours(int x, int y, bool paint = true, int direction = 0)
    //{
    //    int result = 0;
    //    //Direction 1 = up, 2 = right, 3 = down, 4 = left
    //    if (direction != 4)
    //    {
    //        result += PaintAllInactiveNeighbours(x - 1, y, paint, 2);
    //    }
    //    if (direction != 2)
    //    {
    //        result += PaintAllInactiveNeighbours(x + 1, y, paint, 4);
    //    }
    //    if (direction != 3)
    //    {
    //        result += PaintAllInactiveNeighbours(x, y - 1, paint, 1);
    //    }
    //    if (direction != 1)
    //    {
    //        result += PaintAllInactiveNeighbours(x, y + 1, paint, 3);
    //    }

    //    if (x < mapWidth && x >= 0 && y < mapHeight && y >= 0)
    //    {
    //        if (grid[x, y].m_node.state == NodeState.active)
    //        {
    //            if (paint)
    //            {
    //                PaintActive(x, y);
    //            }
    //            grid[x, y].m_node.state = NodeState.active;
    //            result++;
    //        }
    //    }
    //    Debug.Log(result);
    //    return result;
    //}
}
