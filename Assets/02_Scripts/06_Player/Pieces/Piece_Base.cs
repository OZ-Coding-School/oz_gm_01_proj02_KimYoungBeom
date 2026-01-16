using DG.Tweening;
using UnityEngine;

public abstract class Piece_Base : PoolableComponent
{
    [SerializeField] protected SpatialNodeEventCHSO _onNotifySpecialNode;   //PlayerController ¹ß¼Û

    protected float _rotateHalfDuration = 1.0f;
    protected float _bounceAmplitude = 0.15f;
    protected Tween _upDownTween;

    public SpatialNode GroundNode { get; private set; }

    public override void OnSpawn()
    {
        _onNotifySpecialNode.onEvent += HandleNotify;
        Managers.Stage.onIntroEnd += HandleIntroEnd;
    }
    public override void OnDespawn()
    {
        _onNotifySpecialNode.onEvent -= HandleNotify;
        Managers.Stage.onIntroEnd -= HandleIntroEnd;
    }
    public virtual void InjectNode(SpatialNode groundNode)
    {
        GroundNode = groundNode;
    }
    protected abstract void HandleNotify(SpatialNode node);
    protected abstract void HandleIntroEnd();
}
