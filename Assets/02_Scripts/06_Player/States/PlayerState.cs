
public abstract class PlayerState : IState
{
    protected readonly PlayerController mPlayer;
    protected float mElapsedTimeBase = 0f;
    public IState Parent { get; }

    public PlayerState(PlayerController player, IState parent = null)
    {
        mPlayer = player;
        Parent = parent;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }

}
