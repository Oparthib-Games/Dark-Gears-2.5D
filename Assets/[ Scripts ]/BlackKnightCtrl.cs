using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackKnightCtrl : MonoBehaviour
{
    protected enum states { IDLE, PATROL, CHASE, ATTACK }
    [SerializeField]
    private states currState = states.IDLE;
    [SerializeField]
    private states oldState;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float moveSpeed = 0;
    [SerializeField]
    private float maxMoveSpeed = 1.7f;
    [SerializeField]
    private float moveSpeedIncreaseRate = 1f;
    [SerializeField]
    private float minAttackDistance = 1f;
    [SerializeField]
    private bool isAttack = false;
    [SerializeField]
    private int comboType = 1;

    private Vector3 targetPosition;
    private Vector3 targetPositionLand;
    private float disFromTarget;
    private float disFromTargetLand;
    private Vector3 dirToTarget;
    private Vector3 dirToTargetLand;


    Rigidbody RB;
    Animator Anim;
    CapsuleCollider Collider;

    // ! =========== Bullshit ===========+
    float currentVelocity;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        Collider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        Manager();

        if (currState == states.IDLE) Idle();
        if (currState == states.CHASE) Chase();
        if (currState == states.ATTACK) Attack();
    }

    public void Manager()
    {
        if (!target) SwitchState(states.IDLE);
        if(target)
        {
            targetPosition = target.transform.position;
            targetPositionLand = transform.position;
            targetPositionLand.x = target.transform.position.x;

            disFromTargetLand = Vector3.Distance(transform.position, targetPositionLand);
            dirToTargetLand = targetPositionLand - transform.position;

            if(CheckIfTargetCloseEnough() || disFromTargetLand <= minAttackDistance)
            {
                SwitchState(states.ATTACK);
            }
            else
            {
                SwitchState(states.CHASE);
            }
        }
    }


    private bool CheckIfTargetCloseEnough()
    {
        int layer_mask = LayerMask.GetMask("Player");
        float ray_distance = minAttackDistance;
        RaycastHit hit;
        Vector3 ray_origin = transform.TransformPoint(Collider.center);
        Vector3 ray_direction = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(ray_origin, ray_direction, out hit, ray_distance, layer_mask))
        {
            Debug.DrawRay(ray_origin, ray_direction * hit.distance, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(ray_origin, ray_direction * ray_distance, Color.green);
            return false;
        }
    }
    private bool CheckAnimState(string[] anim_states)
    {
        AnimatorStateInfo AnimState = Anim.GetCurrentAnimatorStateInfo(0);
        foreach (string anim_state in anim_states)
        {
            if (AnimState.IsName(anim_state)) return true;
        }
        return false;
    }

    private void GoToMaxSpeed()
    {
        // If Attacking animation is playing, don't move
        if (CheckAnimState(new string[] { "Attack 1", "Attack 2", "Attack 3", "Attack Idle" }))
        {
            moveSpeed = 0;
            Debug.Log("WHYYYYYYYYY");
            return;
        }

        if (moveSpeed < maxMoveSpeed)
            moveSpeed += moveSpeedIncreaseRate * Time.deltaTime;
    }
    private void GoToZeroSpeed()
    {
        if (moveSpeed > 0)
            moveSpeed -= moveSpeedIncreaseRate * Time.deltaTime;
    }

    // ======================================================================================================+
    //                                           MINI STATE MACHINE                                          |
    // ======================================================================================================+
    private void SwitchState(states newState)
    {
        if (newState == currState) return;
        Debug.Log(currState + " ------>" + newState);
        oldState = currState;
        ExitOldState();
        currState = newState;
        NewStateStart();
    }
    private void ExitOldState()
    {
        if (oldState == states.ATTACK)
        {
            isAttack = false;
            Anim.SetBool("isAttack", isAttack);
        }
    }
    private void NewStateStart()
    {
        if (currState == states.ATTACK)
        {
            isAttack = true;
            comboType = Random.Range(1, 3);
        }
    }

    // ======================================================================================================+
    //                                            DIFFERENT STATES                                           |
    // ======================================================================================================+

    // =========================+
    //           IDLE           |
    // =========================+
    private void Idle()
    {
        Anim.CrossFade("Locomotion", 0.12f, -1);

        GoToZeroSpeed();

        if(moveSpeed > 0) transform.position = Vector3.MoveTowards(transform.position, targetPositionLand, moveSpeed * Time.deltaTime);

        Anim.SetFloat("Horizontal", moveSpeed / maxMoveSpeed);
    }

    // =========================+
    //           CHASE          |
    // =========================+
    private void Chase()
    {
        if (!target) return;

        GoToMaxSpeed();

        if (moveSpeed > 0) transform.position = Vector3.MoveTowards(transform.position, targetPositionLand, moveSpeed * Time.deltaTime);
        Anim.SetFloat("Horizontal", moveSpeed/maxMoveSpeed);


        // Rotation [Look at target]
        if (dirToTargetLand.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(dirToTargetLand.x, dirToTargetLand.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
    }


    // =========================+
    //           Attack         |
    // =========================+
    private void Attack()
    {
        //if (!CheckAnimState(new string[] { "Attack 1", "Attack 2", "Attack 3"}))
        //    Anim.CrossFade("Attack 1", 0.2f, 0);
        GoToZeroSpeed();
        Anim.SetBool("isAttack", isAttack);
        Anim.SetInteger("comboType", comboType);
    }

    // ======================================================================================================+
    //                                           Animation Events                                            |
    // ======================================================================================================+
    private void AttackEvent(AnimationEvent animationEvent)
    {

    }
    private void ComboEndEvent(AnimationEvent animationEvent)
    {
        comboType = Random.Range(1, 3);
    }
}
