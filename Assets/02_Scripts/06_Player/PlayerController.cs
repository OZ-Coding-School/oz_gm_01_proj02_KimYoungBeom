using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PoolableComponent
{
    #region 참조
    [SerializeField] private float _moveDuration = 0.867f;
    [SerializeField] private float _sadIdleCool = 3.0f;
    [SerializeField] private float _rotateSpeed = 15.0f;
    #endregion

    #region 상태머신관련
    private StateMachine _stateMC;

    private IdleState _idleState;
    private MoveState _moveState;
    private GoToState _goToState;
    private GoFromState _goFromState;
    private FinishState _finishState;
    #endregion

    #region public 멤버
    #endregion

    #region 프로퍼티
    public SpatialNode CurrentNode { get; private set; }
    public bool IsMoving { get; set; } = false;
    public bool IsGoTo { get; set; } = false;
    public bool IsGoFrom { get; set; } = false;
    public Animator Anim => _anim;
    public float SadIdleCool => _sadIdleCool;
    #endregion

    #region private 멤버
    //컴포넌트
    private Animator _anim;

    //런타임 변수
    private Dictionary<Vector3Int, SpatialNode> _nodeMap;
    private Stack<ICommand> _history = new Stack<ICommand>();
    private Vector2Int _rotateDir = new Vector2Int();

    //상태관리 변수
    private bool _isRotate = false;
    #endregion

    #region LifeCycle
    private void Awake()
    {
        _anim = GetComponent<Animator>();

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
        _stateMC.ChangeState(_idleState);

        Managers.Input.onMoveEvent += OnMove;
        Managers.Input.onUnDoEvent += OnUnDo;
    }
    private void OnDisable()
    {
        Managers.Input.onMoveEvent -= OnMove;
        Managers.Input.onUnDoEvent -= OnUnDo;
    }
    private void Update()
    {
        _stateMC?.Update();
    }
    private void FixedUpdate()
    {
        _stateMC?.FixedUpdate();
        if (_isRotate) RotateToInputDir();
    }
    public void Init(Dictionary<Vector3Int, SpatialNode> nodeMap, SpatialNode startNode)
    {
        _nodeMap = nodeMap;
        SetCurrentNode(startNode);
        transform.position = startNode.WorldPosition + Defines.PLAYER_Y_OFFSET;
        _history.Clear();
    }
    public void SetCurrentNode(SpatialNode node) => CurrentNode = node;
    #endregion

    #region 상태 전환조건 모음
    private void InitTransitions()
    {
        //From Idle
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

        IsGoFrom = true;
        IsMoving = true;
        ICommand lastCommand = _history.Pop();
        RotateStart(lastCommand.MoveDir);
        lastCommand.UnDo();
    }

    private void TryMove(Vector2Int direction)
    {
        if (!CurrentNode.MoveableDirections.Contains(direction)) return;

        Vector3Int targetKey = new Vector3Int(
            CurrentNode.GridCoordinate.x + direction.x,
            Mathf.RoundToInt(CurrentNode.WorldPosition.y),
            CurrentNode.GridCoordinate.y + direction.y
        );

        if (_nodeMap.TryGetValue(targetKey, out SpatialNode targetNode))
        {
            IsGoTo = true;
            IsMoving = true;
            MoveCommand moveCmd = new MoveCommand(this, CurrentNode, targetNode, _moveDuration);

            moveCmd.Execute();
            _history.Push(moveCmd);
        }
    }
    #endregion

    #region Helper 함수
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
        IsMoving = false;
        IsGoTo = false;
        IsGoFrom = false;
        _isRotate = false;
        if (_anim != null)
        {
            _anim.Rebind();
            _anim.Update(0);
        }
    }
    #endregion

    public override void OnSpawn()
    {

    }

    public override void OnDespawn()
    {
        InitAtDespawn();
    }
}