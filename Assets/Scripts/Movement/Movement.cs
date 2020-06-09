using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
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
    public float friction = 0.6f;
    
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
    private const int JUMP_UP = 2;
    private const int JUMP_DOWN = 3;
    private const int DASH = 4;
    private const int JUMP_DASH = 5;
    private const int WALL_GRAB = 6;
    private const int WALL_JUMP = 7;
    private const int ATTACK_JUMP = 8;
    private const int JUMP_READY = 9;
    private const int JUMP_END = 10;

    #endregion

    private void Awake()
    {
        m_FSM = new StateMachine();

        m_FSM.SetCallback(IDLE, stIdle, stIdleBegin, null);
        m_FSM.SetCallback(WALK, stWalk, stWalkBegin, stWalkEnd);
        m_FSM.SetCallback(JUMP_UP, stJumpUp, stJumpUpBegin, null);
        m_FSM.SetCallback(JUMP_DOWN, stJumpDown, stJumpDownBegin, stJumpDownEnd);
        m_FSM.SetCallback(JUMP_READY, stJumpReady, stJumpReadyBegin, null);
        m_FSM.SetCallback(JUMP_END, stJumpEndNormal, stJumpEndBegin, stJumpEndEnd);
        //m_FSM.SetCallback(DASH, stDash, stDashBegin, stDashEnd);
        //m_FSM.SetCallback(JUMP_DASH, stJumpDash, stJumpDashBegin, stJumpDashEnd);
        m_FSM.SetCallback(WALL_GRAB, stWallGrab, stWallGrabBegin, stWallGrabEnd);
        m_FSM.SetCallback(WALL_JUMP, stWallJump, stWallJumpBegin, stWallJumpEnd);
        m_FSM.SetCallback(ATTACK_JUMP, stJumpUp, stAttackJumpBegin, stJumpEndEnd);
    }

    void Start()
    {
        m_Anim = player.transform.GetComponent<Animator>();
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();

        m_FSM.changeState(IDLE);
        rb.gravityScale = 3;
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

    }

    #region IDLE

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
            m_FSM.changeState(JUMP_READY);
        }

        MoveCode();
    }

    #endregion

    #region WALK
    private void stWalkBegin()
    {
        accAccleration = 0;

        float moveAxis = Input.GetAxisRaw("Horizontal");
        float moveAbs = Mathf.Abs(moveAxis);

        m_Anim.SetFloat("Move", moveAbs);
    }

    private void MoveCode()
    {
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
            m_FSM.changeState(JUMP_READY);
            return;
        }
        if(Input.GetButtonDown("Fire1") && coll.onGround)
        {
            m_FSM.changeState(DASH);
            return;
        }


        if ((coll.onLeftWall && moveAxis < 0) || (coll.onRightWall && moveAxis > 0))
        {
            return;
        }

        MoveCode();
    }

    void stWalkEnd()
    {
        m_Anim.SetFloat("Move", 0);
    }

    #endregion

    #region JUMP

    #region JUMP_READY

    float accJumpReadyTime = 0;
    public float JumpReadyTime = 0;

    void stJumpReadyBegin()
    {
        accJumpReadyTime = 0;
        m_Anim.SetBool("Jump", true);
    }
    
    void stJumpReady()
    {
        accJumpReadyTime += Time.deltaTime;
        if(accJumpReadyTime < JumpReadyTime)
        {
            MoveCode();
            return;
        }

        m_FSM.changeState(JUMP_UP);
    }

    #endregion

    #region JUMP_UP

    void stJumpUpBegin()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;

        coll.onGround = false;
        GetComponent<BetterJumping>().enabled = true;
    }

    void stJumpUp()
    {
        if (rb.velocity.y < 0)
        {
            m_FSM.changeState(JUMP_DOWN);
            return;
        }

        float moveAxis = Input.GetAxisRaw("Horizontal");

        if ((coll.onLeftWall && moveAxis < 0) || (coll.onRightWall && moveAxis > 0))
        {
            return;
        }

        MoveCode();
    }

    #endregion

    #region JUMP_DOWN

    void stJumpDownBegin()
    {

    }

    void stJumpDown()
    {
        if(coll.onGround)
        {
            m_FSM.changeState(JUMP_END);
            return;
        }

        if (Input.GetButtonDown("Jump") && coll.canJump)
        {
            m_FSM.changeState(JUMP_READY);
            return;
        }

        if (coll.onWall && rb.velocity.y < 0)
        {
            if (Input.GetKey(KeyCode.A) && coll.onLeftWall || Input.GetKey(KeyCode.D) && coll.onRightWall)
            {
                m_FSM.changeState(WALL_GRAB);
            }

            return;
        }
    }

    void stJumpDownEnd()
    {

    }

    #endregion

    #region JUMP_END

    float accJumpEndTime = 0;
    public float JumpEndTime = 0;

    void stJumpEndBegin()
    {
        accJumpEndTime = 0;
    }

    void stJumpEndNormal()
    {
        accJumpEndTime += Time.deltaTime;
        if (accJumpEndTime < JumpEndTime)
            return;

        m_FSM.changeState(IDLE);
    }

    void stJumpEndEnd()
    {
        m_Anim.SetBool("Jump", false);
    }

    #endregion

    #endregion

    //#region DASH
    //private float accTime;

    //void stDashBegin()
    //{
    //    m_Anim.SetBool("WallJump", true);
    //    accTime = 0;
    //    rb.velocity = rb.velocity.normalized * dashSpeed;
    //}

    //void stDash()
    //{
    //    accTime += Time.deltaTime;
    //    if(accTime >= dashTime)
    //    {
    //        m_FSM.changeState(WALK);
    //    }
    //}

    //void stDashEnd()
    //{
    //    accTime = 0;
    //    m_Anim.SetBool("WallJump", false);
    //}

    //bool isDash = false;

    //void stJumpDashBegin()
    //{
    //    m_Anim.SetBool("WallJump", true);

    //    float x = Input.GetAxisRaw("Horizontal");
    //    float y = Input.GetAxisRaw("Vertical");

    //    Vector2 dir = new Vector2(x, y);

    //    rb.velocity = dir.normalized * dashSpeed;

    //    rb.gravityScale = 0;
    //    GetComponent<BetterJumping>().enabled = false;
    //    accTime = 0;
    //    isDash = true;
    //}

    //void RigidbodyDrag(float x)
    //{
    //    rb.drag = x;
    //}

    //void stJumpDash()
    //{
    //    accTime += Time.deltaTime;
    //    if (accTime < dashTime && isDash == false)
    //        return;

    //    if(isDash == false)
    //    {
    //        accTime = 0;
    //        isDash = true;
    //        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);
    //    }

    //    accTime += Time.deltaTime;
    //    if(accTime > 0.3)
    //    {
    //        rb.gravityScale = 3;
    //        GetComponent<BetterJumping>().enabled = true;
    //    }

    //    if (coll.onGround)
    //        m_FSM.changeState(IDLE);
    //}

    //void stJumpDashEnd()
    //{
    //    m_Anim.SetBool("WallJump", false);
        
    //}

    //#endregion

    #region WALL_GRAB

    void stWallGrabBegin()
    {
        m_Anim.SetBool("WallGrab", true);

        if(coll.onRightWall)
        {
            player.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if(coll.onLeftWall)
        {
            player.transform.localScale = new Vector3(1, 1, 1);
        }

        rb.gravityScale = 0;
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

        if(false == coll.onWall || Input.GetButtonDown("Horizontal"))
        {
            m_FSM.changeState(IDLE);
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
        m_Anim.SetBool("Jump", false);
        rb.gravityScale = 3;
    }

    #endregion

    #region WALL_JUMP

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

    #endregion

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

    Vector3 beforePos;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rb.velocity.y < 0)
            {
                Enemy enemyObject = collision.transform.GetComponent<Enemy>();
                enemyObject.OnDamaged();

                m_FSM.changeState(ATTACK_JUMP);
            }
            else
            {
                OnDamaged(collision.transform);
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
