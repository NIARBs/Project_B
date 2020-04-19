using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EMoveBlockState
{
    LeftAndRight,
    BottomAndTop,
    Stop
}

public class MoveBlock : MonoBehaviour
{
    [Tooltip ("LeftAndRight: 좌우로 이동, BottomAndTop: 위/아래로 이동, Stop: 멈춤")]
    public EMoveBlockState state = EMoveBlockState.LeftAndRight;
  
    [Header ("- 이동할 거리")][Tooltip ("출발 좌표를 입력합니다.")]
    [SerializeField] float SrcCoord = -1.0f;
    [Tooltip ("목적 좌표를 입력합니다.")]
    [SerializeField] float DstCoord = 1.0f;

    [Header ("- 속성 설정")][Tooltip ("생체인식이 취소될 때, 플랫폼이 원래의 자리로 돌아올지를 설정합니다.")]
    [SerializeField] bool isReturnBlock = false;

    [Tooltip ("플랫폼 이동 속도를 설정합니다.")]
    [SerializeField] float moveSpeed = 1f;

    private Vector2 originCoord;
    private Vector2 originSrcCoord;
    private Vector2 originDstCoord;
    private bool isDst = false;
    private bool isPause = false;
    private bool isReturn = false;

    void Start()
    {
        originCoord.x = this.transform.position.x;
        originCoord.y = this.transform.position.y;
        originSrcCoord.x = this.transform.position.x + SrcCoord;
        originSrcCoord.y = this.transform.position.y + SrcCoord;
        originDstCoord.x = this.transform.position.x + DstCoord;
        originDstCoord.y = this.transform.position.y + DstCoord;
    }

    void FixedUpdate()
    {
        if(isPause)
        {
            if(!isReturn && isReturnBlock)
            {
                if(state == EMoveBlockState.LeftAndRight)
                {
                    ReturnMoveLeftAndRight();
                }
                else if(state == EMoveBlockState.BottomAndTop)
                {
                    ReturnMoveBottomAndTop();
                }
            }
            return;
        }

        if(state == EMoveBlockState.LeftAndRight)
        {
            MoveLeftAndRight();
        }
        else if(state == EMoveBlockState.BottomAndTop)
        {
            MoveBottomAndTop();
        }
    }
    
    void MoveLeftAndRight()
    {
        Vector3 moveDirection = Vector3.zero;

        if(isDst)
        {
            moveDirection = Vector3.right;
        }
        else
        {
            moveDirection = Vector3.left;
        }
        
        this.transform.position += moveDirection * moveSpeed * 0.01f;

        if(originSrcCoord.x >= this.transform.position.x)
        {
            isDst = true;
        }
        else if(originDstCoord.x <= this.transform.position.x)
        {
            isDst = false;
        }
    }
    
    void MoveBottomAndTop()
    {
        Vector3 moveDirection = Vector3.zero;

        if (isDst)
        {
            moveDirection = Vector3.up;
        }
        else
        {
            moveDirection = Vector3.down;
        }

        this.transform.position += moveDirection * moveSpeed * 0.01f;

        if (originSrcCoord.y >= this.transform.position.y)
        {
            isDst = true;
        }
        else if (originDstCoord.y <= this.transform.position.y)
        {
            isDst = false;
        }
    }

    void ReturnMoveLeftAndRight()
    {
        MoveLeftAndRight();

        if(originCoord.x + 0.05 >= this.transform.position.x && originCoord.x - 0.05 <= this.transform.position.x)
        {
            this.transform.position = originCoord;
            isReturn = true;
        }
    }

    void ReturnMoveBottomAndTop()
    {
        MoveBottomAndTop();

        if(originCoord.y + 0.05 >= this.transform.position.y && originCoord.y - 0.05 <= this.transform.position.y)
        {
            this.transform.position = originCoord;
            isReturn = true;
        }
    }

    public void SetPause(bool pause)
    {
        isPause = pause;
        if(isPause)
        {
            isReturn = false;
        }
    }
}

