using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float accelerationTime;
    [SerializeField] private float deaccelerationTime;
    [SerializeField] private float moveDistance;

    private float acceleration;
    private float deacceleration;
    private float maxSpeed;

    protected Rigidbody2D rigid;

    /*
    int VerticalPositionOrigin;     // 점프 시작시 위치
    int VerticalPosition;           // 현재 위치
    int VerticalSpeed;              // 속도
    int VerticalForce;              // 현재 가속도
    int VerticalForceFall;          // 강하 시 가속도
    int VerticalForceDecimalPart;   // 가속도 증가값
    int CorrectionValue;            // 누적 계산에서의 보정치？

    int HorizontalSpeed = 00;       // 가로 방향 속도

    // 점프 시작 시 초기 파라미터
    static readonly byte[] VerticalForceDecimalPartData = { 0x20, 0x20, 0x1e, 0x28, 0x28 }; // 가속도 증가값
    static readonly byte[] VerticalFallForceData = { 0x70, 0x70, 0x60, 0x90, 0x90 }; // 강하시의 가속도
    static readonly sbyte[] InitialVerticalSpeedData = { -4, -4, -4, -5, -5 }; // 초속도
    static readonly byte[] InitialVerticalForceData = { 0x00, 0x00, 0x00, 0x00, 0x00 }; // 초기 가속도

    // 낙하 최대속도
    static readonly sbyte DOWN_SPEED_LIMIT = 0x04;

    // 1플레전의 점프 버튼 누름 상태
    bool JumpBtnPrevPress = false;

    // 지면에있느냐,점프중이냐
    public enum MovementState
    {
        OnGround,
        Jumping
    }

    MovementState CurrentState = MovementState.OnGround;
    */

    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetButtonDown("Jump"))
        {
            CurrentState = MovementState.Jumping;
        }
        */
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveVelocity = Vector3.zero;

        if(Input.GetAxisRaw("Horizontal") < 0)
        {
            moveVelocity = Vector3.left;
        }
        else if(Input.GetAxisRaw("Horizontal") > 0)
        {
            moveVelocity = Vector3.right;
        }
        Debug.Log(moveVelocity);

        transform.position += moveVelocity * moveDistance * Time.deltaTime;
    }

    /*
    public void ResetParam(int initVerticalPos)
    {
        VerticalSpeed = 0;
        VerticalForce = 0;
        VerticalForceFall = 0;
        VerticalForceDecimalPart = 0;
        CurrentState = MovementState.OnGround;
        CorrectionValue = 0;

        VerticalPosition = initVerticalPos;
    }

    public int PosY
    {
        //set { VerticalPosition = value; }
        get { return VerticalPosition; }
    }

    public MovementState GetPlayerState
    {
        get
        {
            return CurrentState;
        }
    }

    public void Movement(bool jumpBtnPress)
    {
        JumpCheck(jumpBtnPress);
        MoveProcess(jumpBtnPress);

        JumpBtnPrevPress = jumpBtnPress;
    }

    private void JumpCheck(bool jumpBtnPress)
    {
        // 처음 점프버튼 눌렀어?
        if (jumpBtnPress == false) return;
        if (JumpBtnPrevPress == true) return;

        // 땅 위에 있는 상태?
        if (CurrentState == 0)
        {
            // 점프 시작 준비
            PreparingJump();
        }
    }

    private void PreparingJump()
    {
        VerticalForceDecimalPart = 0;
        VerticalPositionOrigin = VerticalPosition;

        CurrentState = MovementState.Jumping;

        int idx = 0;
        if (HorizontalSpeed >= 0x1c) idx++;
        if (HorizontalSpeed >= 0x19) idx++;
        if (HorizontalSpeed >= 0x10) idx++;
        if (HorizontalSpeed >= 0x09) idx++;

        VerticalForce = VerticalForceDecimalPartData[idx];
        VerticalForceFall = VerticalFallForceData[idx];
        VerticalForceDecimalPart = InitialVerticalForceData[idx];
        VerticalSpeed = InitialVerticalSpeedData[idx];
    }

    private void MoveProcess(bool jumpBtnPress)
    {
        // 속도가 0이나 플러스면 화면 아래로 내려가 있는 걸로 해서 낙하 상태의 가속도로 전환해.
        if (VerticalSpeed >= 0)
        {
            VerticalForce = VerticalForceFall;
        }
        else
        {
            // A버튼이 떨어졌어&상승중?
            if (jumpBtnPress == false && JumpBtnPrevPress == true)
            {
                if (VerticalPositionOrigin - VerticalPosition >= 1)
                {
                    // 낙하 상태의 가속도값으로 전환
                    VerticalForce = VerticalForceFall;
                }
            }
        }

        Physics();
    }

    private void Physics()
    {
        // 누적 계산에서의 보정치 같음
        int cy = 0;
        CorrectionValue += VerticalForceDecimalPart;
        if (CorrectionValue >= 256)
        {
            CorrectionValue -= 256;
            cy = 1;
        }

        // 현재 위치 속도 가산 (누적계산 보정값도 가산)
        VerticalPosition += VerticalSpeed + cy;

        // 가속도의 고정 소수 점부에 대한 가산
        // 1바이트를 오버플로 하면 속도가 가산된다
        VerticalForceDecimalPart += VerticalForce;
        if (VerticalForceDecimalPart >= 256)
        {
            VerticalForceDecimalPart -= 256;
            VerticalSpeed++;
        }

        // 속도 상한 체크
        if (VerticalSpeed >= DOWN_SPEED_LIMIT)
        {
            // 수수께끼의 판정
            if (VerticalForceDecimalPart >= 0x80)
            {
                VerticalSpeed = DOWN_SPEED_LIMIT;
                VerticalForceDecimalPart = 0x00;
            }
        }
    }
    */
}
