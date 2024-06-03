using System;
using UnityEngine;
public abstract class State //state
{
    public Action Current { get; protected set; }
    public State()
    {
        Current = Enter;
    }
    public virtual void Enter()
    {
        //logic
        Current = Update;
    }
    public virtual void Update()
    {
        //logic
    }

    public virtual void Exit()
    {
        //logic
    }
}
