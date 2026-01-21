using Unity.Behavior;
using UnityEngine;
using DG.Tweening;

public class Piece_Enemy : Piece_Base, IStageMovable
{
    [SerializeField] private BehaviorGraphAgent _behaviorAgent;

    public bool IsMovingEnemy { get; private set; } = false;
    public Vector2Int ForwardDir => _forwordDir;
    public SpatialNode NotifyNode => _notifyNode;
    public Animator Anim => _anim;

    private Vector2Int _forwordDir;
    private SpatialNode _notifyNode;

    private bool _isIntroEnd = false;
    private bool _isDeath = false;
    private Vector3 _deathLookDir;
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
        CompleteTurn();

        _isDeath = false;
        _isIntroEnd = false;
        _notifyNode = null;

        //카메라를 보며 적당한 애니메이션 실행
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        //애니메이션 정리
    }

    private void FixedUpdate()
    {
        if (!_isIntroEnd) RotateToCamera();
        else if (!_isDeath) RotateToTarget(transform.position + new Vector3(_forwordDir.x, 0.0f, _forwordDir.y));
        else RotateToTarget(transform.position + _deathLookDir);
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
        if (_behaviorAgent == null || _isDeath) return;
        _turnCompletionSource = new AwaitableCompletionSource();
        await _turnCompletionSource.Awaitable;
    }
    public void CompleteTurn()
    {
        if (_turnCompletionSource != null)
        {
            _turnCompletionSource?.SetResult();
            _turnCompletionSource = null;
        }
    }
    public void AttackSuccessRequest(Vector2Int enemyForwardDir, SpatialNode notifyNode)
    {
        Managers.Stage.BroadcastAttackSuccess(enemyForwardDir, notifyNode);
    }
    public void ChangeForwardDir(Vector2Int forwardDir)
    {
        _forwordDir = forwardDir;
    }
    protected override void HandleIntroEnd()
    {
        //정해진 곳으로 회전하며 아이들 전환
        _isIntroEnd = true;
    }

    protected override void HandleNotify(SpatialNode node)
    {
        SpatialNode prevPlayerNode = _notifyNode;
        _notifyNode = node;

        if (node == GroundNode)
        {
            //죽음 애니메이션
            Vector3Int prevNodePos = prevPlayerNode == null ? Vector3Int.zero : prevPlayerNode.WorldCoordinate;
            _deathLookDir = prevNodePos - _notifyNode.WorldCoordinate;
            GroundNode.ChangeNodeState(ENodeState.None);
            ReturnPoolAfterAnimation(_deathLookDir);
        }
    }
    private void ReturnPoolAfterAnimation(Vector3 deathLookDir)
    {
        _isDeath = true;
        _anim.CrossFadeInFixedTime(Defines.DEATH_HASH, 0.1f);
        Vector3 movePos = GroundNode.WorldCoordinate - _deathLookDir;
        transform.DOMove(movePos, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                ReturnPool();
            });
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
