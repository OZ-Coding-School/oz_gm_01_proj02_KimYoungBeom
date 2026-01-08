using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player, IState parent = null) : base(player, parent) { }

    private bool _isSadIdle = false;
    public override void Enter()
    {
        _elapsedTimeBase = 0.0f;
        _player.Anim.CrossFade(Defines.IDLE_HASH, 0.1f);
    }
    public override void Update() { }
    public override void FixedUpdate()
    {
        if (!_isSadIdle)
        {
            _elapsedTimeBase += Time.fixedDeltaTime;

            if (_elapsedTimeBase > _player.SadIdleCool)
            {
                _elapsedTimeBase = 0.0f;
                _isSadIdle = true;
                _player.Anim.CrossFade(Defines.SAD_IDLE_HASH, 0.1f);
            }
        }
    }
    public override void Exit()
    {
        _isSadIdle = false;
        _elapsedTimeBase = 0.0f;
    }

}
