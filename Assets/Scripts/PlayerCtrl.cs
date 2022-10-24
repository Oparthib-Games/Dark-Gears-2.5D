using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // ! =========== Speed ===========
    [SerializeField]
    private float moveSpeed = 35f;
    [SerializeField]
    private float turnSpeed = 0.1f;
    [SerializeField]
    private float jumpSpeed = 300f;
    [SerializeField]
    private float wallJumpSpeed = 400f;
    [SerializeField]
    private float jumpFallSpeed = 3.5f;
    [SerializeField]
    private float dashSpeed = 3500f;

    // ! =========== Hanging ===========
    [SerializeField]
    private Vector3 hangPointOffset = new Vector3(0.3f, 0, 0);
    [SerializeField]
    private Vector3 grabPosition;

    // ! =========== Audio ===========
    [Header("AUDIO:")]
    public AudioClip landingAudioClip;
    public AudioClip[] footstepAudioClips;
    [Range(0, 1)]
    public float footstepAudioVolume = 0.5f;

    // ! =========== Move ===========
    [SerializeField]
    private Vector3 moveAmount;
    [SerializeField]
    private Vector3 moveAmountNormalize;
    private int facingDirection = 1;
    private int right = 1;
    private int left = -1;

    // ! =========== Jump ===========
    [SerializeField]
    private bool isJump = false;
    [SerializeField]
    private float jumpTimeout = 0.5f;
    private float jumpTimeoutDelta;
    [SerializeField]
    private bool isGrounded = false;
    [SerializeField]
    private bool isHanging = false;

    // ! =========== Dash ===========
    [SerializeField]
    private bool isDash = false;
    [SerializeField]
    private float dashTimeout = 0.5f;
    private float dashTimeoutDelta;

    [Header("JUST FOR DEBUGGING:")]
    // ! =========== Permission ===========
    [SerializeField]
    private bool canMove = true;
    [SerializeField]
    private bool canRotate = true;
    [SerializeField]
    private bool canHang = false;

    // ! =========== Components ===========
    private CapsuleCollider capsuleCollider;
    private Rigidbody RB;
    private Animator Anim;

    // ! =========== Bullshit ===========
    float currentVelocity;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        MovementHandler();
        RotationHandler();
        DetectFacingDirection();
        DashHandler();
        AnimationHandler();
    }
    private void FixedUpdate()
    {
        JumpHandler();
        WallHanging();
        JumpFromWall();
    }

    public void MovementHandler()
    {
        if (!canMove) return;

        moveAmount = transform.forward * InputHandler.H * Mathf.Sign(InputHandler.H) * Time.deltaTime * moveSpeed;
        //moveAmount.x = Mathf.Abs(moveAmount.x);
        RB.AddForce(moveAmount, ForceMode.Impulse);
    }
    public void RotationHandler()
    {
        if (!canRotate) return;

        Vector3 direction = new Vector3(InputHandler.H, 0, 0).normalized;

        if (direction.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, turnSpeed);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
    }

    public void DetectFacingDirection()
    {
        if (transform.eulerAngles.y > 0 && transform.eulerAngles.y < 180) facingDirection = right;
        else facingDirection = left;
    }

    public void JumpHandler()
    {
        #region Raycasting [to prevent multiple jump]
        int layer_mask = LayerMask.GetMask("Ground", "Floor", "Wall");

        float ray_distance = 0.5f;
        RaycastHit hit;
        Vector3 ray_origin_offset = new Vector3(0, 0.2f, 0);
        Vector3 ray_origin = transform.position + ray_origin_offset;
        Vector3 ray_direction = transform.TransformDirection(Vector3.down);
        if (Physics.Raycast(ray_origin, ray_direction, out hit, ray_distance, layer_mask))
        {
            Debug.DrawRay(ray_origin, ray_direction * hit.distance, Color.red);
            isGrounded = true;
        }
        else
        {
            Debug.DrawRay(ray_origin, ray_direction * ray_distance, Color.green);
            isGrounded = false;
        }
        #endregion

        if(isGrounded)
        {
            if (jumpTimeoutDelta >= 0.0f) jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;
        }

        isJump = InputHandler.isJump && isGrounded && jumpTimeoutDelta <= 0.0f;

        if (isJump) {
            RB.AddForce(Vector3.up * jumpSpeed * Time.deltaTime, ForceMode.Impulse);
            Anim.Play("Jump Up");
        }
        if(!isGrounded && !isHanging && RB.velocity.y < 0) {
            RB.velocity += Vector3.up * Physics.gravity.y * jumpFallSpeed * Time.deltaTime;
        }
    }

    public void WallHanging()
    {
        int layer_mask = LayerMask.GetMask("Wall");

        #region Raycasting [to detect wall jumping]
        if(InputHandler.H != 0)
        {

            float ray_distance = 0.5f;
            RaycastHit hit;
            Vector3 ray_origin_offset = new Vector3(InputHandler.H * 0.1f, 1.5f, 0);
            Vector3 ray_origin = transform.position + ray_origin_offset;
            Vector3 ray_direction = transform.TransformDirection(Vector3.forward);
            if (Physics.Raycast(ray_origin, ray_direction, out hit, ray_distance, layer_mask) && InputHandler.isAttack)
            {
                Debug.DrawRay(ray_origin, ray_direction * hit.distance, Color.red);
                canHang = true;
            }
            else
            {
                Debug.DrawRay(ray_origin, ray_direction * ray_distance, Color.green);
                if(!isHanging) canHang = false;
            }

            isHanging = canHang && !isGrounded;
        }
        #endregion

        #region Raycasting [to detect wall jumping while already hanging]
        if (isHanging)
        {
            float ray_distance2 = 1f;
            RaycastHit hit2;
            Vector3 ray_origin2 = transform.position;
            Vector3 ray_direction2 = transform.TransformDirection(Vector3.forward);
            if (Physics.Raycast(ray_origin2, ray_direction2, out hit2, ray_distance2, layer_mask))
            {
                Debug.DrawRay(ray_origin2, ray_direction2 * hit2.distance, Color.red);
                grabPosition = hit2.point - hangPointOffset * facingDirection;
                //grabPosition = new Vector3(transform.position.x, hit2.point.y, transform.position.z);
            }
            else
            {
                Debug.DrawRay(ray_origin2, ray_direction2 * ray_distance2, Color.green);
                isHanging = false;
            }
        }
        #endregion

        if (isHanging)
        {
            canMove = false;
            canRotate = false;
            RB.velocity = Vector3.zero;
            RB.useGravity = false;

            if (transform.position != grabPosition)
            {
                transform.position = Vector3.Lerp(transform.position, grabPosition, 1);
            }
        }
        else
        {
            canMove = true;
            canRotate = true;
            RB.useGravity = true;
        }
    }

    public void JumpFromWall()
    {
        if (isHanging && InputHandler.isJump)
        {
            isHanging = false;
            isJump = true;
            int jumpDirection = facingDirection * -1;
            // if (facingDirection == InputHandler.H) jumpDirection = 0;
            // Debug.Log(facingDirection + " : " + InputHandler.H + " : " + jumpDirection);
            RB.AddForce(new Vector3(jumpDirection, 2, 0) * wallJumpSpeed * Time.deltaTime, ForceMode.Impulse);
        }
    }

    public void AnimationHandler()
    {
        if(canMove)
        {
            moveAmountNormalize = Vector3.Normalize(moveAmount);
            Anim.SetFloat("Horizontal", Mathf.Abs(moveAmountNormalize.x));
            Anim.SetFloat("Vertical", Mathf.Abs(moveAmountNormalize.z));
        }
        Anim.SetBool("isJump", isJump);
        Anim.SetBool("isGrounded", isGrounded);
        Anim.SetBool("isHanging", isHanging);
    }

    public void DashHandler()
    {
        if (dashTimeoutDelta >= 0.0f) dashTimeoutDelta -= Time.deltaTime;

        isDash = InputHandler.isDash && dashTimeoutDelta <= 0.0f;

        if (isDash)
        {
            dashTimeoutDelta = dashTimeout;
            Anim.Play("Dash Forward");
            isDash = false;
        }
    }

    // =====================================
    //           Animation Events
    // =====================================
    private void LandEvent(AnimationEvent animationEvent)
    {
        if(landingAudioClip != null)
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.position, footstepAudioVolume);
    }
    private void FootstepEvent(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.position, footstepAudioVolume);
            }
        }
    }
    private void DashForwardEvent(AnimationEvent animationEvent)
    {
        RB.AddForce(Vector3.right * dashSpeed * facingDirection * Time.deltaTime, ForceMode.Impulse);
    }


    // =====================================
    //           Getters & Setters
    // =====================================
    public void setCanMove(bool val)
    {
        canMove = val;
        moveAmount = Vector3.zero;
    }
    public void setCanRotate(bool val)
    {
        canRotate = val;
    }
}
