public class GoFromState : PlayerState
{
    public GoFromState(PlayerController player, IState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        _player.Anim.SetFloat(Defines.ANIM_SPEED_HASH, -1.0f);
        _player.Anim.CrossFade(Defines.JUMP_HASH, 0.1f, 0, 1.0f);
    }
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit()
    {
        _player.Anim.SetFloat(Defines.ANIM_SPEED_HASH, 1.0f);
    }
}
