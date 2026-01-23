using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CompleteTurn", story: "End Stage Trun [My]", category: "Action", id: "d0731d577839b6b6b62c995bf343b466")]
public partial class CompleteTurnAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        My.Value.CompleteTurn();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

