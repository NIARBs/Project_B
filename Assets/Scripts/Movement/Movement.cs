using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.UIElements;

public class Movement : MonoBehaviour
{
    protected Animator m_Anim;

    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;

    public GameObject playerBody;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float acceleration = 20;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

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

    void Start()
    {
        m_Anim = this.transform.Find("model").GetComponent<Animator>();
        m_Anim.Play("Player_Idle");
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);

        if(coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }

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

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
                Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround)
                WallJump();
        }

        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (wallGrab || wallSlide || !canMove)
            return;

        if (x > 0)
        {
            side = 1;
        }
        if (x < 0)
        {
            side = -1;
        }
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
    }


    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);

        hasDashed = true;

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        //dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        //dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }

    private void WallSlide()
    {
        if (!canMove)
            return;

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private float accAccleration = 0;
    private void Walk(Vector2 dir)
    {
        float moveAxis = Input.GetAxisRaw("Horizontal");
        float moveAbs = Mathf.Abs(moveAxis);

        if (moveAxis == 0)
        {
            accAccleration = 0;
        }
        else
        {
            if (playerBody.transform.localScale.x == moveAxis)
            {
                accAccleration = 0;
            }

            playerBody.transform.localScale = new Vector3(-1 * moveAxis, 1, 1);
        }

        m_Anim.SetFloat("Move", moveAbs);
        m_Anim.SetBool("jump", true);
        m_Anim.SetBool("sit", true);


        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            accAccleration += moveAxis * acceleration * Time.deltaTime;

            float fHorizontalVelocity = rb.velocity.x;
            fHorizontalVelocity += accAccleration;

            if (moveAbs < 0.01f)
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
            else if (Mathf.Sign(moveAxis) != Mathf.Sign(fHorizontalVelocity))
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
            else
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

            if(fHorizontalVelocity >= speed)
            {
                fHorizontalVelocity = speed;
            }
            else if(fHorizontalVelocity <= -speed)
            {
                fHorizontalVelocity = -speed;
            }


            rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);
            //rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            //rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        m_Anim.Play("Player_Jump");
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log(collision.transform.position.y);
            if (rb.velocity.y < 0 && transform.position.y + 1 > collision.transform.position.y)
            {
                Debug.Log("들어오냐");
                TargetAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        // 10번째 레이어는 PlayerDamaged
        gameObject.layer = 10;

        int knockBackDir = transform.position.x - targetPos.x > 0 ? 1 : -1;
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
        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        Enemy enemyObject = enemy.GetComponent<Enemy>();
        enemyObject.OnDamaged();
    }
}
