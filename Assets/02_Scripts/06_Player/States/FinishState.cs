using UnityEngine;

public class FinishState : PlayerState
{
    public FinishState(PlayerController player, IState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        _elapsedTimeBase = 0.0f;
        _player.Anim.CrossFade(Defines.VICTORY_IDLE_HASH, 0.1f);
    }
    public override void Update() { }
    public override void FixedUpdate()
    {
        _elapsedTimeBase += Time.fixedDeltaTime;
        if (_elapsedTimeBase > _player.GetClipLength(Defines.VICTORY_IDLE_HASH))
        {
            _elapsedTimeBase = 0.0f;
            _player.OnStageClear?.Raised();
        }
    }
    public override void Exit()
    {
        _elapsedTimeBase = 0.0f;
    }
}
