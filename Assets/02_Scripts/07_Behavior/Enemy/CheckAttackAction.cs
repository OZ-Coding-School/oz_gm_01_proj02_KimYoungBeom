using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckAttack", story: "Check Attack [My]", category: "Action", id: "c5192858cc61f616df7bb06b9181bd7c")]
public partial class CheckAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Piece_Enemy> My;
    private SpatialNode _groundNode;
    private Vector2Int _forwardDir;
    private SpatialNode _notifyNode;

    private bool _canAttack;
    protected override Status OnStart()
    {
        _groundNode = My.Value.GroundNode;
        _forwardDir = My.Value.ForwardDir;
        _notifyNode = My.Value.NotifyNode;
        _canAttack = false;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //top view
        if (Managers.Camera.CurrentViewMode == EViewMode.Top)
        {
            Vector2Int targetKey = _groundNode.GridCoordinate + _forwardDir;
            CheckAttack(targetKey);
        }
        //Quarter view
        else
        {
            var worldCoord = _groundNode.WorldCoordinate;
            Vector3Int targetKey = worldCoord + new Vector3Int(_forwardDir.x, worldCoord.y, _forwardDir.y);
            CheckAttack(targetKey);
        }
        return _canAttack ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
    }

    private void CheckAttack(Vector2Int targetKey)
    {
        if (targetKey.x > 0 || targetKey.y < 0) return;
        var targetNode = Managers.Stage.GetNodeAt(targetKey);

        CheckTargetNode(targetNode);
    }
    private void CheckAttack(Vector3Int targetKey)
    {
        if (targetKey.x > 0 || targetKey.z < 0) return;
        var targetNode = Managers.Stage.GetNodeAt(targetKey);

        CheckTargetNode(targetNode);
    }
    private void CheckTargetNode(SpatialNode targetNode)
    {
        if (targetNode == null) return;

        if (targetNode == _notifyNode)
        {
            _canAttack = true;
        }
    }
}

