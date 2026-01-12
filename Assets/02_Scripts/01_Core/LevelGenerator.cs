using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<PoolableObjSO> _nodePoolDataList;
    [SerializeField] private PoolableObjSO _playerPoolData;
    [SerializeField] private PoolableObjSO _goalPoolData;

    private Dictionary<ENodeShape, PoolableObjSO> _shapeMap;

    private void Awake()
    {
        GenerateShapeMap();
        Managers.Stage.RegisterGenerator(this);
    }

    private void GenerateShapeMap()
    {
        _shapeMap = new Dictionary<ENodeShape, PoolableObjSO>();

        foreach (var config in _nodePoolDataList)
        {
            if (config.prefab is SpatialNode spatialPrefab)
            {
                if (!_shapeMap.ContainsKey(spatialPrefab.NodeShape))
                {
                    _shapeMap.Add(spatialPrefab.NodeShape, config);
                }
            }
        }
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
            if (_shapeMap.TryGetValue(nodeData.nodeShape, out var targetPool))
            {
                SpatialNode node = Managers.Pool.Spawn<SpatialNode>(targetPool, nodeData.worldPos);
                node.InjectData(nodeData);

                Managers.Stage.SetNodeMap(node);

                if (node.NodeState == ENodeState.Start)
                {
                    startNode = node;
                }
                if (node.NodeState == ENodeState.Finish)
                {
                    Managers.Pool.Spawn<Piece_Goal>(_goalPoolData, node.WorldPosition);
                    finishNode = node;
                }
            }
            else
            {
                Utils.Log($"[LevelGenerator] {nodeData.nodeShape}에 해당하는 프리팹 설정이 PoolConfigs에 없습니다.");
            }
        }
        if (startNode != null && finishNode != null)
        {
            PlayerController player = Managers.Pool.Spawn<PlayerController>(_playerPoolData, startNode.WorldPosition);
            player.Init(startNode);
            Managers.Camera.SetPlayerTarget(player);
            if (doIntro)
            {
                float introTime = startNode.WorldPosition.x - finishNode.WorldPosition.x;
                _ = Managers.Camera.StartStageIntro(startNode.WorldPosition, finishNode.WorldPosition, introTime);
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