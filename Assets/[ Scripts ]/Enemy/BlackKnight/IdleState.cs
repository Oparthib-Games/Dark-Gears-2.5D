using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackKnight
{
    public class IdleState : State
    {
        BlackKnightCtrl CTRL;

        public IdleState(BlackKnightCtrl CTRL)
        {
            this.name = "Idle";
            this.CTRL = CTRL;
        }

        public override void Enter()
        {
            Debug.Log("Idle State: Start");
        }

        public override void Tick(float delta)
        {
            Debug.Log("Idle State: Tick");

            CTRL.Anim.CrossFade("Locomotion", 0.12f, -1);
            if (CTRL.moveSpeed > 0)
            {
                CTRL.moveSpeed -= CTRL.moveSpeedIncreaseRate * Time.deltaTime;
                CTRL.transform.position = Vector3.MoveTowards(CTRL.transform.position, CTRL.targetPositionLand, CTRL.moveSpeed * Time.deltaTime);
            }

            CTRL.Anim.SetFloat("Horizontal", CTRL.moveSpeed / CTRL.maxMoveSpeed);
        }

        public override void Exit()
        {
            Debug.Log("Idle State: Exit");
        }
    }
}