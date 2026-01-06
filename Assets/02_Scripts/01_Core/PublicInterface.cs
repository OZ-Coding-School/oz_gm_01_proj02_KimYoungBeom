using System;
using System.Collections.Generic;
using UnityEngine;

public class PublicInterface { }

public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}
public interface IPoolContainer
{
    void DespawnAll();
    void Clear();
    PoolableComponent GetBase();
    void ReleaseBase(PoolableComponent obj);
}
public interface INode
{
    Vector3 WorldPosition { get; }
    Vector2Int GridCoordinate { get; }
    List<Vector2Int> MoveableDirections { get; }
    Action OnStateChanged { get; set; }
}