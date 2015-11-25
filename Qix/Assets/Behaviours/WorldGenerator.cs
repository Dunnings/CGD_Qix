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
    public GameObject renderObject;
    public Texture2D fill;
    public GameObject[,] grid = new GameObject[150,75];

    private RenderTexture renderTarget;
    private int mapWidth = 150;
    private int mapHeight = 75;

	void Start () 
    {
        InitialiseTextureUpdater();

        for (int x = 0; x <= mapWidth; x++)
        {
            for (int y = 0; y <= mapHeight; y++)
            {
                DrawTexture(x * 32, (mapHeight - y) * 32, fill);


                GameObject gridElement = new GameObject();
                gridElement.AddComponent<GridElement>();
                gridElement.name = x + " - " + y;

                gridElement.transform.position = new Vector2(x, y);
                gridElement.transform.parent = this.transform;
            }            
        }
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

}
