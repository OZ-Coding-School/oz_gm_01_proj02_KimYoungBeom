using DG.Tweening;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "Perform Attack [My] , [Duration]", category: "Action", id: "ceeba2ca14e714e9e30952db599264c6")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;
    [SerializeReference] public BlackboardVariable<float> Duration;

    private SpatialNode _groundNode;
    private SpatialNode _notifyNode;
    private Animator _anim;
    private Vector3 _targetPos;
    protected override Status OnStart()
    {
        _groundNode = My.Value.GroundNode;
        _notifyNode = My.Value.NotifyNode;
        _anim = My.Value.Anim;
        _targetPos = _groundNode.WorldCoordinate + ((Vector3)(_notifyNode.WorldCoordinate - _groundNode.WorldCoordinate) / 2.0f);
        return Status.Running;

    }

    protected override Status OnUpdate()
    {
        _anim.CrossFadeInFixedTime(Defines.ATTACK_HASH, 0.0f);
        Sequence attackSeq = DOTween.Sequence();
        attackSeq.Append(
            My.Value.transform.DOMove(_targetPos, Duration.Value * 0.5f)
            .SetEase(Ease.OutQuad));
        attackSeq.AppendCallback(() =>
        {
            My.Value.AttackSuccessRequest(My.Value.ForwardDir, _notifyNode);
            My.Value.CompleteTurn();
        });
        attackSeq.AppendInterval(Duration.Value * 0.5f);
        attackSeq.Append(
            My.Value.transform.DOMove(_groundNode.WorldCoordinate, Duration.Value)
            .SetEase(Ease.InOutCubic));

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

