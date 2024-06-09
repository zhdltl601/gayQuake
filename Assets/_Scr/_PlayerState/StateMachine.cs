using System;
using System.Collections.Generic;
public class StateMachine<stateEnum> where stateEnum : Enum
{
    public State CurrentState { get; protected set; }
    public Dictionary<stateEnum, State> StateDictionary { get; } //can replace T to PlayerState(State)

    //singleton needs rework
    //singleton will automaticaly make instance when not initialized
    //identifer "Instance" will change to property

    //? need to use generic manualy to get Instance
    public static StateMachine<stateEnum> Instance { get; private set; }
    public StateMachine()
    {
        Instance = this;// will be reworked
        StateDictionary = new Dictionary<stateEnum, State>();
    }
    public void AddState(stateEnum type, State instance)
    {
        StateDictionary.Add(type, instance);
    }
    public void Initialize(stateEnum state)
    {
        CurrentState = StateDictionary[state];
    }
    public void ChangeState(stateEnum type)
    {
        CurrentState.Exit();
        //CurrentState.Current might need to change?
        CurrentState = StateDictionary[type];
        CurrentState.Enter();
    }
    public void UpdateState()
    {
        CurrentState.Current?.Invoke();
    }

}
