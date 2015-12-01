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
    public List<Texture2D> playerFills;
    public List<Texture2D> playerConstructionFills;
    public Texture2D construction;
    public Texture2D burntTile;
    public GridElement[,] grid = new GridElement[150,75];

    private RenderTexture renderTarget;
    private int mapWidth = 150;
    private int mapHeight = 75;

    public AudioClip completeSound;
    
	void Awake ()
    {
        Instance = this;
        InitialiseTextureUpdater();

        GameObject parent = new GameObject();
        parent.name = "parent";

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameObject element = new GameObject();
                element.name = x + " " + y;
                element.transform.position = new Vector2(x, y);
                GridElement gridElement = element.AddComponent<GridElement>();
                element.transform.parent = parent.transform;

                element.AddComponent<BoxCollider2D>();
                element.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0.5f);

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
                if (y == 0 || y == mapHeight-1)
                {
                    gridElement.m_node.directions[1] = true;
                    gridElement.m_node.directions[3] = true;
                    gridElement.m_node.state = NodeState.active;
                }
                if (x == 0 && y == 0)
                {
                    gridElement.m_node.directions[0] = true;
                    gridElement.m_node.directions[1] = true;
                    gridElement.m_node.directions[2] = false;
                    gridElement.m_node.directions[3] = false;
                }
                if (x == mapWidth - 1 && y == 0)
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

            }            
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (grid[x, y].m_node.state == NodeState.active)
                {
                    DrawTexture(x * 32, (mapHeight - y-1) * 32, white);
                }
                else
                {
                    DrawTexture(x * 32, (mapHeight - y-1) * 32, blank);
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
    public void PaintConstruction(int x, int y, int player)
    {
        DrawTexture(x * 32 , (mapHeight - y) * 32 , playerConstructionFills[player]);
    }
    public void PaintActive(int x, int y, int player)
    {
        DrawTexture(x * 32 , (mapHeight - y) * 32 , playerFills[player]);
        //AudioManager.instance.PlaySingle(completeSound);        
    }
    public void PaintBurnt(int x, int y)
    {
        DrawTexture(x * 32 , (mapHeight - y) * 32 , burntTile);
        //AudioManager.instance.PlaySingle(completeSound);        
    }

    public void PaintInactive(int x, int y)
    {
        DrawTexture(x * 32 , (mapHeight - y) * 32 , blank);
        //AudioManager.instance.PlaySingle(completeSound);        
    }

    int totalTiles = 11250;
    
}
