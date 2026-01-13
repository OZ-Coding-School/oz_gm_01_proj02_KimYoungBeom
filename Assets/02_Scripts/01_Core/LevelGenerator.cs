using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private PoolableObjSO _basicNodePoolData;
    [SerializeField] private PoolableObjSO _movingNodePoolData;
    [SerializeField] private PoolableObjSO _playerPoolData;
    [SerializeField] private PoolableObjSO _goalPoolData;

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

        SpatialNode startNode = null;
        SpatialNode finishNode = null;

        foreach (var nodeData in nodeGraph.Nodes)
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
            node.InjectData(nodeData);

            Managers.Stage.SetNodeMap(node);

            if (node.NodeState == ENodeState.Start)
            {
                startNode = node;
            }
            if (node.NodeState == ENodeState.Finish)
            {
                Managers.Pool.Spawn<Piece_Goal>(_goalPoolData, node.WorldCoordinate);
                finishNode = node;
            }
            if (node is IStageMovable moveNode)
            {
                Managers.Stage.SetMovableList(moveNode);
            }
        }
        if (startNode != null && finishNode != null)
        {
            PlayerController player = Managers.Pool.Spawn<PlayerController>(_playerPoolData, startNode.WorldCoordinate);
            player.Init(startNode);
            Managers.Camera.SetPlayerTarget(player);
            if (doIntro)
            {
                float introTime = startNode.WorldCoordinate.x - finishNode.WorldCoordinate.x;
                _ = Managers.Camera.StartStageIntro(startNode.WorldCoordinate, finishNode.WorldCoordinate, introTime);
            }
            else
            {
                _ = ChangeViewAtReloadStage();
            }
        }
    }
    private async Awaitable ChangeViewAtReloadStage()
    {
        Managers.Camera.ChangeView(EViewMode.Lobby);
        await Awaitable.NextFrameAsync(destroyCancellationToken);
        Managers.Camera.ChangeView(EViewMode.Quarter);
    }
}