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
        TouchingWall,
        WallSliding,
        Falling
    }

    private bool canMove = true;
    public bool isFacingRight = true;
    private int facingDirection = 1;

    [Header("컴포넌트")]
    public Animator animator;
    [SerializeField] private Transform wallCheck;
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
    private float moveVelocityX;
    private float moveVelocityY;
    [SerializeField] private float checkFeetRadius;

    [Header("점프")]
    [SerializeField] private float jumpPower;
    private float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("벽점프")]
    [SerializeField] private float checkWallDistance;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallJumpPower;
    [SerializeField] private Vector2 wallJumpDirection;

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
        if(canMove)
        {
            Move(direction.x);
        }

        ModifyPhysics();

        if (jumpTimer > Time.time && currentState == MovementState.OnGround)
        {
            Jump();
        }
        else if(jumpTimer > Time.time && (currentState == MovementState.WallSliding || currentState == MovementState.TouchingWall))
        {
            WallJump();
        }
    }

    private void Move(float horizontal)
    {
        moveVelocityX = rigid.velocity.x + horizontal;
        moveVelocityY = rigid.velocity.y;

        if(currentState == MovementState.WallSliding)
        {
            if(rigid.velocity.y < -wallSlideSpeed)
            {
                moveVelocityY = -wallSlideSpeed;
            }
        }

        if((moveVelocityX < 0 && isFacingRight) || (moveVelocityX > 0 && !isFacingRight))
        {
            Flip();
        }

        if(Mathf.Abs(horizontal) < 0.001f)
        {
            //Debug.Log("[MoveStateChange] Deaccel");
            moveVelocityX *= Mathf.Pow(deacceleration, Time.deltaTime * 10f);
        }
        else if(Mathf.Sign(horizontal) != Mathf.Sign(moveVelocityX))
        {
            //Debug.Log("[MoveStateChange] Turn");
            moveVelocityX *= Mathf.Pow(turnSpeed, Time.deltaTime * 10f);
        }
        else
        {
            //Debug.Log("[MoveStateChange] Accel");
            moveVelocityX *= Mathf.Pow(moveSpeed * acceleration, Time.deltaTime * 10f);
        }

        rigid.velocity = new Vector2(moveVelocityX, moveVelocityY);

        

        Debug.Log(rigid.velocity);

        // 애니메이션 준비
        // animator.SetFloat("move", Mathf.Abs(rb.velocity.x));
    }

    private void ModifyPhysics()
    {
        if(currentState == MovementState.OnGround)
        {
            rigid.gravityScale = 0f;

            return;
        }

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

        Debug.Log("Change Gravity: " + rigid.gravityScale);
    }

    private void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        jumpTimer = 0f;
    }

    private void WallJump()
    {
        Debug.Log("벽점");
        Vector2 force = new Vector2(wallJumpPower * wallJumpDirection.x * -facingDirection, wallJumpPower * wallJumpDirection.y);
        rigid.velocity = Vector2.zero;
        rigid.AddForce(force, ForceMode2D.Impulse);
        StartCoroutine("StopMove");
    }

    IEnumerator StopMove()
    {
        canMove = false;

        yield return new WaitForSeconds(0.3f);

        canMove = true;
    }

    private void CheckMovementState()
    {
        if(Physics2D.OverlapCircle(feetPos.position, checkFeetRadius, groundLayer))
        {
            currentState = MovementState.OnGround;
        }
        else if ((currentState == MovementState.TouchingWall || currentState == MovementState.WallSliding) &&
                currentState != MovementState.OnGround && rigid.velocity.y < 0 && direction.x != 0)
        {
            currentState = MovementState.WallSliding;
        }
        else if (currentState != MovementState.OnGround &&
                Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, checkWallDistance, groundLayer))
        {
            currentState = MovementState.TouchingWall;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(isFacingRight)
        {
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + checkWallDistance, wallCheck.position.y, wallCheck.position.z));
        }
        else
        {
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x - checkWallDistance, wallCheck.position.y, wallCheck.position.z));
        }
    }

    private void Flip()
    {
        // 슬라이딩 중에 반대 방향으로 이동할 경우 위치 변경
        if(currentState == MovementState.WallSliding)
        {
            currentState = MovementState.Falling;
        }

        facingDirection *= -1;
        isFacingRight = !isFacingRight;
    }
}
