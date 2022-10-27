using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    private enum states { IDLE, PATROL, CHASE, COMBO_01, COMBO_02, COMBO_03, COMBO_04 }
    [SerializeField]
    private states state = states.IDLE;

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

    private Vector3 targetPosition;
    private Vector3 targetPositionLand;
    private float disFromTarget;
    private float disFromTargetLand;
    private Vector3 dirToTarget;
    private Vector3 dirToTargetLand;


    Rigidbody RB;
    Animator Anim;
    CapsuleCollider Collider;

    // ! =========== Bullshit ===========
    float currentVelocity;
    
    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        Collider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        Controller();
        if (state == states.IDLE) Idle();
        if(state == states.CHASE) Chanse();
        if(state == states.COMBO_01) Combo01();
    }

    public void Controller()
    {
        if (!target) state = states.IDLE;
        if(target)
        {
            targetPosition = target.transform.position;
            targetPositionLand = transform.position;
            targetPositionLand.x = target.transform.position.x;

            disFromTargetLand = Vector3.Distance(transform.position, targetPositionLand);
            dirToTargetLand = targetPositionLand - transform.position;

            if(CheckIfTargetCloseEnough() || disFromTargetLand <= minAttackDistance)
            {
                state = states.COMBO_01;
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

    private void Idle()
    {
        if (moveSpeed > 0)
        {
            moveSpeed -= moveSpeedIncreaseRate * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPositionLand, moveSpeed * Time.deltaTime);
        }

        Anim.SetFloat("Horizontal", moveSpeed / maxMoveSpeed);
    }

    private void Chanse()
    {
        if (!target) return;

        if (moveSpeed < maxMoveSpeed)
            moveSpeed += moveSpeedIncreaseRate * Time.deltaTime;
        
        transform.position = Vector3.MoveTowards(transform.position, targetPositionLand, moveSpeed * Time.deltaTime);
        Anim.SetFloat("Horizontal", moveSpeed/maxMoveSpeed);


        if (dirToTargetLand.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(dirToTargetLand.x, dirToTargetLand.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
    }

    private void Combo01()
    {
        
    }
}
