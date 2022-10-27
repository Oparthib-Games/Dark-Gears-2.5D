using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackKnight
{
    public class AttackState : State
    {
        BlackKnightCtrl CTRL;

        public AttackState(BlackKnightCtrl CTRL)
        {
            this.name = "Attack";
            this.CTRL = CTRL;
        }

        public override void Enter()
        {
            Debug.Log("Attack State: Start");
        }

        public override void Tick(float delta)
        {
            Debug.Log("Attack State: Tick");
        }

        public override void Exit()
        {
            Debug.Log("Attack State: Exit");
        }
    }
}