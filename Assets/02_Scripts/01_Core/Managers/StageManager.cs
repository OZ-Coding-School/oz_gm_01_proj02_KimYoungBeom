using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Repository")]
    [SerializeField] private List<NodeGraphSO> _stageRepository = new List<NodeGraphSO>();

    [Header("이벤트 구독")]
    [SerializeField] private VoidEventCHSO _onPlayerTurnEnd;        //PlayerController 발송
    [Header("BT용 이벤트 구독")]
    [SerializeField] private Event_ExecuteStageTurn _onExecuteStageTurn;
    [SerializeField] private Event_ClearStacksRequest _onClearStacksRequest;

    private LevelGenerator _generator;
    private readonly SpatialNode[,,] _nodeMap3D = new SpatialNode[Defines.MAX_NODE_COUNT, Defines.MAX_NODE_COUNT, Defines.MAX_NODE_COUNT];
    private readonly SpatialNode[,] _nodeMap2D = new SpatialNode[Defines.MAX_NODE_COUNT, Defines.MAX_NODE_COUNT];
    private readonly List<IStageMovable> _movableList = new List<IStageMovable>();
    private bool _isProcessing = false;
    private bool _isStageClear = false;

    public event Action onTurnCountChange;
    public event Action onStageTurnEnd;
    public event Action onGenerateLevel;
    public event Action onClearRequest;
    public event Action<bool> onGetAllKeys;
    public event Action onIntroEnd;
    public event Action<Vector2Int, SpatialNode> onAttackSuccess;
    public int CurrentTurnCount { get; private set; }
    public bool IsStageTurn { get; private set; } = false;
    public int RemainingKeyCount { get; private set; }

    #region LifeCycle
    private void OnEnable()
    {
        _onPlayerTurnEnd.onEvent += HandlePlayerTurnEnd;
    }
    private void OnDisable()
    {
        _onPlayerTurnEnd.onEvent -= HandlePlayerTurnEnd;
    }
    #endregion

    #region 외부호출 함수
    //LevelGenerator 호출
    public void RegisterGenerator(LevelGenerator generator)
    {
        _generator = generator;
        int targetIndex = Managers.Game.CurrentStageIndex;
        if (targetIndex != -1)
        {
            RequestGenerate(targetIndex);
        }
    }
    public void SetNodeMap(SpatialNode node)
    {
        var keyForArray3D = CalculateArrayIndex(node.WorldCoordinate);
        var keyForArray2D = CalculateArrayIndex(node.GridCoordinate);

        _nodeMap3D[keyForArray3D.x, keyForArray3D.y, keyForArray3D.z] = node;
        if (_nodeMap2D[keyForArray2D.x, keyForArray2D.y] != null)
        {
            if (_nodeMap2D[keyForArray2D.x, keyForArray2D.y].WorldCoordinate.y < node.WorldCoordinate.y)
            {
                _nodeMap2D[keyForArray2D.x, keyForArray2D.y] = node;
            }
        }
        else
        {
            _nodeMap2D[keyForArray2D.x, keyForArray2D.y] = node;
        }
    }
    public void SetMovableList(IStageMovable movable)
    {
        _movableList.Add(movable);
    }
    public void IntroEndRequest()
    {
        onIntroEnd?.Invoke();
    }
    public void AddRemainingKeyCount()
    {
        RemainingKeyCount++;
    }
    //GameManager 호출 - 재시작
    public void RequestGenerate(int index)
    {
        RequestGenerate(index, true);
    }

    //InputManager 호출(false로)
    public void RequestGenerate(int index, bool doIntro)
    {
        _isStageClear = false;

        _onClearStacksRequest.SendEventMessage();
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

        onGenerateLevel?.Invoke();
        onTurnCountChange?.Invoke();
    }

    //PlayerController 호출(Command Pattern)
    public bool UseTurn()
    {
        if (!_isProcessing)
        {
            _isProcessing = true;
            if (CurrentTurnCount == 0)
            {
                ControlTurnCountAsync();
                return false;
            }
            ControlTurnCountAsync();
        }
        return CurrentTurnCount != 0;
    }
    private async void ControlTurnCountAsync()
    {
        await Awaitable.NextFrameAsync();
        CurrentTurnCount--;
        if (CurrentTurnCount < 0) CurrentTurnCount = 0;
        onTurnCountChange?.Invoke();
        _isProcessing = false;
    }

    //MovingNode - Action에서 호출
    public void UpdateNode(SpatialNode node, Vector3Int from, Vector3Int to)
    {
        node.SetCoordinate(to);

        Vector3Int fromKeyForArray = CalculateArrayIndex(from);
        Vector3Int toKeyForArray = CalculateArrayIndex(to);

        _nodeMap3D[fromKeyForArray.x, fromKeyForArray.y, fromKeyForArray.z] = null;
        _nodeMap3D[toKeyForArray.x, toKeyForArray.y, toKeyForArray.z] = node;

        if (_nodeMap2D[fromKeyForArray.x, fromKeyForArray.z] == node)
        {
            _nodeMap2D[fromKeyForArray.x, fromKeyForArray.z] = null;
            for (int i = Defines.MAX_NODE_COUNT - 1; i >= 0; i--)
            {
                if (_nodeMap3D[fromKeyForArray.x, i, fromKeyForArray.z] != null)
                {
                    _nodeMap2D[fromKeyForArray.x, fromKeyForArray.z] = _nodeMap3D[fromKeyForArray.x, i, fromKeyForArray.z];
                    break;
                }
            }
        }
        if (_nodeMap2D[toKeyForArray.x, toKeyForArray.z] != null)
        {
            if (_nodeMap2D[toKeyForArray.x, toKeyForArray.z].WorldCoordinate.y < node.WorldCoordinate.y)
            {
                _nodeMap2D[toKeyForArray.x, toKeyForArray.z] = node;
            }
        }
        else
        {
            _nodeMap2D[toKeyForArray.x, toKeyForArray.z] = node;
        }

    }

    //Pieces 호출
    public void StageClearRequest()
    {
        _isStageClear = true;
        onClearRequest?.Invoke();
    }
    public void DeactiveButton()
    {
        RemainingKeyCount++;
        onGetAllKeys?.Invoke(false);
    }
    public void GetKeyRequest()
    {
        RemainingKeyCount--;
        if (RemainingKeyCount <= 0)
        {
            onGetAllKeys?.Invoke(true);
        }
    }
    public void BroadcastAttackSuccess(Vector2Int enemyForward, SpatialNode notifyNode)
    {
        onAttackSuccess?.Invoke(enemyForward, notifyNode);
    }
    // 여러 곳 ~
    public SpatialNode GetNodeAt(Vector3Int key)
    {
        Vector3Int keyForArray = CalculateArrayIndex(key);
        if (keyForArray.x < 0 || keyForArray.z < 0 || keyForArray.y < 0) return null;
        var node = _nodeMap3D[keyForArray.x, keyForArray.y, keyForArray.z];
        return node;
    }
    public SpatialNode GetNodeAt(Vector2Int key)
    {
        var keyForArray = CalculateArrayIndex(key);
        if (keyForArray.x < 0 || keyForArray.y < 0) return null;
        var node = _nodeMap2D[keyForArray.x, keyForArray.y];
        return node;
    }
    #endregion

    #region 턴 관리
    private async void StartStageTurn()
    {
        if (!IsStageTurn) return;

        List<Awaitable> tasks = new List<Awaitable>();
        foreach (var obj in _movableList)
        {
            tasks.Add(obj.ExecuteStageTurn());
        }
        _onExecuteStageTurn.SendEventMessage();
        foreach (var task in tasks)
        {
            await task;
        }

        IsStageTurn = false;
        onStageTurnEnd?.Invoke();
    }
    #endregion

    #region 이벤트 핸들러
    private void HandlePlayerTurnEnd()
    {
        if (IsStageTurn) return;
        if (_isStageClear) return;

        IsStageTurn = true;

        StartStageTurn();
    }

    #endregion

    #region Helper
    private void InitNodeMap()
    {
        Array.Clear(_nodeMap2D, 0, _nodeMap2D.Length);
        Array.Clear(_nodeMap3D, 0, _nodeMap3D.Length);
        _movableList.Clear();
        RemainingKeyCount = 0;
    }
    private Vector3Int CalculateArrayIndex(Vector3Int target)
    {
        int offsetY = Mathf.FloorToInt(Defines.MAX_NODE_COUNT / 2);
        return new Vector3Int(-target.x, target.y + offsetY, target.z);
    }
    private Vector2Int CalculateArrayIndex(Vector2Int target)
    {
        return new Vector2Int(-target.x, target.y);
    }
    #endregion
}