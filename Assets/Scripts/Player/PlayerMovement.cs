using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        OnGround,
        Jumping,
        Falling
    }

    public MovementState currentState = MovementState.OnGround;

    [Range(0, 100)]
    [SerializeField] private float maxSpeed;
    private float acceleration = 0.01f;
    private float deacceleration = 0.3f;
    private float turnSpeed = 0.3f;
    private float moveVelocityX;

    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float checkFeetRadius;
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpTime;
    private float jumpTimeCounter;

    protected Rigidbody2D rigid;

    // Start is called before the first frame update
    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckMovementState();

        Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        moveVelocityX = rigid.velocity.x + Input.GetAxisRaw("Horizontal");

        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.001f)
        {
            Debug.Log("deaccel");
            moveVelocityX *= Mathf.Pow(deacceleration, Time.deltaTime * 10f);
        }
        else if(Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(moveVelocityX))
        {
            Debug.Log("Turn");
            moveVelocityX *= Mathf.Pow(turnSpeed, Time.deltaTime * 10f);
        }
        else
        {
            Debug.Log("accel");

            moveVelocityX *= Mathf.Pow(maxSpeed * acceleration, Time.deltaTime * 10f);
            Debug.Log("maxSpeed : " + maxSpeed);
            Debug.Log("acceleration : " + acceleration);
            Debug.Log("deltaTime : " + (Time.deltaTime * 10f));
            Debug.Log("moveVelocityX : " + moveVelocityX);
        }

        rigid.velocity = new Vector2(moveVelocityX, rigid.velocity.y);

        Debug.Log(rigid.velocity);
    }

    private void Jump()
    {
        if(currentState == MovementState.OnGround && Input.GetButtonDown("Jump"))
        {
            currentState = MovementState.Jumping;
            jumpTimeCounter = jumpTime;

            rigid.velocity = new Vector2(moveVelocityX, jumpPower);
        }

        if(currentState == MovementState.Jumping && Input.GetButton("Jump"))
        {
            if(jumpTimeCounter > 0)
            {
                rigid.velocity = new Vector2(moveVelocityX, jumpPower);
                jumpTimeCounter -= Time.deltaTime;
            }
                
            {
                currentState = MovementState.Falling;
            }
        }
        
        if(Input.GetButtonUp("Jump"))
        {
            currentState = MovementState.Falling;
        }
    }

    private void CheckMovementState()
    {
        if(Physics2D.OverlapCircle(feetPos.position, checkFeetRadius, groundMask))
        {
            currentState = MovementState.OnGround;
        }
        else if(rigid.velocity.y <= 0)
        {
            currentState = MovementState.Falling;
        }
        else
        {
            currentState = MovementState.Jumping;
        }
    }
}
