using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Created by David Dunnings

public enum NodeState
{
    active,
    construction,
    inactive,
    testing
}

public class Node
{
    public Vector3 position;
    //0 = up
    //1 = right
    //2 = down
    //3 = left
    public bool[] directions = new bool[4];
    public int owner = 0;
    public NodeState state = NodeState.inactive;
}
