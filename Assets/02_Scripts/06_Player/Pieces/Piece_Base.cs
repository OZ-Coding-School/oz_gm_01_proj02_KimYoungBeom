using UnityEngine;

public abstract class Piece_Base : PoolableComponent
{
    [SerializeField] protected SpatialNodeEventCHSO _onNotifySpecialNode;
    [SerializeField] protected ENodeState _state;

    public override void OnSpawn()
    {
        _onNotifySpecialNode.onEvent += HandleNotify;
    }
    public override void OnDespawn()
    {
        _onNotifySpecialNode.onEvent -= HandleNotify;
    }

    protected abstract void HandleNotify(SpatialNode node);
}
