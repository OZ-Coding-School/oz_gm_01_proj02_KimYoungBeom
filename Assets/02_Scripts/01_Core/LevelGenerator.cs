using System.Collections.Generic;
using Unity.Behavior;
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

    private Piece_Goal _goal;

    private void Awake()
    {
        Managers.Stage.RegisterGenerator(this);
    }
    private void OnEnable()
    {
        Managers.Camera.onCameraHigh += HandleCameraHigh;
        Managers.Stage.onGetAllKeys += HandleGetAllKeys;
    }
    private void OnDisable()
    {
        Managers.Camera.onCameraHigh -= HandleCameraHigh;
        Managers.Stage.onGetAllKeys -= HandleGetAllKeys;
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
        _goal = null;

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

    #region 이벤트 핸들러
    private void HandleCameraHigh()
    {
        if (Managers.Stage.RemainingKeyCount > 0) _goal.ReturnPool();
    }
    private void HandleGetAllKeys()
    {
        if (_goal.enabled) return;
        Managers.Pool.Spawn<Piece_Goal>(_goalPoolData, _goal.GroundNode.WorldCoordinate);
    }
    #endregion

    #region 헬퍼함수
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
                _goal = Managers.Pool.Spawn<Piece_Goal>(_goalPoolData, node.WorldCoordinate);
                break;
            case ENodeState.Key:
                Managers.Stage.AddRemainingKeyCount();
                //기물 소환

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
    #endregion
}