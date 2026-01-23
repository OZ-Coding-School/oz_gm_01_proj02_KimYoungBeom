using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckMove", story: "Check Can Move [My] , [To]", category: "Action", id: "bac60f937d1ab6997415888a6620a635")]
public partial class CheckMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;
    [SerializeReference] public BlackboardVariable<SpatialNode> To;

    private SpatialNode _groundNode;
    private Vector2Int _forwardDir;
    private bool _isTopView;
    private bool _canMove;
    protected override Status OnStart()
    {
        _groundNode = My.Value.GroundNode;
        _forwardDir = My.Value.ForwardDir;
        _isTopView = (Managers.Camera.CurrentViewMode == EViewMode.Top);
        _canMove = false;
        To.Value = null;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isTopView)
        {
            Vector2Int targetKey = _groundNode.GridCoordinate + _forwardDir;
            CheckMove(targetKey);
        }
        else
        {
            var worldCoord = _groundNode.WorldCoordinate;
            Vector3Int targetKey = worldCoord + new Vector3Int(_forwardDir.x, worldCoord.y, _forwardDir.y);
            CheckMove(targetKey);
        }
        return _canMove ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
    }

    private void CheckMove(Vector2Int targetKey)
    {
        var targetNode = Managers.Stage.GetNodeAt(targetKey);
        CheckTargetTrailDir(targetNode);
    }
    private void CheckMove(Vector3Int targetKey)
    {
        var targetNode = Managers.Stage.GetNodeAt(targetKey);
        CheckTargetTrailDir(targetNode);
    }
    private void CheckTargetTrailDir(SpatialNode node)
    {
        if (node == null) return;

        To.Value = node;

        Vector2Int[] movabeDir = node.GetTrailDirection();
        foreach (var movabeDirItem in movabeDir)
        {
            if (_forwardDir == -movabeDirItem) _canMove = true;
        }
    }
}

