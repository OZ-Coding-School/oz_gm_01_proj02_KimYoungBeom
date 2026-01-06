using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpatialNode : PoolableComponent, INode
{
    [Header("Node Shape")]
    [SerializeField] private ENodeShape _nodeShape;

    [Header("Node Data")]
    [SerializeField] private NodeData _data;

    [Header("Visuals")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private VisualEffect _vfxGraph;

    private MaterialPropertyBlock _propBlock;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public Vector3 WorldPosition => transform.position;
    public Vector2Int GridCoordinate => _data.gridCoord;
    public List<Vector2Int> MoveableDirections => _data.allowedDirs;
    public ENodeShape NodeShape => _nodeShape;

    public System.Action OnStateChanged { get; set; }


    private void ResetVisuals()
    {
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

        _propBlock.SetFloat(DissolveAmount, 0f);
        _meshRenderer.SetPropertyBlock(_propBlock);

        _meshRenderer.enabled = true;
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

    public void SetupDirectionsByShape()
    {
        _data.allowedDirs = _nodeShape switch
        {
            ENodeShape.Cross => new() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right },
            ENodeShape.Horizontal => new() { Vector2Int.left, Vector2Int.right },
            ENodeShape.Vertical => new() { Vector2Int.up, Vector2Int.down },
            ENodeShape.UpRight => new() { Vector2Int.up, Vector2Int.right },
            ENodeShape.UpLeft => new() { Vector2Int.up, Vector2Int.left },
            ENodeShape.DownRight => new() { Vector2Int.down, Vector2Int.right },
            ENodeShape.DownLeft => new() { Vector2Int.down, Vector2Int.left },
            _ => new()
        };
    }

    public void SetGridCoordinate(Vector2Int coord) => _data.gridCoord = coord;
}