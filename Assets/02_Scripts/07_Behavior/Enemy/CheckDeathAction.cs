using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckDeath", story: "Init and Check Death [My] , [IsMoving]", category: "Action", id: "eedfdb8d393f6a656b3faaad1bd9ba00")]
public partial class CheckDeathAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;
    [SerializeReference] public BlackboardVariable<bool> IsMoving;
    private bool _isDeath;
    protected override Status OnStart()
    {
        _isDeath = My.Value.IsDeath;
        IsMoving.Value = My.Value.IsMovingEnemy;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return _isDeath ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

