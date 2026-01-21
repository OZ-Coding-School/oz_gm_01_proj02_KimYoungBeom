using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeDirAtNotMove", story: "Change Direction after Not Moving [My]", category: "Action", id: "59ca810a4aaee186b80f9fc3ef5636ed")]
public partial class ChangeDirAtNotMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;

    private SpatialNode _groundNode;
    private Vector2Int _forwardDir;
    private Vector2Int[] _movablDir;
    protected override Status OnStart()
    {
        _groundNode = My.Value.GroundNode;
        _forwardDir = My.Value.ForwardDir;
        _movablDir = _groundNode.GetTrailDirection();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        ChangeDirection();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }

    private void ChangeDirection()
    {
        if (_movablDir.Length < 2) return;
        if (_movablDir[1] == Vector2Int.zero) return;
        foreach (var move in _movablDir)
        {
            if (move != _forwardDir)
            {
                My.Value.ChangeForwardDir(move);
                break;
            }
        }
    }
}

