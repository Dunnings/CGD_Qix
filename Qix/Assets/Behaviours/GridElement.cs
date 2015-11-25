/*
 * Grid Element describes a single grid element within the 
 * game world
 * 
 * */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class GridElement : MonoBehaviour 
{	
	void Awake () 
    {        
        //scale box collider
        GetComponent<BoxCollider2D>().size = new Vector2(1.0f, 1.0f);
        GetComponent<BoxCollider2D>().offset = new Vector2(0.0f, 0.0f);	
	}
}
