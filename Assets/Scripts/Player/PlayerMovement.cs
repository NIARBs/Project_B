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

    [Tooltip("게임이 시작될 때 플레이어 캐릭터가 바라보는 방향을 설정합니다.")]
    public bool isFacingRight = false;
    public int facingDirection = 1;

    [Header("컴포넌트")]
    public Animator animator;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask groundLayer;
    protected Rigidbody2D rigid;

    [Header("움직임")]
    public MovementState currentState = MovementState.OnGround;

    [Range(0, 100)]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 direction;
    private bool canMove = true;
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
    private bool isJump;
    private bool isElasticityJump;
    [SerializeField] private float elasticityJumpDelay;
    private float elasticityJumpTimer;

    [Header("벽점프")]
    [SerializeField] private float checkWallDistance;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallJumpPower;
    [SerializeField] private Vector2 wallJumpDirection;

    [Header("물리")]
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("상호작용")]
    [SerializeField] private float knockBackRange;
    [SerializeField] private float restTime;

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

        if (Input.GetButtonDown("Jump"))
        {
            isJump = true;
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

        // 융통성 점프
        if(jumpTimer > Time.time && elasticityJumpTimer > Time.time && isElasticityJump)
        {
            Debug.Log("융통성 점프");
            Jump();
        }
        // 점프
        else if(jumpTimer > Time.time && currentState == MovementState.OnGround)
        {
            Jump();
        }
        // 벽점프
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

        

        //Debug.Log(rigid.velocity);

        animator.SetFloat("Move", Mathf.Abs(moveVelocityX));
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

        //Debug.Log("Change Gravity: " + rigid.gravityScale);
    }

    private void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

        // 점프 후 초기화
        isElasticityJump = false;
        jumpTimer = 0f;
        elasticityJumpTimer = 0f;
    }

    private void WallJump()
    {
        Vector2 force = new Vector2(wallJumpPower * wallJumpDirection.x * -facingDirection, wallJumpPower * wallJumpDirection.y);
        rigid.velocity = Vector2.zero;
        rigid.AddForce(force, ForceMode2D.Impulse);
        StartCoroutine("StopMove");

        // 점프 후 초기화
        isElasticityJump = false;
        jumpTimer = 0f;
        elasticityJumpTimer = 0f;
    }

    IEnumerator StopMove()
    {
        canMove = false;

        yield return new WaitForSeconds(0.3f);

        canMove = true;
    }

    private void CheckMovementState()
    {
        bool isOnGround = Physics2D.OverlapCircle(feetPos.position, checkFeetRadius, groundLayer);
        bool isTouchingWall = Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, checkWallDistance, groundLayer);
        if(isOnGround)
        {
            currentState = MovementState.OnGround;
            animator.SetBool("Jump", false);
            animator.SetBool("Falling", false);
            animator.SetBool("Sliding", false);
        }
        else if((currentState == MovementState.TouchingWall || currentState == MovementState.WallSliding) &&
               currentState != MovementState.OnGround && rigid.velocity.y < 0 && direction.x != 0)
        {
            // 슬라이딩 중에 벽이 사라지면 떨어짐
            if(!isTouchingWall)
            {
                currentState = MovementState.Falling;
                animator.SetBool("Sliding", false);
                animator.SetBool("Falling", true);
                return;
            }

            currentState = MovementState.WallSliding;
            animator.SetBool("Sliding", true);
        }
        else if(currentState != MovementState.OnGround &&
               isTouchingWall)
        {
            currentState = MovementState.TouchingWall;
        }
        else if(isJump)
        {
            isJump = false;
            currentState = MovementState.Jumping;
            animator.SetBool("Jump", true);
        }
        else
        {
            if(currentState == MovementState.OnGround)
            {
                isElasticityJump = true;
                elasticityJumpTimer = Time.time + elasticityJumpDelay;
            }

            currentState = MovementState.Falling;
            animator.SetBool("Falling", true);
        }
    }

    private void Flip()
    {
        // 슬라이딩 중에 반대 방향으로 이동할 경우 위치 변경
        if (currentState == MovementState.WallSliding)
        {
            currentState = MovementState.Falling;
            animator.SetBool("Sliding", false);
            animator.SetBool("Falling", true);
        }

        facingDirection *= -1;
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(facingDirection, transform.localScale.y, transform.localScale.z);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            GameManager.GetInstance().NextStage();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                Enemy enemyObject = collision.transform.GetComponent<Enemy>();
                enemyObject.OnDamaged();
            }
            else
            {
                OnDamaged(collision.transform);
            }
        }
    }

    private void OnDamaged(Transform enemy)
    {
        Enemy enemyObject = enemy.GetComponent<Enemy>();
        int damage = enemyObject.GetDamage();
        GameManager.GetInstance().DecreaseHP(damage);

        // 10번째 레이어는 PlayerDamaged
        gameObject.layer = 10;

        int knockBackDir = transform.position.x - enemy.transform.position.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(knockBackDir, 1) * knockBackRange, ForceMode2D.Impulse);

        // TODO: 공격받은 애니메이션 추가
        Invoke("OnRest", restTime);
    }
    
    private void OnRest()
    {
        // 9번째 레이어는 Player
        gameObject.layer = 9;
    }
}