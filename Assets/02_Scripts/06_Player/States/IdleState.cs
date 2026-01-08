public class IdleState : PlayerState
{
    public IdleState(PlayerController player, IState parent = null) : base(player, parent) { }

    public override void Enter()
    {

    }
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
