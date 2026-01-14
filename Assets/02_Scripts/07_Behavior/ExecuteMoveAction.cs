using DG.Tweening;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ExecuteMoveAction", story: "Move to Next Target Node and Update Map", category: "Action", id: "48d7dc341e085eee9846e9401a70d5f2")]
public partial class ExecuteMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<MovingSpatialNode> MovingSpatialNode;
    [SerializeReference] public BlackboardVariable<Vector3Int> NextTarget;
    [SerializeReference] public BlackboardVariable<float> Duration;

    protected override Status OnUpdate()
    {
        if (MovingSpatialNode.Value == null) return Status.Failure;

        PerformAsyncMove();
        return Status.Success;
    }

    private void PerformAsyncMove()
    {
        MovingSpatialNode node = MovingSpatialNode.Value;
        Vector3Int startPos = node.WorldCoordinate;
        Vector3Int targetPos = NextTarget.Value;


        node.transform.DOMove(targetPos, Duration.Value)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Managers.Stage.UpdateNode(node, startPos, targetPos);
                MovingSpatialNode.Value.CompleteTurn();
            });
    }
}

