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

