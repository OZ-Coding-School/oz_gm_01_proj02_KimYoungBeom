using Unity.Behavior;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Piece_Enemy : Piece_Base, IStageMovable
{
    [SerializeField] private BehaviorGraphAgent _behaviorAgent;

    [Header("이벤트 구독")]
    [SerializeField] private SpatialNodeEventCHSO _onNotifyFromNode;    //PlayerController 발송
    [SerializeField] private BoolEventCHSO _onNotifyAvatar;             //PlayerController 발송
    [SerializeField] private BoolEventCHSO _onNotifyDeath;              //PlayerController 발송
    public bool IsMovingEnemy { get; private set; } = false;
    public Vector2Int ForwardDir => _forwordDir;
    public SpatialNode[] NotifyNodes => _notifyNodes;
    public Animator Anim => _anim;
    public bool IsDeath => _isDeath;

    private Vector2Int _forwordDir;
    private readonly SpatialNode[] _notifyNodes = new SpatialNode[2];

    private bool _isIntroEnd = false;
    private bool _isDeath = false;
    private bool _isAvatar;

    private Vector3 _deathLookDir;
    private Animator _anim;
    private SpatialNode _prevPlayerNode;

    private AwaitableCompletionSource _turnCompletionSource;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        _onNotifyFromNode.onEvent += RegisterFromNode;
        _onNotifyAvatar.onEvent += RecognizePlayer;
        _onNotifyDeath.onEvent += HandleAvatarDeath;

        _anim.CrossFadeInFixedTime(Defines.IDLE_HASH, 0.0f);
        CompleteTurn();

        _isDeath = false;
        _isIntroEnd = false;

        //카메라를 보며 적당한 애니메이션 실행
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        _onNotifyFromNode.onEvent -= RegisterFromNode;
        _onNotifyAvatar.onEvent -= RecognizePlayer;
        _onNotifyDeath.onEvent -= HandleAvatarDeath;

        transform.DOKill();
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
        if (_isAvatar) _notifyNodes[1] = node;
        else _notifyNodes[0] = node;

        bool isTopMatch = false;
        if (Managers.Camera.CurrentViewMode == EViewMode.Top)
        {
            SpatialNode vNotifyNode = Managers.Stage.GetNodeAt(node.GridCoordinate);
            SpatialNode vGroundNode = Managers.Stage.GetNodeAt(GroundNode.GridCoordinate);
            if (vNotifyNode == vGroundNode)
            {
                isTopMatch = true;
            }
        }
        if (node == GroundNode || isTopMatch)
        {
            //죽음 애니메이션
            Vector3Int prevNodePos = _prevPlayerNode == null ? Vector3Int.zero : _prevPlayerNode.WorldCoordinate;
            _deathLookDir = prevNodePos - node.WorldCoordinate;
            GroundNode.ChangeNodeState(ENodeState.None);
            ReturnPoolAfterAnimation(_deathLookDir);
        }
    }
    private void HandleAvatarDeath(bool isAvatar)
    {
        if (!isAvatar) return;
        _notifyNodes[1] = null;
    }
    private void RecognizePlayer(bool isAvatar)
    {
        _isAvatar = isAvatar;
    }
    private void RegisterFromNode(SpatialNode node)
    {
        _prevPlayerNode = node;
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
