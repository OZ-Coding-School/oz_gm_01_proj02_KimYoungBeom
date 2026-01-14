using DG.Tweening;
using Unity.Behavior;
using UnityEngine;

public class MovingSpatialNode : SpatialNode, IStageMovable
{
    [SerializeField] private BehaviorGraphAgent _behaviorAgent;

    [Header("BT용 이벤트 구독")]
    [SerializeField] private Event_ExecuteStageTurn _onExecuteStageTurn;
    [SerializeField] private Event_ClearStacksRequest _onClearStacksRequest;

    private AwaitableCompletionSource _turnCompletionSource;

    public override void OnSpawn()
    {
        base.OnSpawn();
        _onClearStacksRequest.SendEventMessage();
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        _onClearStacksRequest.SendEventMessage();
    }
    public async Awaitable ExecuteStageTurn()
    {
        //이동로직 BT로?
        if (_behaviorAgent == null) return;

        _turnCompletionSource = new AwaitableCompletionSource();
        _onExecuteStageTurn.SendEventMessage();
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
