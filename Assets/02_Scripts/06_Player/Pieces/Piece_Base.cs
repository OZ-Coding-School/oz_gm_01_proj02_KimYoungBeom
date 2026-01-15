using UnityEngine;

public abstract class Piece_Base : PoolableComponent
{
    [SerializeField] protected SpatialNodeEventCHSO _onNotifySpecialNode;

    public SpatialNode GroundNode { get; private set; }

    public override void OnSpawn()
    {
        _onNotifySpecialNode.onEvent += HandleNotify;
    }
    public override void OnDespawn()
    {
        _onNotifySpecialNode.onEvent -= HandleNotify;
    }
    public virtual void InjectNode(SpatialNode groundNode)
    {
        GroundNode = groundNode;
    }
    protected abstract void HandleNotify(SpatialNode node);
}
