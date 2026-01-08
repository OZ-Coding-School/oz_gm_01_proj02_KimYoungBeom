using UnityEngine;

public class Piece_Goal : Piece_Base
{

    protected override void HandleNotify(SpatialNode node)
    {
        if (node.NodeState != _state) return;
        if ((transform.position - node.WorldPosition).sqrMagnitude > 0.1f) return;

        Managers.Pool.Despawn(poolData, this);
    }
}
