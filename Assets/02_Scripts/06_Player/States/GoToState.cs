public class GoToState : PlayerState
{
    public GoToState(PlayerController player, IState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        _player.Anim.Play(Defines.JUMP_HASH);
    }
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
