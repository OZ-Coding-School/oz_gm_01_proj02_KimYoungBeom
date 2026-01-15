using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private PoolableObjSO _basicNodePoolData;
    [SerializeField] private PoolableObjSO _movingNodePoolData;
    [SerializeField] private PoolableObjSO _playerPoolData;
    [SerializeField] private PoolableObjSO _goalPoolData;

    private SpatialNode _startNode;
    private SpatialNode _finishNode;
    private PlayerController _player;

    private void Awake()
    {
        Managers.Stage.RegisterGenerator(this);
    }

    public void GenerateLevel(NodeGraphSO nodeGraph)
    {
        GenerateLevel(nodeGraph, true);
    }
    public void GenerateLevel(NodeGraphSO nodeGraph, bool doIntro)
    {
        if (nodeGraph == null) return;

        Managers.Pool.DespawnAll();

        _startNode = null;
        _finishNode = null;

        foreach (var nodeData in nodeGraph.Nodes)
        {
            SpatialNode node = SpawnSpatialNode(nodeData);
            node.InjectData(nodeData);

            Managers.Stage.SetNodeMap(node);

            SpawnPieces(node);

            if (node is IStageMovable moveNode) Managers.Stage.SetMovableList(moveNode);
        }

        StartStageIntroCameraMove(doIntro);
    }

    private async Awaitable ChangeViewAtReloadStage()
    {
        Managers.Camera.ChangeView(EViewMode.Lobby);
        await Awaitable.NextFrameAsync(destroyCancellationToken);
        Managers.Camera.ChangeView(EViewMode.Quarter);
    }
    private SpatialNode SpawnSpatialNode(NodeData nodeData)
    {
        SpatialNode node = null;
        switch (nodeData.nodeState)
        {
            case ENodeState.Moving:
                node = Managers.Pool.Spawn<MovingSpatialNode>(_movingNodePoolData, nodeData.worldCoord);
                break;
            default:
                node = Managers.Pool.Spawn<SpatialNode>(_basicNodePoolData, nodeData.worldCoord);
                break;
        }
        return node;
    }
    private void SpawnPieces(SpatialNode node)
    {
        switch (node.NodeState)
        {
            case ENodeState.Start:
                _startNode = node;
                _player = Managers.Pool.Spawn<PlayerController>(_playerPoolData, _startNode.WorldCoordinate);
                _player.Init(_startNode);
                break;
            case ENodeState.Finish:
                _finishNode = node;
                Managers.Pool.Spawn<Piece_Goal>(_goalPoolData, node.WorldCoordinate);
                break;
            case ENodeState.Key:



                break;
        }
    }

    private void StartStageIntroCameraMove(bool doIntro)
    {
        if (_startNode != null && _finishNode != null)
        {
            Managers.Camera.SetPlayerTarget(_player);
            if (doIntro)
            {
                float introTime = _startNode.WorldCoordinate.x - _finishNode.WorldCoordinate.x;
                _ = Managers.Camera.StartStageIntro(_startNode.WorldCoordinate, _finishNode.WorldCoordinate, introTime);
            }
            else
            {
                _ = ChangeViewAtReloadStage();
            }
        }
    }
}