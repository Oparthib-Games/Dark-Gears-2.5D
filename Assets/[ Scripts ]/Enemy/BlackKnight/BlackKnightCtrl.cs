using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackKnight
{
    public class BlackKnightCtrl : StateMachine
    {
        [SerializeField]
        public GameObject target;

        public float moveSpeed = 0f;
        public float maxMoveSpeed = 1.7f;
        public float moveSpeedIncreaseRate = 1f;
        public float minAttackDistance = 1f;

        public Vector3 targetPosition;
        public Vector3 targetPositionLand;
        public float disFromTarget;
        public float disFromTargetLand;
        public Vector3 dirToTarget;
        public Vector3 dirToTargetLand;

        public Rigidbody RB;
        public Animator Anim;
        public CapsuleCollider Collider;

        void Start()
        {
            RB = GetComponent<Rigidbody>();
            Anim = GetComponent<Animator>();
            Collider = GetComponent<CapsuleCollider>();

            SwitchState(new IdleState(this));
        }

        void Update()
        {
            currState.Tick(Time.deltaTime);
        }


        // TODO: A function for reducing speed to 0
        // TODO: A function for increasing speed to max
    }

}