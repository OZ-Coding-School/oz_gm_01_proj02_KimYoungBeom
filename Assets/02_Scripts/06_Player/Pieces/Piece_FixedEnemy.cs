using UnityEngine;

public class Piece_FixedEnemy : Piece_Base, IStageMovable
{
    private Vector2Int _forwordDir;
    private SpatialNode _notifyNode;

    private bool _canAttack = false;
    private bool _isIntroEnd = false;

    private Animator _anim;

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
    }

    private void FixedUpdate()
    {
        if (!_isIntroEnd) RotateToCamera();
        else RotateToTarget(transform.position + new Vector3(_forwordDir.x, 0.0f, _forwordDir.y));
    }

    protected override void HandleIntroEnd()
    {
        //정해진 곳으로 회전하며 아이들 전환
        _isIntroEnd = true;
    }

    protected override void HandleNotify(SpatialNode node)
    {
        _notifyNode = node;
        //top view이면 GridCoord 비교
        if (Managers.Camera.CurrentViewMode == EViewMode.Top)
        {
            Vector2Int targetKey = GroundNode.GridCoordinate + _forwordDir;
            CheckAttack(targetKey);
        }
        //Quarter view이면 WorldCoord 비교
        else
        {
            var worldCoord = GroundNode.WorldCoordinate;
            Vector3Int targetKey = worldCoord + new Vector3Int(_forwordDir.x, worldCoord.y, _forwordDir.y);
            CheckAttack(targetKey);
        }

        if (node == GroundNode)
        {
            //죽음
            ReturnPool();
        }
    }
    private void CheckAttack(Vector2Int targetKey)
    {
        var targetNode = Managers.Stage.GetNodeAt(targetKey);

        if (targetNode == _notifyNode)
        {
            _canAttack = true;
        }
    }
    private void CheckAttack(Vector3Int targetKey)
    {
        var targetNode = Managers.Stage.GetNodeAt(targetKey);

        if (targetNode == _notifyNode)
        {
            _canAttack = true;
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
    public override void InjectNode(SpatialNode groundNode)
    {
        base.InjectNode(groundNode);
        InitAttackDirection(groundNode.NodeState);
    }
    public async Awaitable ExecuteStageTurn()
    {
        if (_canAttack)
        {
            _anim.CrossFadeInFixedTime(Defines.ATTACK_HASH, 0.0f);

            //공격 로직 or 행동트리 실행
            await Awaitable.WaitForSecondsAsync(0.5f);
            Utils.Log("공격 판정 수행");
        }
        else
        {
            Utils.Log("_canAttack is False");
        }
        _canAttack = false;
    }
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }
}
