using System;
using System.Threading;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private PoolableObjSO _basicNodePoolData;
    [SerializeField] private PoolableObjSO _movingNodePoolData;
    [SerializeField] private PoolableObjSO _playerPoolData;
    [SerializeField] private PoolableObjSO _goalPoolData;
    [SerializeField] private PoolableObjSO _keyPoolData;
    [SerializeField] private PoolableObjSO _fixedEnemyPoolData;
    [SerializeField] private PoolableObjSO _movingEnemyPoolData;

    private SpatialNode _startNode;
    private SpatialNode _finishNode;
    private PlayerController _player;

    private Piece_Goal _goal;
    private CancellationTokenSource _currentCts;
    private void Awake()
    {
        Managers.Stage.RegisterGenerator(this);
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    public void GenerateLevel(NodeGraphSO nodeGraph)
    {
        GenerateLevel(nodeGraph, true);
    }
    public void GenerateLevel(NodeGraphSO nodeGraph, bool doIntro)
    {
        StopCurrentTask();

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
        }

        StartStageIntroCameraMove(doIntro);
    }

    #region 이벤트 핸들러


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
                CheckAndSetMovableList(node);
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
                _goal.InjectNode(node);
                break;
            case ENodeState.Key:
                Managers.Stage.AddRemainingKeyCount();
                var key = Managers.Pool.Spawn<Piece_Key>(_keyPoolData, node.WorldCoordinate);
                key.InjectNode(node);
                break;
            case ENodeState.OnEnemyDown:
            case ENodeState.OnEnemyUp:
            case ENodeState.OnEnemyRight:
            case ENodeState.OnEnemyLeft:
                var fixedEnemy = Managers.Pool.Spawn<Piece_Enemy>(_fixedEnemyPoolData, node.WorldCoordinate);
                fixedEnemy.InjectNode(node);
                CheckAndSetMovableList(fixedEnemy);
                break;
            case ENodeState.MovingEnemy:
                var enemy = Managers.Pool.Spawn<Piece_Enemy>(_movingEnemyPoolData, node.WorldCoordinate);
                enemy.InjectNode(node);
                CheckAndSetMovableList(enemy);
                break;
        }
    }
    private void CheckAndSetMovableList(Component comp)
    {
        if (comp is IStageMovable movable)
        {
            Managers.Stage.SetMovableList(movable);
        }
    }
    private void StartStageIntroCameraMove(bool doIntro)
    {
        if (_startNode != null && _finishNode != null)
        {
            Managers.Camera.SetPlayerTarget(_player);
            if (doIntro)
            {
                float introTimeX = _startNode.WorldCoordinate.x - _finishNode.WorldCoordinate.x;
                float introTimeY = _finishNode.WorldCoordinate.z - _startNode.WorldCoordinate.z;
                float introTime = Mathf.Max(introTimeX, introTimeY);
                _ = Managers.Camera.StartStageIntro(_startNode.WorldCoordinate, _finishNode.WorldCoordinate, introTime);
            }
            else
            {
                _ = ChangeViewAtReloadStage();
                IntroEndAfterOneSecAsync();
            }
        }
    }
    private async void IntroEndAfterOneSecAsync()
    {
        _currentCts = new CancellationTokenSource();
        try
        {
            await Awaitable.WaitForSecondsAsync(1.0f, _currentCts.Token);
            Managers.Stage.IntroEndRequest();
        }
        catch (OperationCanceledException)
        {
            Utils.Log("Hide Goal 1초 대기가 취소 됨");
        }
    }
    private void StopCurrentTask()
    {
        if (_currentCts != null)
        {
            _currentCts.Cancel();
            _currentCts.Dispose();
            _currentCts = null;
        }
    }
    #endregion
}