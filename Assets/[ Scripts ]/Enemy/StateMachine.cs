using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State currState;

    public void SwitchState(State newState)
    {
        currState?.Exit();
        currState = newState;
        currState?.Enter();
    }

    private void Update()
    {
        currState?.Tick(Time.deltaTime);
    }
}
