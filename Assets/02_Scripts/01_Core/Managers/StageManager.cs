using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Repository")]
    [SerializeField] private List<NodeGraphSO> _stageRepository = new List<NodeGraphSO>();

    private LevelGenerator _generator;
    private readonly Dictionary<Vector3Int, SpatialNode> _nodeMap3D = new Dictionary<Vector3Int, SpatialNode>();
    private readonly Dictionary<Vector2Int, SpatialNode> _nodeMap2D = new Dictionary<Vector2Int, SpatialNode>();

    public event Action onTurnCountChange;

    public int CurrentTurnCount { get; private set; }
    public Dictionary<Vector3Int, SpatialNode> NodeMap3D => _nodeMap3D;
    public Dictionary<Vector2Int, SpatialNode> NodeMap2D => _nodeMap2D;

    #region 외부호출 함수
    public void RegisterGenerator(LevelGenerator generator)
    {
        _generator = generator;
        int targetIndex = Managers.Game.CurrentStageIndex;
        if (targetIndex != -1)
        {
            RequestGenerate(targetIndex);
        }
    }

    public void RequestGenerate(int index)
    {
        RequestGenerate(index, true);
    }
    public void RequestGenerate(int index, bool doIntro)
    {
        if (_generator == null) return;
        if (index < 0) return;
        if (index >= _stageRepository.Count)
        {
            //모든 스테이지를 클리어 했을 때 (일단 로비로 돌아감)
            Managers.Game.LoadLobbyScene();
            return;
        }
        InitNodeMap();

        var nodeGraph = _stageRepository[index];
        CurrentTurnCount = nodeGraph.TurnCount;
        _generator.GenerateLevel(nodeGraph, doIntro);

        onTurnCountChange?.Invoke();
    }
    public bool UseTurn()
    {
        if (CurrentTurnCount == 0)
        {
            onTurnCountChange?.Invoke();
            return false;
        }
        CurrentTurnCount--;
        onTurnCountChange?.Invoke();
        return true;
    }
    public void SetNodeMap(SpatialNode node)
    {
        int x = node.GridCoordinate.x;
        int y = Mathf.RoundToInt(node.WorldPosition.y);
        int z = node.GridCoordinate.y;
        Vector3Int key3D = new Vector3Int(x, y, z);
        Vector2Int key2D = new Vector2Int(x, z);
        if (!_nodeMap3D.ContainsKey(key3D)) _nodeMap3D[key3D] = node;
        if (!_nodeMap2D.ContainsKey(key2D)) _nodeMap2D[key2D] = node;
    }
    public SpatialNode GetNodeAt(Vector3Int key)
    {
        if (_nodeMap3D.TryGetValue(key, out var node))
        {
            return node;
        }
        return null;
    }
    public SpatialNode GetNodeAt(Vector2Int key)
    {
        if (_nodeMap2D.TryGetValue(key, out var node))
        {
            return node;
        }
        return null;
    }
    #endregion

    #region Helper
    private void InitNodeMap()
    {
        _nodeMap2D.Clear();
        _nodeMap3D.Clear();
    }
    #endregion
}