using DG.Tweening;
using UnityEngine;

public class Piece_Key : Piece_Base
{
    private Tween _tween;
    private void Awake()
    {
        _bounceAmplitude = 0.1f;
        _rotateHalfDuration = 1.2f;
    }
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }
    public override void OnSpawn()
    {
        base.OnSpawn();
        UpDownIdle();
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        _tween?.Kill();
    }
    protected override void HandleIntroEnd()
    {
    }

    protected override void HandleNotify(SpatialNode node)
    {
        if (node != GroundNode) return;

        Managers.Stage.GetKeyRequest();

        ReturnPool();
    }
    private void UpDownIdle()
    {
        _tween?.Kill();
        _tween = transform.DOMoveY(_bounceAmplitude, _rotateHalfDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetRelative();
    }
}
