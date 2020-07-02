using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOld : MonoBehaviour
{
    enum PANIC_LV3_STATE
    {
        STOP,
        RUN_LEFT,
        RUN_RIGHT,
        END
    }

    enum PANIC_LV
    {
        Lv1,
        Lv2,
        Lv3,
        Lv_END,
    }

    #region state
    private const int IDLE = 0;
    private const int WALK = 1;
    private const int JUMP = 2;
    private const int ROLL = 3;
    private const int CROUCH = 4;
    private const int CLIMB = 5;
    private const int WallJump = 6;
    #endregion

    #region globalState
    private const int PANIC_LV0 = 0;
    private const int PANIC_LV1 = 1;
    private const int PANIC_LV2 = 2;
    private const int PANIC_LV3 = 3;
    #endregion

    [SerializeField] public Text PanicText;

    [SerializeField] public float Speed = 1;
    [SerializeField] public float MaxSpeed = 10;
    [SerializeField] public float RollSpeed = 0.5f;
    [SerializeField] public float RollTime = 0.5f;
    [SerializeField] public float JumpSpeed = 5;
    [SerializeField] private float panicScore = 0.0f;
    [SerializeField] private float trembleDist = 0.2f;

    private Rigidbody2D _rigid;
    private Vector2 _dir;
    private Vector2 _scale;
    private Vector2 _trembleDir = Vector2.right;

    private float _accRollTime = 0;
    private float _gravity = 0;

    private bool _canWallJump = true;

    StateMachine _stateMachine;
    Animator _anim;

    private void Awake()
    {
        _stateMachine = new StateMachine();

        //_stateMachine.SetCallback(IDLE, StIdle, StIdleBegin, null);
        //_stateMachine.SetCallback(WALK, StWalk, StWalkBegin, null);
        //_stateMachine.SetCallback(JUMP, StJump, StJumpBegin, null);
        //_stateMachine.SetCallback(ROLL, StRoll, StRollBegin, StRollEnd);
        //_stateMachine.SetCallback(CROUCH, StCrouch, StCrouchBegin, StCrouchEnd);
        //_stateMachine.SetCallback(CLIMB, StClimbUpdate, StClimbBegin, StclimbEnd);

        //_stateMachine.SetGlobalCallback(PANIC_LV0, GstPanicLv0, null, null);
        //_stateMachine.SetGlobalCallback(PANIC_LV1, GstPanicLv1, GstPanicLv1Begin, null);
        //_stateMachine.SetGlobalCallback(PANIC_LV2, GstPanicLv2, GstPanicLv2Begin, null);
        //_stateMachine.SetGlobalCallback(PANIC_LV3, GstPanicLv3, GstPanicLv3Begin, null);
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _scale = transform.localScale;
        _gravity = _rigid.gravityScale;

        _anim.Play("Player_Idle");
    }

    // Update is called once per frame
    void Update()
    {
        // 속도
        if (_rigid.velocity.x > MaxSpeed)
            _rigid.velocity = new Vector2(MaxSpeed, _rigid.velocity.y);
        else if (_rigid.velocity.x < -MaxSpeed)
            _rigid.velocity = new Vector2(-MaxSpeed, _rigid.velocity.y);

        //디버깅용 레이 그린거
        Debug.DrawRay(_rigid.position, Vector3.down, Color.red);
        Debug.DrawRay(_rigid.position, Vector3.right * 0.5f, Color.red);
        Debug.DrawRay(_rigid.position, Vector3.left * 0.5f, Color.red);

        //landing 체크
        if (_rigid.velocity.y <= 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(_rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null && _stateMachine.curState == JUMP)
            {
                if (rayHit.distance <= 0.6)
                {
                    _stateMachine.changeState(IDLE);
                }
            }
        }

        // 패닉 대충 그리기
        Vector3 position = _rigid.position;
        position.x -= transform.localScale.x * 0.5f;
        position.y += transform.localScale.y;
        Debug.DrawRay(position, Vector3.right * panicScore * 0.01f, Color.blue);
    }

    private int GstPanicLv0()
    {
        if (panicScore >= 50)
        {
            return PANIC_LV1;
        }

        return PANIC_LV0;
    }

    void GstPanicLv1Begin()
    {
        PanicText.text = "PANIC_LV1, 똑바로 해라잉";
    }

    private int GstPanicLv1()
    {
        if(panicScore < 50)
        {
            return PANIC_LV0;
        }
        else if (panicScore >= 75)
        {
            return PANIC_LV2;
        }

        return PANIC_LV1;
    }

    void GstPanicLv2Begin()
    {
        PanicText.text = "오줌 지림. 쌍욕을 하는데, 텍스트가 작게 비뚤어진다.";
        Vector3 rotation = PanicText.gameObject.transform.localEulerAngles;
        rotation.y = 30;
        PanicText.gameObject.transform.localEulerAngles = rotation;
    }

    private int GstPanicLv2()
    {
        if (panicScore < 75)
            return PANIC_LV1;

        else if (panicScore >= 99)
            return PANIC_LV3;

        float random = Random.Range(-trembleDist, trembleDist);
        Vector3 dir = Vector3.right;
        dir.x *= random;

        transform.Translate(dir);

        return PANIC_LV2;
    }

    private void GstPanicLv3Begin()
    {
        _stateMachine.Stop = true;
    }

    private int GstPanicLv3()
    {


        return PANIC_LV3;
    }

    private void StIdleBegin()
    {
        _canWallJump = true;
    }

    private int StIdle()
    {
        if (true == getJumpKeyDown())
        {
            return JUMP;
        }

        if (true == getRightKey())
        {
            _dir = Vector2.right;
            return WALK;
        }
        else if (true == getLeftKey())
        {
            _dir = Vector2.left;
            return WALK;
        }

        
        if (true == getDownKey())
        {
            return CROUCH;
        }

        _rigid.velocity = new Vector2(_rigid.velocity.normalized.x * 0.5f, _rigid.velocity.y);
        if (_rigid.velocity.normalized.x <= 1)
            _rigid.velocity = new Vector2(0, _rigid.velocity.y);

        return IDLE;
    }

    private void StWalkBegin()
    {
        float moveAxis = Input.GetAxisRaw("Horizontal");
        float moveAbs = Mathf.Abs(moveAxis);

        _anim.SetFloat("Move", moveAbs);
    }

    int StWalk()
    {
        // 캐릭터 컨트롤러의 무브
        _rigid.AddForce(_dir * Speed, ForceMode2D.Impulse);

        if (false == getRightKey() && _dir == Vector2.right)
        {
            return IDLE;
        }
        else if(false == getLeftKey() && _dir == Vector2.left)
        {
            return IDLE;
        }

        if (true == getJumpKeyDown())
        {
            return JUMP;
        }
        else if (true == getRollKey())
        {
            return ROLL;
        }
        else if (true == getDownKey())
        {
            return CROUCH;
        }

        return WALK;
    }

    void StCrouchBegin()
    {
        transform.localScale = new Vector2(_scale.x, _scale.y * 0.5f);
        //transform.position = new Vector2(transform.position.x, transform.position.y - _scale.y * 0.1f);
    }

    int StCrouch()
    {
        if(false == getDownKey())
        {
            
            return IDLE;
        }

        return CROUCH;
    }

    void StCrouchEnd()
    {
        transform.localScale = _scale;
        transform.position = new Vector2(transform.position.x, transform.position.y + _scale.y * 0.25f);
    }

    void StJumpBegin()
    {
        _rigid.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
    }

    int StJump()
    {
        if (true == getRightKey())
        {
            _dir = Vector2.right;
            _rigid.AddForce(_dir * Speed, ForceMode2D.Impulse);
            
        }
        else if (true == getLeftKey())
        {
            _dir = Vector2.left;
            _rigid.AddForce(_dir * Speed, ForceMode2D.Impulse);

        }

        do // 벽점프 체크
        {
            if (_canWallJump == false)
                break;

            do // 왼쪽 벽 체크
            {
                RaycastHit2D rayHit = Physics2D.Raycast(_rigid.position, Vector3.left, 1, LayerMask.GetMask("Platform"));
                if (rayHit.collider == null)
                    break;

                if (rayHit.distance > 0.6)
                    break;
                
                if(getJumpKeyDown() == true)
                {
                    _rigid.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
                }

            } while (false);

            do // 오른쪽 벽 체크
            {
                RaycastHit2D rayHit = Physics2D.Raycast(_rigid.position, Vector3.right, 1, LayerMask.GetMask("Platform"));
                if (rayHit.collider == null)
                    break;

                if (rayHit.distance > 0.6)
                    break;

                if (getJumpKeyDown() == true)
                {
                    _rigid.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
                }

            } while (false);

        } while (false);

        return JUMP;
    }

    void StRollBegin()
    {
        _accRollTime = 0;

        transform.localScale = new Vector2(_scale.x, _scale.y * 0.5f);
        transform.position = new Vector2(transform.position.x, transform.position.y - _scale.y * 0.25f);
    }

    int StRoll()
    {
        _accRollTime += Time.deltaTime;
        transform.Translate(_dir * (Speed + RollSpeed) * Time.deltaTime);

        if (_accRollTime > RollTime)
        {
            return IDLE;
        }

        return ROLL;
    }

    void StClimbBegin()
    {
        _gravity = _rigid.gravityScale;
        _canWallJump = false;

        _rigid.velocity = Vector2.zero;
        _rigid.gravityScale = 0;
    }

    int StClimbUpdate()
    {
        if(getJumpKeyDown() == true)
        {
            return JUMP;
        }

        return CLIMB;
    }

    void StclimbEnd()
    {
        _rigid.gravityScale = _gravity;
    }

    void StRollEnd()
    {
        _accRollTime = 0;
        transform.localScale = _scale;
        transform.position = new Vector2(transform.position.x, transform.position.y + _scale.y * 0.25f);
    }

    private bool getRightKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.D);
        return isKeyDown;
    }

    private bool getLeftKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.A);
        return isKeyDown;
    }

    private bool getJumpKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space);
        return isKeyDown;
    }

    private bool getJumpKeyDown()
    {
        bool isKeyDown = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space);
        return isKeyDown;
    }

    private bool getDownKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.S);
        return isKeyDown;
    }

    private bool getRollKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        return isKeyDown;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
