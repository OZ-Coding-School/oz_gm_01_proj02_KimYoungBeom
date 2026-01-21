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
    private Vector2Int _forward;
    private readonly Vector2Int[] _moveabldDir = new Vector2Int[2];
    protected override Status OnStart()
    {
        _groundNode = My.Value.GroundNode;
        _forward = My.Value.ForwardDir;

        Array.Clear(_moveabldDir, 0, _moveabldDir.Length);
        SetMovbleDir();
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
        if (_moveabldDir.Length < 1) return;

        if (_moveabldDir.Length < 2)
        {
            if (_moveabldDir[0] != _forward)
            {
                My.Value.ChangeForwardDir(_moveabldDir[0]);
            }
        }
        else
        {
            foreach (var move in _moveabldDir)
            {
                if (move != -_forward)
                {
                    My.Value.ChangeForwardDir(move);
                    break;
                }
            }
        }
    }
    private void SetMovbleDir()
    {
        switch (_groundNode.NodeTrail)
        {
            case ENodeTrail.Vertical:
                _moveabldDir[0] = Vector2Int.up;
                _moveabldDir[1] = Vector2Int.down;
                break;
            case ENodeTrail.Horizontal:
                _moveabldDir[0] = Vector2Int.left;
                _moveabldDir[1] = Vector2Int.right;
                break;
            case ENodeTrail.UpRight:
                _moveabldDir[0] = Vector2Int.up;
                _moveabldDir[1] = Vector2Int.right;
                break;
            case ENodeTrail.UpLeft:
                _moveabldDir[0] = Vector2Int.up;
                _moveabldDir[1] = Vector2Int.left;
                break;
            case ENodeTrail.DownLeft:
                _moveabldDir[0] = Vector2Int.down;
                _moveabldDir[1] = Vector2Int.left;
                break;
            case ENodeTrail.DownRight:
                _moveabldDir[0] = Vector2Int.down;
                _moveabldDir[1] = Vector2Int.right;
                break;
            case ENodeTrail.Up:
                _moveabldDir[0] = Vector2Int.up;
                break;
            case ENodeTrail.Down:
                _moveabldDir[0] = Vector2Int.down;
                break;
            case ENodeTrail.Left:
                _moveabldDir[0] = Vector2Int.left;
                break;
            case ENodeTrail.Right:
                _moveabldDir[0] = Vector2Int.right;
                break;
        }
    }
}