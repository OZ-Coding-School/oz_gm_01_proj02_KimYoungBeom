using DG.Tweening;
using Unity.Behavior;
using UnityEngine;

public class MovingSpatialNode : SpatialNode, IStageMovable
{
    [SerializeField] private BehaviorGraphAgent _behaviorAgent;

    private AwaitableCompletionSource _turnCompletionSource;

    public override void OnSpawn()
    {
        base.OnSpawn();
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
    }
    public async Awaitable ExecuteStageTurn()
    {
        //이동로직 BT로?
        if (_behaviorAgent == null) return;

        _turnCompletionSource = new AwaitableCompletionSource();
        await _turnCompletionSource.Awaitable;
    }

    public void CompleteTurn()
    {
        if (_turnCompletionSource != null)
        {
            _turnCompletionSource?.SetResult();
            _turnCompletionSource = null;
        }
    }

}
