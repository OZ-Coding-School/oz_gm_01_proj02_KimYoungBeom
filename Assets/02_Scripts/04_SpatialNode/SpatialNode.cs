using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpatialNode : PoolableComponent, INode
{
    [Header("Node Shape")]
    [SerializeField] private ENodeShape _nodeShape;
    [SerializeField] private ENodeState _nodeState;

    [Header("Visuals")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private VisualEffect _vfxGraph;

    private NodeData _data;

    private MaterialPropertyBlock _propBlock;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public Vector3 WorldPosition => transform.position;
    public Vector2Int GridCoordinate => _data.gridCoord;
    public List<Vector2Int> MoveableDirections => _data.allowedDirs;
    public ENodeShape NodeShape => _nodeShape;
    public ENodeState NodeState => _nodeState;
    public System.Action OnStateChanged { get; set; }

    public void InjectData(NodeData data)
    {
        _data = data;
        _nodeState = data.nodeState;
        OnStateChanged?.Invoke();
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
    private void ResetVisuals()
    {
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

        _propBlock.SetFloat(DissolveAmount, 0f);
        _meshRenderer.SetPropertyBlock(_propBlock);

        _meshRenderer.enabled = true;
    }
    private IEnumerator FoldingRoutine(float duration)
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