public class GoToState : PlayerState
{
    public GoToState(PlayerController player, IState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        _player.Anim.CrossFade(Defines.JUMP_HASH, 0.1f);
    }
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
