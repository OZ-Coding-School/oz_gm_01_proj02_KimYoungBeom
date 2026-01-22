using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeDirection", story: "Change FWD Dir to Move [My]", category: "Action", id: "e51598b9b0a2e3c65129f33c14be9764")]
public partial class ChangeDirectionAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;

    private SpatialNode _groundNode;
    private SpatialNode _virtualNode;
    private Vector2Int _forwardDir;
    private Vector2Int[] _movablDir;
    protected override Status OnStart()
    {
        _forwardDir = My.Value.ForwardDir;
        _groundNode = My.Value.GroundNode;
        if (Managers.Camera.CurrentViewMode == EViewMode.Top)
        {
            _virtualNode = Managers.Stage.GetNodeAt(_groundNode.GridCoordinate);
            _movablDir = _virtualNode.GetTrailDirection();
        }
        else
        {
            _movablDir = _groundNode.GetTrailDirection();
        }

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
        if (_movablDir[0] == Vector2Int.zero) return;
        if (_movablDir[1] == Vector2Int.zero)
        {
            if (_movablDir[0] != _forwardDir && _movablDir[0] != Vector2Int.zero)
            {
                My.Value.ChangeForwardDir(_movablDir[0]);
            }
        }
        else
        {
            foreach (var move in _movablDir)
            {
                if (move != -_forwardDir)
                {
                    My.Value.ChangeForwardDir(move);
                    break;
                }
            }
        }
    }
}