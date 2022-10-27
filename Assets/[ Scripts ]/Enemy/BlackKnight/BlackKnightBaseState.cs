using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackKnightBaseState : State
{
    protected BlackKnightStateMachine stateMachine;

    protected BlackKnightBaseState(BlackKnightStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
}
