using UnityEngine;

public class Piece_Button : Piece_Base
{
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }

    protected override void HandleIntroEnd()
    {
    }

    protected override void HandleNotify(SpatialNode node)
    {
    }
}
