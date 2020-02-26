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
    [SerializeField] float moveSpeed = 1f;

    public EMoveBlockState state = EMoveBlockState.LeftAndRight;
  
    [SerializeField] float SrcCoord = -1.0f;
    [SerializeField] float DstCoord = 1.0f;
    [SerializeField] bool isReturnBlock = false;

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

