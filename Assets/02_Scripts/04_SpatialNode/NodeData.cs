using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeData : INode
{
    public Vector3 worldPos;
    public Vector2Int gridCoord;
    public List<Vector2Int> allowedDirs;

    public Vector3 WorldPosition => worldPos;
    public Vector2Int GridCoordinate => gridCoord;
    public List<Vector2Int> MoveableDirections => allowedDirs;

    public Action OnStateChanged { get; set; }
}