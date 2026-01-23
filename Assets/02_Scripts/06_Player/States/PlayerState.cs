
public abstract class PlayerState : IState
{
    protected readonly PlayerController _player;
    protected float _elapsedTimeBase = 0f;
    public IState Parent { get; }

    public PlayerState(PlayerController player, IState parent = null)
    {
        _player = player;
        Parent = parent;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }

}
