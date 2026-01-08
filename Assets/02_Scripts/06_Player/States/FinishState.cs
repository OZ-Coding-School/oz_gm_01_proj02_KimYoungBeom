public class FinishState : PlayerState
{
    public FinishState(PlayerController player, IState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        _elapsedTimeBase = 0.0f;
        Utils.Log("Finish state Enter");
        _player.Anim.CrossFade(Defines.VICTORY_IDLE_HASH, 0.1f);
    }
    public override void Update() { }
    public override void FixedUpdate()
    {

    }
    public override void Exit() { }
}
