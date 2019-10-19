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
        IDLE,
        WALK,
        JUMP,
        ROLL,
        END
    }

    [SerializeField] public float Speed = 1;
    [SerializeField] public float RollSpeed = 0.5f;
    [SerializeField] public float RollTime = 0.5f;
    [SerializeField] public float JumpSpeed = 5;
    [SerializeField] private PANIC_LV PanicState = PANIC_LV.Lv2;
    [SerializeField] private PANIC_LV3_STATE Panic_Lv3 = PANIC_LV3_STATE.STOP;
    [SerializeField] private float TrembleDist = 0.5f;
    [SerializeField] public float TrembleLv2Speed = 0.01f;

    private STATE _state;
    private Rigidbody2D _rigid;
    private Vector2 _dir;
    private Vector2 _trembleDir = Vector2.right;

    private float _accRollTime = 0;
    private float _accTrembleDist = 0;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(PanicState)
        {
            case PANIC_LV.Lv1:      Panic_Lv1_Update();     break;
            case PANIC_LV.Lv2:      Panic_Lv2_Update();     break;
            case PANIC_LV.Lv3:      Panic_Lv3_Update();     break;
            case PANIC_LV.Lv_END:                           break;
        }
        

        
    }

    void Panic_Lv1_Update()
    {
        switch (_state)
        {
            case STATE.IDLE: Idle(); break;
            case STATE.WALK: Walk(); break;
            case STATE.JUMP: Jump(); break;
            case STATE.ROLL: Roll(); break;
            case STATE.END: break;
        }
    }

    void Panic_Lv2_Update()
    {
        trembleLv2();
        switch (_state)
        {
            case STATE.IDLE: Idle(); break;
            case STATE.WALK: Walk(); break;
            case STATE.JUMP: Jump(); break;
            case STATE.END: break;
        }
    }

    void Panic_Lv3_Update()
    {
        switch (Panic_Lv3)
        {
            case PANIC_LV3_STATE.STOP: break;
            case PANIC_LV3_STATE.RUN_LEFT:  transform.Translate(Vector2.left * Speed * Time.deltaTime);     break;
            case PANIC_LV3_STATE.RUN_RIGHT: transform.Translate(Vector2.right * Speed * Time.deltaTime);    break;
            case PANIC_LV3_STATE.END: break;
        }

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

    

    void Idle()
    {
        if (true == getRightKey())
        {
            _state = STATE.WALK;
            _dir = Vector2.right;
        }
        else if (true == getLeftKey())
        {
            _state = STATE.WALK;
            _dir = Vector2.left;
        }

        if (true == getJumpKey())
        {
            _state = STATE.JUMP;
        }

    }

    void Walk()
    {
        transform.Translate(_dir * Speed * Time.deltaTime);

        if (false == getRightKey() && _dir == Vector2.right)
        {
            _state = STATE.IDLE;
        }
        else if(false == getLeftKey() && _dir == Vector2.left)
        {
            _state = STATE.IDLE;
        }

        if (true == getJumpKey())
        {
            _state = STATE.JUMP;
        }
        else if (true == getRollKey())
        {
            _state = STATE.ROLL;
            _accRollTime = 0;
        }
    }

    void Jump()
    {
        transform.Translate(Vector2.up * JumpSpeed * Time.deltaTime);

        if (true == getRightKey())
        {
            _dir = Vector2.right;
            transform.Translate(_dir * Speed * Time.deltaTime);
        }
        else if (true == getLeftKey())
        {
            _dir = Vector2.left;
            transform.Translate(_dir * Speed * Time.deltaTime);
        }

    }

    void Roll()
    {
        _accRollTime += Time.deltaTime;
        transform.Translate(_dir * (Speed + RollSpeed) * Time.deltaTime);

        if(_accRollTime > RollTime)
        {
            _accRollTime = 0;
            _state = STATE.IDLE;
        }

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
        if(collision.transform.tag == "Platform" && _state == STATE.JUMP)
        {
            _state = STATE.IDLE;
        }
    }

}
