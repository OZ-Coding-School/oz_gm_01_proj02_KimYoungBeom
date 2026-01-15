using UnityEngine;

public class Piece_Goal : Piece_Base
{
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }

    protected override void HandleNotify(SpatialNode node)
    {
        if (node != GroundNode) return;

        Managers.Stage.StageClearRequest();

        ReturnPool();
    }

}
