using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State currState;

    [SerializeField]
    protected string currStateName;

    public void SwitchState(State newState)
    {
        currState?.Exit();
        currState = newState;
        currStateName = newState.name;
        currState?.Enter();
    }

    private void Update()
    {
        currState?.Tick(Time.deltaTime);
    }
}
