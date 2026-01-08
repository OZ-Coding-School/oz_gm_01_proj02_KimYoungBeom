using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<PoolableObjSO> _nodePoolDataList;
    [SerializeField] private PoolableObjSO _playerPoolData;
    [SerializeField] private PoolableObjSO _goalPoolData;

    private Dictionary<ENodeShape, PoolableObjSO> _shapeMap;
    private Dictionary<Vector3Int, SpatialNode> _nodeMap = new Dictionary<Vector3Int, SpatialNode>();
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
        if (nodeGraph == null) return;

        _nodeMap.Clear();
        Managers.Pool.DespawnAll();

        SpatialNode startNode = null;

        foreach (var nodeData in nodeGraph.Nodes)
        {
            if (_shapeMap.TryGetValue(nodeData.nodeShape, out var targetPool))
            {
                SpatialNode node = Managers.Pool.Spawn<SpatialNode>(targetPool, nodeData.worldPos);
                node.InjectData(nodeData);

                Vector3Int nodeKey = new Vector3Int(
                    node.GridCoordinate.x,
                    Mathf.RoundToInt(nodeData.worldPos.y),
                    node.GridCoordinate.y
                    );
                _nodeMap[nodeKey] = node;

                if (node.NodeState == ENodeState.Start)
                {
                    startNode = node;
                }
                if (node.NodeState == ENodeState.Finish)
                {
                    Managers.Pool.Spawn<Piece_Goal>(_goalPoolData, node.WorldPosition);
                }
            }
            else
            {
                Utils.Log($"[LevelGenerator] {nodeData.nodeShape}에 해당하는 프리팹 설정이 PoolConfigs에 없습니다.");
            }
        }
        if (startNode != null)
        {
            PlayerController player = Managers.Pool.Spawn<PlayerController>(_playerPoolData, startNode.WorldPosition);
            player.Init(_nodeMap, startNode);
        }
    }
}