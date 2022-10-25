using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Vector3 moveAmount;
    [SerializeField]
    private float moveSpeed = 35f;

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
        AnimationHandler();
    }

    private void GoCloseToPlayer()
    {
        moveAmount = player.transform.position * 1 * Time.deltaTime * moveSpeed * RB.mass;
        RB.AddForce(moveAmount, ForceMode.Impulse);
    }

    public void AnimationHandler()
    {
        Vector3 moveAmountNormalize = Vector3.Normalize(moveAmount);
        Anim.SetFloat("Horizontal", Mathf.Abs(moveAmountNormalize.x));
    }
}
