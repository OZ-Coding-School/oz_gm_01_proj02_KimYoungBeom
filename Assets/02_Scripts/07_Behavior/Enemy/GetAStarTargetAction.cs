using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetAStarTarget", story: "Find Next Moving Node [My]", category: "Action", id: "b51cbe8048a85efe182f65b3856a665d")]
public partial class GetAStarTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;

    private SpatialNode _groundNode;
    private SpatialNode _playerNode;
    private bool _isTopView;
    protected override Status OnStart()
    {
        _groundNode = My.Value.GroundNode;
        _playerNode = My.Value.NotifyNode;
        if (Managers.Camera.CurrentViewMode == EViewMode.Top) _isTopView = true;
        else _isTopView = false;

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

