using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackKnightStateMachine : StateMachine
{
    private void Start()
    {
        SwitchState(new OldChaseState(this));
    }
}
