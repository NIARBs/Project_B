using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.UIElements;
using System.Security.Cryptography;

public class Movement : MonoBehaviour
{
    protected Animator m_Anim;
    private StateMachine m_FSM;

    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;

    public GameObject player;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float acceleration = 20;
    public float jumpForce = 50;
    public float wallJumpForce = 50;
    public float attackJumpForce = 15;
    public float wallJumpControllSpeed = 30;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
    
    public Vector2 wallJumpDir;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    [Space]
    [SerializeField]
    float fHorizontalAcceleration = 1;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingBasic = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;

    [SerializeField] float knockBackRange = 6.0f;
    [SerializeField] float restTime = 2.0f;

    private float accAccleration = 0;

    #region state
    private const int IDLE = 0;
    private const int WALK = 1;
    private const int JUMP = 2;
    private const int JUMP_SECOND = 3;
    private const int WALL_GRAB = 5;
    private const int WALL_JUMP = 6;
    private const int ATTACK_JUMP = 7;

    #endregion

    private void Awake()
    {
        m_FSM = new StateMachine();

        m_FSM.SetCallback(IDLE, stIdle, stIdleBegin, null);
        m_FSM.SetCallback(WALK, stWalk, stWalkBegin, stWalkEnd);
        m_FSM.SetCallback(JUMP, stJump, stJumpBegin, stJumpEnd);
        m_FSM.SetCallback(WALL_GRAB, stWallGrab, stWallGrabBegin, stWallGrabEnd);
        m_FSM.SetCallback(WALL_JUMP, stWallJump, stWallJumpBegin, stWallJumpEnd);
        m_FSM.SetCallback(ATTACK_JUMP, stJump, stAttackJumpBegin, stJumpEnd);
    }

    void Start()
    {
        m_Anim = player.transform.GetComponent<Animator>();
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();

        m_FSM.changeState(IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        m_FSM.Update();

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

    }

    private void stIdleBegin()
    {
        m_Anim.SetFloat("Move", 0);

        wallJumped = false;
        GetComponent<BetterJumping>().enabled = true;
    }

    private void stIdle()
    {
        float moveAxis = Input.GetAxisRaw("Horizontal");
        if (moveAxis != 0)
        {
            m_FSM.changeState(WALK);
        }

        if (Input.GetButtonDown("Jump"))
        {
            m_FSM.changeState(JUMP);
        }
    }

    private void stWalkBegin()
    {
        accAccleration = 0;

        float moveAxis = Input.GetAxisRaw("Horizontal");
        float moveAbs = Mathf.Abs(moveAxis);

        m_Anim.SetFloat("Move", moveAbs);
    }

    private void stWalk()
    {
        if (m_FSM.curState != WALK)
            return;

        float moveAxis = Input.GetAxisRaw("Horizontal");
        float moveAbs = Mathf.Abs(moveAxis);

        if (moveAxis == 0)
        {
            m_FSM.changeState(IDLE);
            return;
        }

        if (Input.GetButtonDown("Jump") && coll.onGround)
        {
            m_FSM.changeState(JUMP);
            return;
        }

        if ((coll.onLeftWall && moveAxis < 0) || (coll.onRightWall && moveAxis > 0))
        {
            return;
        }

        if (moveAxis == 0)
        {
            accAccleration = 0;
        }
        else
        {
            if (player.transform.localScale.x == moveAxis)
            {
                accAccleration = 0;
            }

            player.transform.localScale = new Vector3(-1 * moveAxis, 1, 1);
        }

        accAccleration += moveAxis * acceleration * Time.deltaTime;

        float fHorizontalVelocity = rb.velocity.x;
        fHorizontalVelocity += accAccleration;

        if (moveAbs < 0.01f)
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
        else if (Mathf.Sign(moveAxis) != Mathf.Sign(fHorizontalVelocity))
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
        else
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

        if (fHorizontalVelocity >= speed)
        {
            fHorizontalVelocity = speed;
        }
        else if (fHorizontalVelocity <= -speed)
        {
            fHorizontalVelocity = -speed;
        }

        rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);

    }

    void stWalkEnd()
    {
        m_Anim.SetFloat("Move", 0);
    }

    void stJumpBegin()
    {
        m_Anim.SetBool("Jump", true);

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;

        coll.onGround = false;
        GetComponent<BetterJumping>().enabled = true;
    }

    void stJump()
    {
        if (true == coll.onGround)
        {
            m_FSM.changeState(IDLE);
            return;
        }

        if (coll.onWall && rb.velocity.y < 0)
        {
            m_FSM.changeState(WALL_GRAB);
            return;
        }

        float moveAxis = Input.GetAxisRaw("Horizontal");
        float moveAbs = Mathf.Abs(moveAxis);

        if ((coll.onLeftWall && moveAxis < 0) || (coll.onRightWall && moveAxis > 0))
        {
            return;
        }

        if (moveAxis == 0)
        {
            accAccleration = 0;
        }
        else
        {
            if (player.transform.localScale.x == moveAxis)
            {
                accAccleration = 0;
            }

            player.transform.localScale = new Vector3(-1 * moveAxis, 1, 1);
        }

        accAccleration += moveAxis * acceleration * Time.deltaTime;

        float fHorizontalVelocity = rb.velocity.x;
        fHorizontalVelocity += accAccleration;

        if (moveAbs < 0.01f)
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
        else if (Mathf.Sign(moveAxis) != Mathf.Sign(fHorizontalVelocity))
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
        else
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

        if (fHorizontalVelocity >= speed)
        {
            fHorizontalVelocity = speed;
        }
        else if (fHorizontalVelocity <= -speed)
        {
            fHorizontalVelocity = -speed;
        }

        rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);

    }

    void stJumpEnd()
    {
        m_Anim.SetBool("Jump", false);
    }

    void stJumpSecond()
    {
        if (true == coll.onGround)
        {
            m_FSM.changeState(IDLE);
            return;
        }

        if (coll.onWall && rb.velocity.y < 0)
        {
            m_FSM.changeState(WALL_GRAB);
            return;
        }

        float moveAxis = Input.GetAxisRaw("Horizontal");
        float moveAbs = Mathf.Abs(moveAxis);

        if (moveAxis == 0)
        {
            accAccleration = 0;
        }
        else
        {
            if (player.transform.localScale.x == moveAxis)
            {
                accAccleration = 0;
            }

            player.transform.localScale = new Vector3(-1 * moveAxis, 1, 1);
        }

        accAccleration += moveAxis * acceleration * Time.deltaTime;

        float fHorizontalVelocity = rb.velocity.x;
        fHorizontalVelocity += accAccleration;

        if (moveAbs < 0.01f)
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
        else if (Mathf.Sign(moveAxis) != Mathf.Sign(fHorizontalVelocity))
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
        else
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

        if (fHorizontalVelocity >= speed)
        {
            fHorizontalVelocity = speed;
        }
        else if (fHorizontalVelocity <= -speed)
        {
            fHorizontalVelocity = -speed;
        }

        rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);
    }

    void stWallGrabBegin()
    {
        m_Anim.SetBool("WallGrab", true);
    }

    void stWallGrab()
    {
        if (true == coll.onGround)
        {
            m_FSM.changeState(IDLE);
            return;
        }

        if(Input.GetButtonDown("Jump"))
        {
            m_FSM.changeState(WALL_JUMP);
            return;
        }

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
         float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    void stWallGrabEnd()
    {
        m_Anim.SetBool("WallGrab", false);
    }

    void stWallJumpBegin()
    {
        m_Anim.SetBool("WallJump", true);

        Vector2 wallDir = coll.onRightWall ? new Vector2(-wallJumpDir.x, wallJumpDir.y) : wallJumpDir;

        rb.velocity = new Vector2(0, 0);
        rb.velocity += wallDir.normalized * wallJumpForce;
    }

    void stWallJump()
    {
        if (true == coll.onGround)
        {
            m_FSM.changeState(IDLE);
            return;
        }

        if (coll.onWall && rb.velocity.y < 0)
        {
            m_FSM.changeState(WALL_GRAB);
            return;
        }

        float moveAxis = Input.GetAxisRaw("Horizontal");
        if (moveAxis != 0)
        {
            player.transform.localScale = new Vector3(-1 * moveAxis, 1, 1);
        }

        rb.velocity = new Vector2(rb.velocity.x + moveAxis * wallJumpControllSpeed * Time.deltaTime, rb.velocity.y);
    }

    void stWallJumpEnd()
    {
        m_Anim.SetBool("WallJump", false);
    }

    void stAttackJumpBegin()
    {
        m_Anim.SetBool("Jump", true);

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * attackJumpForce;

        coll.onGround = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Finish")
        {
            GameManager.GetInstance().NextStage();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rb.velocity.y <= 0 && transform.position.y + 1 > collision.transform.position.y)
            {
                TargetAttack(collision.transform);
            }
            else
            {
                //OnDamaged(collision.transform);
            }
        }
    }

    void OnDamaged(Transform enemy)
    {
        Enemy enemyObject = enemy.GetComponent<Enemy>();
        int damage = enemyObject.GetDamage();
        GameManager.GetInstance().DecreaseHP(damage);

        // 10번째 레이어는 PlayerDamaged
        gameObject.layer = 10;

        int knockBackDir = transform.position.x - enemy.transform.position.x > 0 ? 1 : -1;
        rb.AddForce(new Vector2(knockBackDir, 1) * knockBackRange, ForceMode2D.Impulse);

        // TODO: 공격받은 애니메이션 추가

        Invoke("OnRest", restTime);
    }

    void OnRest()
    {
        // 9번째 레이어는 Player
        gameObject.layer = 9;
    }

    void TargetAttack(Transform enemy)
    {
        Enemy enemyObject = enemy.GetComponent<Enemy>();
        enemyObject.OnDamaged();

        m_FSM.changeState(ATTACK_JUMP);
    }
}
