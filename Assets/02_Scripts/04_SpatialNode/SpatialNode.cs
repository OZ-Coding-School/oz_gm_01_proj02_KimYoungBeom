using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpatialNode : PoolableComponent, INode
{
    [Header("Node Shape")]
    [SerializeField] protected ENodeShape _nodeShape;
    [SerializeField] protected ENodeState _nodeState;

    [Header("Visuals")]
    [SerializeField] protected MeshRenderer _meshRenderer;
    [SerializeField] protected VisualEffect _vfxGraph;

    protected NodeData _data;

    protected MaterialPropertyBlock _propBlock;
    protected static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public Vector3Int WorldCoordinate { get; private set; }

    public Vector2Int GridCoordinate { get; private set; }
    public List<Vector2Int> MoveableDirections => _data.allowedDirs;
    public ENodeShape NodeShape => _nodeShape;
    public ENodeState NodeState => _nodeState;
    public Action OnStateChanged { get; set; }
    public event Action OnUpdateVisuals;
    public void InjectData(NodeData data)
    {
        _data = data;
        SetCoordinate(_data.WorldCoordinate);
        _nodeShape = _data.nodeShape;
        _nodeState = _data.nodeState;
        OnStateChanged?.Invoke();
        OnUpdateVisuals?.Invoke();
    }

    public override void OnSpawn()
    {
        ResetVisuals();
        if (_vfxGraph != null) _vfxGraph.Play();
    }
    public override void OnDespawn()
    {
        StopAllCoroutines();
    }
    public void ExecuteFolding(float duration)
    {
        StartCoroutine(FoldingRoutine(duration));
    }
    public void SetCoordinate(Vector2Int gridCoordinate)
    {
        GridCoordinate = gridCoordinate;
        WorldCoordinate = new Vector3Int(gridCoordinate.x, WorldCoordinate.y, gridCoordinate.y);
    }
    public void SetCoordinate(Vector3Int worldCoordinate)
    {
        WorldCoordinate = worldCoordinate;
        GridCoordinate = new Vector2Int(worldCoordinate.x, worldCoordinate.z);
    }
    protected void ResetVisuals()
    {
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

        _propBlock.SetFloat(DissolveAmount, 0f);
        _meshRenderer.SetPropertyBlock(_propBlock);

        _meshRenderer.enabled = true;
    }
    protected IEnumerator FoldingRoutine(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _propBlock.SetFloat(DissolveAmount, t);
            _meshRenderer.SetPropertyBlock(_propBlock);
            yield return null;
        }
    }

    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }
}