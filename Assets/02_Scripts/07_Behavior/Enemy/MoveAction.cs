using DG.Tweening;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move", story: "Move forward Node [My] , [To] ,[Duration]", category: "Action", id: "7dafc7bf9caa2cad3f6d4c31c8d3153e")]
public partial class MoveAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;
    [SerializeReference] public BlackboardVariable<SpatialNode> To;
    [SerializeReference] public BlackboardVariable<float> Duration;

    private SpatialNode _fromNode;
    private SpatialNode _toNode;
    private Vector2Int _forwardDir;
    protected override Status OnStart()
    {
        _fromNode = My.Value.GroundNode;
        _toNode = To.Value;
        _forwardDir = My.Value.ForwardDir;

        My.Value.Anim.CrossFadeInFixedTime(Defines.MOVE_HASH, 0.1f);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        My.Value.transform.DOMove(_toNode.WorldCoordinate, Duration.Value)
            .SetEase(Ease.InOutQuad);
        _fromNode.ChangeNodeState(ENodeState.None);
        _toNode.ChangeNodeState(My.Value.GetNodeState(_forwardDir));
        My.Value.ChangeGroundNode(_toNode);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

