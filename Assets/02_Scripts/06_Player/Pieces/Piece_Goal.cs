using UnityEngine;

public class Piece_Goal : Piece_Base
{
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }

    protected override void HandleNotify(SpatialNode node)
    {
        if (node.NodeState != _state) return;
        if ((transform.position - node.WorldCoordinate).sqrMagnitude > 0.1f) return;

        ReturnPool();
    }
}
