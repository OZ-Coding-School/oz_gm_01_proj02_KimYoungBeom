using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpatialNode : PoolableComponent, INode
{
    [Header("Node Æ¯¼º")]
    [SerializeField] protected ENodeShape _nodeShape;
    [SerializeField] protected ENodeState _nodeState;
    [SerializeField] protected ENodeTrail _nodeTrail;


    [Header("Visuals")]
    [SerializeField] protected MeshRenderer _meshRenderer;
    [SerializeField] protected VisualEffect _vfxGraph;

    protected NodeData _data;

    protected MaterialPropertyBlock _propBlock;
    protected static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public Vector3Int WorldCoordinate { get; private set; }

    public Vector2Int GridCoordinate { get; private set; }
    public List<Vector2Int> MovableDirections => _data.allowedDirs;
    public ENodeShape NodeShape => _nodeShape;
    public ENodeState NodeState => _nodeState;
    public ENodeTrail NodeTrail => _nodeTrail;
    public Action OnStateChanged { get; set; }
    public event Action OnUpdateVisuals;

    private readonly Vector2Int[] _trailDir = new Vector2Int[2];

    public void InjectData(NodeData data)
    {
        _data = data;
        SetCoordinate(_data.WorldCoordinate);
        _nodeShape = _data.nodeShape;
        _nodeState = _data.nodeState;
        _nodeTrail = _data.nodeTrail;
        OnStateChanged?.Invoke();
        OnUpdateVisuals?.Invoke();

        MakeTrailDirection();
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
    public void ChangeNodeState(ENodeState changedState)
    {
        if (!CheckChangeState()) return;
        _nodeState = changedState;
    }
    private bool CheckChangeState()
    {
        if (NodeState == ENodeState.Key || NodeState == ENodeState.Finish || NodeState == ENodeState.Moving)
        {
            return false;
        }
        return true;
    }
    public Vector2Int[] GetTrailDirection()
    {
        return _trailDir;
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


    private void MakeTrailDirection()
    {
        Array.Clear(_trailDir, 0, _trailDir.Length);
        switch (NodeTrail)
        {
            case ENodeTrail.Vertical:
                _trailDir[0] = Vector2Int.up;
                _trailDir[1] = Vector2Int.down;
                break;
            case ENodeTrail.Horizontal:
                _trailDir[0] = Vector2Int.left;
                _trailDir[1] = Vector2Int.right;
                break;
            case ENodeTrail.UpRight:
                _trailDir[0] = Vector2Int.up;
                _trailDir[1] = Vector2Int.right;
                break;
            case ENodeTrail.UpLeft:
                _trailDir[0] = Vector2Int.up;
                _trailDir[1] = Vector2Int.left;
                break;
            case ENodeTrail.DownLeft:
                _trailDir[0] = Vector2Int.down;
                _trailDir[1] = Vector2Int.left;
                break;
            case ENodeTrail.DownRight:
                _trailDir[0] = Vector2Int.down;
                _trailDir[1] = Vector2Int.right;
                break;
            case ENodeTrail.Up:
                _trailDir[0] = Vector2Int.up;
                break;
            case ENodeTrail.Down:
                _trailDir[0] = Vector2Int.down;
                break;
            case ENodeTrail.Left:
                _trailDir[0] = Vector2Int.left;
                break;
            case ENodeTrail.Right:
                _trailDir[0] = Vector2Int.right;
                break;
            default:
                _trailDir[0] = Vector2Int.zero;
                break;
        }
    }
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }
}