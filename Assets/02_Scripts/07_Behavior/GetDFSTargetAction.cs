using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetDFSTargetAction", story: "MovingNode DFS(Patrol) - Find Next Node for Move", category: "Action", id: "c82ea6ff4c630e883a5583d1fe280a60")]
public partial class GetDFSTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<MovingSpatialNode> MovingSpatialNode;
    [SerializeReference] public BlackboardVariable<List<Vector3Int>> VisitedList;
    [SerializeReference] public BlackboardVariable<List<Vector3Int>> PathStack;
    [SerializeReference] public BlackboardVariable<Vector3Int> NextTarget;

    private static readonly Vector3Int[] _directions = {
        Vector3Int.forward, Vector3Int.back, Vector3Int.right, Vector3Int.left
    };

    protected override Status OnUpdate()
    {
        if (MovingSpatialNode.Value == null) return Status.Failure;

        Vector3Int current = MovingSpatialNode.Value.WorldCoordinate;

        if (!VisitedList.Value.Contains(current))
        {
            VisitedList.Value.Add(current);
        }

        for (int i = 0; i < _directions.Length; i++)
        {
            Vector3Int candidate = current + _directions[i];
            if (Managers.Stage.GetNodeAt(candidate) == null && !VisitedList.Value.Contains(candidate))
            {
                PathStack.Value.Add(current);
                NextTarget.Value = candidate;
                return Status.Success;
            }
        }

        int stackCount = PathStack.Value.Count;
        if (stackCount > 0)
        {
            int lastIndex = stackCount - 1;
            NextTarget.Value = PathStack.Value[lastIndex];

            PathStack.Value.RemoveAt(lastIndex);
            return Status.Success;
        }

        VisitedList.Value.Clear();
        return Status.Failure;
    }
}

