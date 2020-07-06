using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        OnGround,
        Jumping
    }

    private MovementState currentState = MovementState.OnGround;

    [Range(0, 100)]
    [SerializeField] private float maxSpeed;
    private float acceleration = 0.01f;
    private float deacceleration = 0.3f;
    private float turnSpeed = 0.3f;

    [SerializeField] private float jumpPower;
    private float jumpTimer = 0.0f;
    private float jumpTimeLimit = 0.1f;

    protected Rigidbody2D rigid;

    // Start is called before the first frame update
    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float moveVelocity = rigid.velocity.x;
        moveVelocity += Input.GetAxisRaw("Horizontal");

        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.001f)
        {
            moveVelocity *= Mathf.Pow(deacceleration, Time.deltaTime * 10f);
        }
        else if(Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(moveVelocity))
        {
            moveVelocity *= Mathf.Pow(turnSpeed, Time.deltaTime * 10f);
        }
        else
        {
            moveVelocity *= Mathf.Pow(maxSpeed * acceleration, Time.deltaTime * 10f);
        }

        rigid.velocity = new Vector2(moveVelocity, rigid.velocity.y);
    }

    private void Jump()
    {
        if(Input.GetButtonDown("Jump"))
        {
            currentState = MovementState.Jumping;
        }

        if(Input.GetButtonUp("Jump"))
        {
            if (rigid.velocity.y > 0)
            {
                currentState = MovementState.OnGround;
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * 0.5f);
            }
        }

        if(Input.GetButton("Jump") == false || jumpTimer >= jumpTimeLimit)
        {

            return;
        }

        //rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * jumpPower * ((jumpTimer * 10) + 1f), ForceMode2D.Impulse);

        jumpTimer += Time.deltaTime;
    }
}
