using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ClearStacks", story: "Clear [VisitedList] and [PathStack]", category: "Action", id: "a407426e21bb62a8c5b608abdc75e48c")]
public partial class ClearStacksAction : Action
{
    [SerializeReference] public BlackboardVariable<List<Vector3Int>> PathStack;
    [SerializeReference] public BlackboardVariable<List<Vector3Int>> VisitedList;
    [SerializeReference] public BlackboardVariable<MovingSpatialNode> MovingSpatialNode;

    protected override Status OnStart()
    {
        VisitedList.Value.Clear();
        PathStack.Value.Clear();
        MovingSpatialNode.Value.CompleteTurn();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

