
using System;
using System.Collections.Generic;
public interface IState
{
    void Enter();
    void Update();
    void FixedUpdate();
    void Exit();
    IState Parent { get; }
}
public class StateMachine
{
    public IState CurrentState { get; private set; }
    private Dictionary<IState, List<Transition>> mTransitionDic = new();
    private List<Transition> mAnyTransitions = new();

    public void Update()
    {
        Transition transition = GetValidTransition();
        if (transition != null)
        {
            ChangeState(transition.To);
        }
        CurrentState?.Update();
    }
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
    public bool IsCurrentState(IState state)
    {
        return CurrentState == state;
    }
    public void ChangeState(IState nextState)
    {
        if (CurrentState != null) CurrentState.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }
    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        if (!mTransitionDic.ContainsKey(from)) mTransitionDic[from] = new List<Transition>();
        mTransitionDic[from].Add(new Transition(to, condition));
    }
    public void AddAnyTransition(IState to, Func<bool> condition)
    {
        mAnyTransitions.Add(new Transition(to, condition));
    }

    private Transition GetValidTransition()
    {
        if (mAnyTransitions.Count != 0 && mAnyTransitions != null)
        {
            foreach (var transition in mAnyTransitions)
            {
                if (transition.Condition.Invoke())
                {
                    return transition;
                }
            }
        }
        IState state = CurrentState;

        while (state != null)
        {
            if (mTransitionDic.ContainsKey(state))
            {
                foreach (var transition in mTransitionDic[state])
                {
                    if (transition.Condition.Invoke())
                    {
                        return transition;
                    }
                }
            }
            state = state.Parent;
        }
        return null;
    }

    private class Transition
    {
        public IState To { get; }
        public Func<bool> Condition { get; }
        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }

    }
}
