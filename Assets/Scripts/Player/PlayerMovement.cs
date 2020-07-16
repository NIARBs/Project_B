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
    private float moveVelocity;

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
        moveVelocity = rigid.velocity.x + Input.GetAxisRaw("Horizontal");

        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.001f)
        {
            Debug.Log("deaccel");
            moveVelocity *= Mathf.Pow(deacceleration, Time.deltaTime * 10f);
        }
        else if(Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(moveVelocity))
        {
            Debug.Log("Turn");
            moveVelocity *= Mathf.Pow(turnSpeed, Time.deltaTime * 10f);
        }
        else
        {
            Debug.Log("accel");
            moveVelocity *= Mathf.Pow(maxSpeed * acceleration, Time.deltaTime * 10f);
        }

        rigid.velocity = new Vector2(moveVelocity, rigid.velocity.y);
    }

    private void Jump()
    {
        if(currentState == MovementState.OnGround && Input.GetButtonDown("Jump"))
        {
            currentState = MovementState.Jumping;
            jumpTimeCounter = jumpTime;

            rigid.velocity = new Vector2(moveVelocity, jumpPower);
        }

        if(currentState == MovementState.Jumping)
        {
            if(Input.GetButton("Jump"))
            {
                if(jumpTimeCounter > 0)
                {
                    rigid.velocity = new Vector2(moveVelocity, jumpPower);
                    jumpTimeCounter -= Time.deltaTime;
                }
            }
            else
            {
                currentState = MovementState.Falling;
            }
        }

        Debug.Log(rigid.velocity);
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
