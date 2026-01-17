using Unity.Behavior;
using UnityEngine;

public class Piece_Enemy : Piece_Base, IStageMovable
{
    [SerializeField] private BehaviorGraphAgent _behaviorAgent;

    [Header("BT용 이벤트 구독")]
    [SerializeField] private Event_ExecuteStageTurn _onExecuteStageTurn;

    public bool IsMovingEnemy { get; private set; } = false;
    public Vector2Int ForwardDir => _forwordDir;
    public bool CanAttack => _canAttack;
    public SpatialNode NotifyNode => _notifyNode;
    public Animator Anim => _anim;

    private Vector2Int _forwordDir;
    private SpatialNode _notifyNode;

    private bool _canAttack = false;
    private bool _isIntroEnd = false;

    private Animator _anim;

    private AwaitableCompletionSource _turnCompletionSource;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
    }
    public override void OnSpawn()
    {
        base.OnSpawn();
        _anim.CrossFadeInFixedTime(Defines.IDLE_HASH, 0.0f);

        //카메라를 보며 적당한 애니메이션 실행
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        //애니메이션 정리

        _canAttack = false;
        _isIntroEnd = false;
        CompleteTurn();
    }

    private void FixedUpdate()
    {
        if (!_isIntroEnd) RotateToCamera();
        else RotateToTarget(transform.position + new Vector3(_forwordDir.x, 0.0f, _forwordDir.y));
    }
    public override void InjectNode(SpatialNode groundNode)
    {
        base.InjectNode(groundNode);
        InitAttackDirection(groundNode.NodeState);
        if (groundNode.NodeState == ENodeState.MovingEnemy)
        {
            IsMovingEnemy = true;
            groundNode.ChangeNodeState(ENodeState.OnEnemyDown);
        }
        else
        {
            IsMovingEnemy = false;
        }
    }
    public async Awaitable ExecuteStageTurn()
    {
        if (_canAttack)
        {
            if (_behaviorAgent == null) return;

            _turnCompletionSource = new AwaitableCompletionSource();
            _onExecuteStageTurn.SendEventMessage();
            await _turnCompletionSource.Awaitable;

            //공격 로직 or 행동트리 실행
            //await Awaitable.WaitForSecondsAsync(0.5f);
        }
        _canAttack = false;
    }
    public void CompleteTurn()
    {
        if (_turnCompletionSource != null)
        {
            _turnCompletionSource?.SetResult();
            _turnCompletionSource = null;
        }
    }
    public void AttackSuccessRequest()
    {
        //StageManger의 onAttackSuccess 이벤트 발송
        Utils.Log("Attack Success Request");
    }

    protected override void HandleIntroEnd()
    {
        //정해진 곳으로 회전하며 아이들 전환
        _isIntroEnd = true;
    }

    protected override void HandleNotify(SpatialNode node)
    {
        _notifyNode = node;

        if (node == GroundNode)
        {
            //죽음 애니메이션

            ReturnPool();
        }
    }

    private void InitAttackDirection(ENodeState state)
    {
        switch (state)
        {
            case ENodeState.OnEnemyRight:
                _forwordDir = Vector2Int.right; break;
            case ENodeState.OnEnemyLeft:
                _forwordDir = Vector2Int.left; break;
            case ENodeState.OnEnemyUp:
                _forwordDir = Vector2Int.up; break;
            case ENodeState.OnEnemyDown:
                _forwordDir = Vector2Int.down; break;
            default:
                _forwordDir = Vector2Int.down; break;
        }
    }
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }
}
