using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float moveSpeed = 0f;
    [SerializeField]
    private float maxMoveSpeed = 1f;

    Rigidbody RB;
    Animator Anim;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        GoCloseToPlayer();
    }

    private void GoCloseToPlayer()
    {
        Vector3 target_position = transform.position;
        target_position.x = player.transform.position.x;
        //transform.position = Vector3.Lerp(transform.position, target_position, moveSpeed * Time.deltaTime);


        Anim.SetFloat("Horizontal", moveSpeed / maxMoveSpeed);
    }
}
