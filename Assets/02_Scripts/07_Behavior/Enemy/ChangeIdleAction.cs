using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeIdle", story: "Change Anim to Idle [My]", category: "Action", id: "5544256ca7b2685c09eba0576314eea5")]
public partial class ChangeIdleAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;

    private Animator _anim;
    protected override Status OnStart()
    {
        _anim = My.Value.Anim;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _anim.CrossFadeInFixedTime(Defines.IDLE_HASH, 0.2f);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

