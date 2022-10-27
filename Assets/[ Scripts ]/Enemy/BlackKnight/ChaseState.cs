using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackKnight
{
    public class ChaseState : State
    {
        BlackKnightCtrl CTRL;

        public ChaseState(BlackKnightCtrl CTRL)
        {
            this.name = "Idle";
            this.CTRL = CTRL;
        }

        public override void Enter()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void Tick(float delta)
        {
            throw new System.NotImplementedException();
        }
    }
}