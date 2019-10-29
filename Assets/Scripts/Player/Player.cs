using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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

    enum STATE
    {
        IDLE = 0,
        WALK,
        JUMP,
        ROLL,
        CROUCH,
        END
    }

    [SerializeField] public float Speed = 1;
    [SerializeField] public float MaxSpeed = 10;
    [SerializeField] public float RollSpeed = 0.5f;
    [SerializeField] public float RollTime = 0.5f;
    [SerializeField] public float JumpSpeed = 5;
    [SerializeField] private PANIC_LV PanicState = PANIC_LV.Lv2;
    [SerializeField] private float TrembleDist = 0.5f;
    [SerializeField] public float TrembleLv2Speed = 0.01f;

    private Rigidbody2D _rigid;
    private Vector2 _dir;
    private Vector2 _scale;
    private Vector2 _trembleDir = Vector2.right;

    private float _accRollTime = 0;
    private float _accTrembleDist = 0;

    StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = new StateMachine { };

        _stateMachine.SetCallback((int)STATE.IDLE, StIdle, null, null);
        _stateMachine.SetCallback((int)STATE.WALK, StWalk, null, null);
        _stateMachine.SetCallback((int)STATE.JUMP, StJump, null, null);
        _stateMachine.SetCallback((int)STATE.ROLL, StRoll, StRollBegin, StRollEnd);
        _stateMachine.SetCallback((int)STATE.CROUCH, StCrouch, StCrouchBegin, StCrouchEnd);
    }

    

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();

        if (_rigid.velocity.x >= MaxSpeed)
            _rigid.velocity = new Vector2(MaxSpeed, _rigid.velocity.y);
    }

    void trembleLv2()
    {
        float moveDist = TrembleLv2Speed * Time.deltaTime;
        _accTrembleDist += moveDist;

        transform.Translate(_trembleDir * moveDist);
        if(_accTrembleDist >= TrembleDist)
        {
            _trembleDir.x *= -1;
            _accTrembleDist = 0;
        }
    }

    private int StIdle()
    {
        if (true == getRightKey())
        {
            _dir = Vector2.right;
            return (int)STATE.WALK;
        }
        else if (true == getLeftKey())
        {
            _dir = Vector2.left;
            return (int)STATE.WALK;
        }

        if (true == getJumpKey())
        {
            _rigid.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
            return (int)STATE.JUMP;
        }
        else if (true == getDownKey())
        {
            return (int)STATE.CROUCH;
        }

        return (int)STATE.IDLE;
    }

    int StWalk()
    {
        _rigid.AddForce(_dir * Speed, ForceMode2D.Force);

        if (false == getRightKey() && _dir == Vector2.right)
        {
            return (int)STATE.IDLE;
        }
        else if(false == getLeftKey() && _dir == Vector2.left)
        {
            return (int)STATE.IDLE;
        }

        if (true == getJumpKey())
        {
            _rigid.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
            return (int)STATE.JUMP;
        }
        else if (true == getRollKey())
        {
            _accRollTime = 0;
            return (int)STATE.ROLL;
        }
        else if (true == getDownKey())
        {
            return (int)STATE.CROUCH;
        }

        return (int)STATE.WALK;
    }

    void StCrouchBegin()
    {
        transform.localScale = new Vector2(_scale.x, _scale.y * 0.5f);
        transform.position = new Vector2(transform.position.x, transform.position.y - _scale.y * 0.25f);
    }

    int StCrouch()
    {
        if(false == getDownKey())
        {
            
            return (int)STATE.IDLE;
        }

        return (int)STATE.CROUCH;
    }

    void StCrouchEnd()
    {
        transform.localScale = _scale;
        transform.position = new Vector2(transform.position.x, transform.position.y + _scale.y * 0.25f);
    }

    int StJump()
    {
        if (true == getRightKey())
        {
            _dir = Vector2.right;
            _rigid.AddForce(_dir * Speed, ForceMode2D.Force);
            
        }
        else if (true == getLeftKey())
        {
            _dir = Vector2.left;
            _rigid.AddForce(_dir * Speed, ForceMode2D.Force);

        }

        return (int)STATE.JUMP;
    }

    void StRollBegin()
    {
        transform.localScale = new Vector2(_scale.x, _scale.y * 0.5f);
        transform.position = new Vector2(transform.position.x, transform.position.y - _scale.y * 0.25f);
    }

    int StRoll()
    {
        _accRollTime += Time.deltaTime;
        transform.Translate(_dir * (Speed + RollSpeed) * Time.deltaTime);

        if (_accRollTime > RollTime)
        {
            return (int)STATE.IDLE;
        }

        return (int)STATE.ROLL;
    }

    void StRollEnd()
    {
        _accRollTime = 0;
        transform.localScale = _scale;
        transform.position = new Vector2(transform.position.x, transform.position.y + _scale.y * 0.25f);
    }

    private bool getRightKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        return isKeyDown;
    }

    private bool getLeftKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        return isKeyDown;
    }

    private bool getJumpKey()
    {
        if (PanicState == PANIC_LV.Lv3)
            return false;

        bool isKeyDown = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space);
        return isKeyDown;
    }

    private bool getDownKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        return isKeyDown;
    }

    private bool getRollKey()
    {
        bool isKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        return isKeyDown;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Platform" && _stateMachine.curState == (int)STATE.JUMP)
        {
            Vector2 colPos = collision.GetContact(0).point;
            if(colPos.y < transform.localPosition.y)
            {
                _stateMachine.curState = (int)STATE.IDLE;
                transform.localPosition = new Vector2(transform.localPosition.x, colPos.y + transform.localScale.y * 0.5f);
            }
        }
    }

}
