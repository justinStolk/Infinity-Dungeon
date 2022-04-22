using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private BaseState currentState;
    private Dictionary<System.Type, BaseState> states = new();

    public FSM(System.Type startingState, params BaseState[] allStates)
    {
        foreach(BaseState state in allStates)
        {
            state.Initialize(this);
            states.Add(state.GetType(), state);
        }
        SwitchState(startingState);
    }
    public void OnStateMachineUpdate()
    {
        currentState?.OnStateUpdate();
    }

    public void SwitchState(System.Type newState)
    {
        currentState?.OnStateExit();
        currentState = states[newState];
        currentState?.OnStateEnter();
    }




}
