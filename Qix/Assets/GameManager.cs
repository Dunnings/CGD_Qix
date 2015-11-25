using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

// Created by David Dunnings

public class GameManager : MonoBehaviour {

	void Awake(){
		
	}

	void Start () {
        VectorLine.SetLine(Color.green, new Vector2(0, 0), new Vector2(Screen.width - 1, Screen.height - 1));
    }
	
	void Update () {
		
	}
	
	void OnDestroy(){
		
	}
}
