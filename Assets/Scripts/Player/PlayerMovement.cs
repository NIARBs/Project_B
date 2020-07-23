using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        OnGround,
        Jumping,
        Falling
    }

    [Header("컴포넌트")]
    public Animator animator;
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask groundLayer;

    [Header("움직임")]
    public MovementState currentState = MovementState.OnGround;

    [Range(0, 100)]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 direction;
    private float acceleration = 0.01f;
    private float deacceleration = 0.3f;
    private float turnSpeed = 0.3f;
    private float moveVelocity;
    [SerializeField] private float checkFeetRadius;

    [Header("점프")]
    [SerializeField] private float jumpSpeed;
    private float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("물리")]
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    protected Rigidbody2D rigid;

    // Start is called before the first frame update
    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        CheckMovementState();

        if(Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }
    }

    private void FixedUpdate()
    {
        Move(direction.x);

        if(jumpTimer > Time.time && currentState == MovementState.OnGround)
        {
            Jump();
        }

        ModifyPhysics();
    }

    private void Move(float horizontal)
    {
        moveVelocity = rigid.velocity.x + horizontal;

        if(Mathf.Abs(horizontal) < 0.001f)
        {
            Debug.Log("deaccel");
            moveVelocity *= Mathf.Pow(deacceleration, Time.deltaTime * 10f);
        }
        else if(Mathf.Sign(horizontal) != Mathf.Sign(moveVelocity))
        {
            Debug.Log("Turn");
            moveVelocity *= Mathf.Pow(turnSpeed, Time.deltaTime * 10f);
        }
        else
        {
            Debug.Log("accel");

            moveVelocity *= Mathf.Pow(moveSpeed * acceleration, Time.deltaTime * 10f);
            Debug.Log("maxSpeed : " + moveSpeed);
            Debug.Log("acceleration : " + acceleration);
            Debug.Log("deltaTime : " + (Time.deltaTime * 10f));
        }

        rigid.velocity = new Vector2(moveVelocity, rigid.velocity.y);

        

        Debug.Log(rigid.velocity);

        // 애니메이션 준비
        // animator.SetFloat("move", Mathf.Abs(rb.velocity.x));
    }

    private void ModifyPhysics()
    {
        if(currentState == MovementState.OnGround)
        {
            rigid.gravityScale = 0f;
        }
        else
        {
            rigid.gravityScale = gravity;
            if(rigid.velocity.y < 0)
            {
                rigid.gravityScale = gravity * fallMultiplier;
            }
            else if(rigid.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rigid.gravityScale = gravity * (fallMultiplier * 0.6f);
            }
            else if(rigid.velocity.y > 0 && Input.GetButton("Jump"))
            {
                rigid.gravityScale = gravity * (fallMultiplier * 0.4f);
            }
        }
    }

    private void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0f;
    }

    private void CheckMovementState()
    {
        if(Physics2D.OverlapCircle(feetPos.position, checkFeetRadius, groundLayer))
        {
            currentState = MovementState.OnGround;
        }
        else if(rigid.velocity.y < 0)
        {
            currentState = MovementState.Falling;
        }
        else
        {
            currentState = MovementState.Jumping;
        }
    }
}
