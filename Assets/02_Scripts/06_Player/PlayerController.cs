using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PoolableComponent
{
    #region 참조
    [Header("수치제어")]
    [SerializeField] private float _moveDuration = 0.87f;
    [SerializeField] private float _sadIdleCool = 3.0f;
    [SerializeField] private float _rotateSpeed = 15.0f;

    [Header("이벤트 발송")]
    [SerializeField] private SpatialNodeEventCHSO _onNotifySpecialNode;
    [SerializeField] private VoidEventCHSO _onStageClear;
    [SerializeField] private BoolEventCHSO _onPlayerMoving;

    [Header("컴포넌트 참조")]
    [SerializeField] private Transform _eyePoint;
    #endregion

    #region 상태머신관련
    private StateMachine _stateMC;

    private IdleState _idleState;
    private MoveState _moveState;
    private GoToState _goToState;
    private GoFromState _goFromState;
    private FinishState _finishState;
    #endregion

    #region private 멤버
    //컴포넌트
    private Animator _anim;

    //런타임 변수
    private Dictionary<Vector3Int, SpatialNode> _nodeMap3D;
    private Dictionary<Vector2Int, SpatialNode> _nodeMap2D = new Dictionary<Vector2Int, SpatialNode>();
    private Stack<ICommand> _history = new Stack<ICommand>();
    private readonly Dictionary<int, float> _clipLenghCacheDic = new Dictionary<int, float>();

    //상태관리 변수
    private Vector2Int _rotateDir = new Vector2Int();
    private bool _isRotate = false;
    private bool _isLastMove = false;
    private EViewMode _currentView = EViewMode.Quarter;

    #endregion

    #region public 멤버
    #endregion

    #region 프로퍼티
    public SpatialNode CurrentNode { get; private set; }
    public SpatialNode FromNode { get; private set; }
    public bool IsMoving { get; set; } = false;
    public bool IsGoTo { get; set; } = false;
    public bool IsGoFrom { get; set; } = false;
    public Animator Anim => _anim;
    public float SadIdleCool => _sadIdleCool;
    public VoidEventCHSO OnStageClear => _onStageClear;
    public Transform EyePoint => _eyePoint;
    public EViewMode CurrentView => _currentView;
    //이벤트
    public BoolEventCHSO OnPlayerMoving => _onPlayerMoving;
    #endregion


    #region LifeCycle
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        InitClipLength(_anim.runtimeAnimatorController);

        _stateMC = new StateMachine();
        _idleState = new IdleState(this);
        _moveState = new MoveState(this);
        _goToState = new GoToState(this, _moveState);
        _goFromState = new GoFromState(this, _moveState);
        _finishState = new FinishState(this);

        InitTransitions();

    }
    private void OnEnable()
    {
        Managers.Input.onMoveEvent += OnMove;
        Managers.Input.onUnDoEvent += OnUnDo;
        Managers.Camera.onViewChanged += HandleViewChanged;
    }
    private void OnDisable()
    {
        Managers.Input.onMoveEvent -= OnMove;
        Managers.Input.onUnDoEvent -= OnUnDo;
        Managers.Camera.onViewChanged -= HandleViewChanged;
    }
    private void Update()
    {
        _stateMC?.Update();
    }
    private void FixedUpdate()
    {
        _stateMC?.FixedUpdate();
        if (_isRotate && _currentView != EViewMode.FirstPerson) RotateToInputDir();
    }
    private void LateUpdate()
    {
        if (_currentView != EViewMode.FirstPerson) return;
        transform.rotation = Quaternion.Euler(0f, Managers.Camera.GetPanValue(), 0f);
    }
    public void Init(Dictionary<Vector3Int, SpatialNode> nodeMap, SpatialNode startNode)
    {
        InitAtDespawn();
        _nodeMap3D = nodeMap;
        SetCurrentNode(startNode);
        transform.position = startNode.WorldPosition + Defines.PLAYER_Y_OFFSET;
        _history.Clear();

        _nodeMap2D.Clear();
        foreach (var node in nodeMap)
        {
            var key = node.Value.GridCoordinate;
            var value = node.Value;
            if (!_nodeMap2D.ContainsKey(key))
            {
                _nodeMap2D[key] = value;
            }
        }
    }
    #endregion

    #region 외부 호출
    public void SetCurrentNode(SpatialNode node) => CurrentNode = node;
    public void SetFromNode(SpatialNode node) => FromNode = node;
    public float GetClipLength(int hash)
    {
        return _clipLenghCacheDic.TryGetValue(hash, out float clipLength) ? clipLength : 0.0f;
    }
    #endregion

    #region 이벤트핸들러
    private void HandleViewChanged(EViewMode mode)
    {
        _currentView = mode;
    }
    #endregion

    #region 상태 전환조건 모음
    private void InitTransitions()
    {
        //From Idle
        _stateMC.AddTransition(_idleState, _finishState, () => _isLastMove);
        _stateMC.AddTransition(_idleState, _moveState, () => IsMoving);

        //From Move
        _stateMC.AddTransition(_moveState, _idleState, () => !IsMoving);
        _stateMC.AddTransition(_moveState, _goToState, () => IsGoTo && !_stateMC.IsCurrentState(_goToState));
        _stateMC.AddTransition(_moveState, _goFromState, () => IsGoFrom && !_stateMC.IsCurrentState(_goFromState));
    }
    #endregion

    #region 커맨드패턴
    public void OnMove(Vector2 input)
    {
        if (IsMoving || CurrentNode == null) return;
        Vector2Int dir = GetDiscreteDirection(input);

        if (dir != Vector2Int.zero)
        {
            RotateStart(dir);
            TryMove(dir);
        }
    }

    public void OnUnDo()
    {
        if (IsMoving || _history.Count == 0) return;
        if (Managers.Camera.IsBlending) return;
        if (_currentView == EViewMode.FirstPerson) return;

        ICommand lastCommand = _history.Pop();

        if (!lastCommand.CheckUnDo())
        {
            _history.Push(lastCommand);
            return;
        }
        if (!Managers.Stage.UseTurn()) return;

        RotateStart(lastCommand.MoveDir);
        IsGoFrom = true;
        IsMoving = true;
        _onPlayerMoving.Raised(IsMoving);

        lastCommand.UnDo();
    }

    private void TryMove(Vector2Int direction)
    {
        if (!CurrentNode.MoveableDirections.Contains(direction)) return;
        if (Managers.Camera.IsBlending) return;
        if (_currentView == EViewMode.FirstPerson) return;

        if (_currentView == EViewMode.Top)
        {
            Vector2Int targetKey = CurrentNode.GridCoordinate + direction;

            if (_nodeMap2D.TryGetValue(targetKey, out SpatialNode targetNode) && Managers.Stage.UseTurn())
            {
                ExecuteCommand(targetNode);
            }
        }
        else
        {
            Vector3Int targetKey = new Vector3Int(
                CurrentNode.GridCoordinate.x + direction.x,
                Mathf.RoundToInt(CurrentNode.WorldPosition.y),
                CurrentNode.GridCoordinate.y + direction.y
            );

            if (_nodeMap3D.TryGetValue(targetKey, out SpatialNode targetNode) && Managers.Stage.UseTurn())
            {
                ExecuteCommand(targetNode);
            }
        }
    }
    private void ExecuteCommand(SpatialNode targetNode)
    {
        IsGoTo = true;
        IsMoving = true;
        _onPlayerMoving.Raised(IsMoving);
        MoveCommand moveCmd = new MoveCommand(this, CurrentNode, targetNode, _moveDuration);

        moveCmd.Execute();
        _history.Push(moveCmd);

        NotifySpecialNode(targetNode);
    }
    #endregion

    #region Helper 함수
    private void InitClipLength(RuntimeAnimatorController controller)
    {
        foreach (var clip in controller.animationClips)
        {
            int hash = Animator.StringToHash(clip.name);
            if (!_clipLenghCacheDic.ContainsKey(hash))
            {
                _clipLenghCacheDic.Add(hash, clip.length);
            }
        }
    }
    private void NotifySpecialNode(SpatialNode node)
    {
        switch (node.NodeState)
        {
            case ENodeState.Finish:
                _ = NotifySpecialNodeAsync(node, 0.7f);
                break;
            default: break;
        }
    }
    private async Awaitable NotifySpecialNodeAsync(SpatialNode node, float durationMultiplier)
    {
        try
        {
            await Awaitable.WaitForSecondsAsync(_moveDuration * durationMultiplier, destroyCancellationToken);

            if (node.NodeState == ENodeState.Finish)
            {
                _isLastMove = true;
                _history.Clear();
            }

            _onNotifySpecialNode?.Raised(node);
        }
        catch
        {
            Utils.Log("PlayerController - NotifySpecialNodeAsync CATCH");
        }
    }
    private Vector2Int GetDiscreteDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f) return Vector2Int.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector2Int(input.x > 0 ? 1 : -1, 0);
        }
        else
        {
            return new Vector2Int(0, input.y > 0 ? 1 : -1);
        }
    }
    private void RotateStart(Vector2Int input)
    {
        _rotateDir = input;
        _isRotate = true;
    }
    private void RotateToInputDir()
    {
        if (_rotateDir.sqrMagnitude < 0.01f) return;

        Vector3 targetDir = new Vector3(_rotateDir.x, 0.0f, _rotateDir.y);
        Quaternion lookQtrn = Quaternion.LookRotation(targetDir, Vector3.up);

        float angleDiff = Quaternion.Angle(transform.rotation, lookQtrn);

        if (angleDiff < 0.5f)
        {
            transform.rotation = lookQtrn;
            _isRotate = false;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookQtrn, Time.fixedDeltaTime * _rotateSpeed);
        }
    }
    private void InitAtDespawn()
    {
        DG.Tweening.DOTween.KillAll();
        IsMoving = false;
        IsGoTo = false;
        IsGoFrom = false;
        _isRotate = false;
        _isLastMove = false;
        if (_anim != null)
        {
            _anim.Rebind();
            _anim.Update(0);
        }
    }
    #endregion

    #region PoolableComponenet
    public override void OnSpawn()
    {
        _currentView = EViewMode.Quarter;
        _stateMC.ChangeState(_idleState);
    }

    public override void OnDespawn()
    {
        InitAtDespawn();
    }
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }
    #endregion
}