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
                Debug.Log("되돌아가는 중");
                if(state == EMoveBlockState.LeftAndRight)
                {
                    StartCoroutine("StartReturnMoveLeftAndRight");
                }
                else if(state == EMoveBlockState.BottomAndTop)
                {
                    StartCoroutine("StartReturnMoveBottomAndTop");
                }

                isReturn = true;
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

    IEnumerator StartReturnMoveLeftAndRight()
    {
        while(originCoord.x + 0.2 < this.transform.position.x || originCoord.x - 0.2 > this.transform.position.x)
        {
            Vector3 moveDirection = Vector3.zero;

            if (isDst)
            {
                moveDirection = Vector3.right;
            }
            else
            {
                moveDirection = Vector3.left;
            }

            this.transform.position += moveDirection * moveSpeed * 0.01f;

            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("되돌아옴");
    }

    IEnumerator StartReturnMoveBottomAndTop()
    {
        while(originCoord.y + 0.2 < this.transform.position.y || originCoord.y - 0.2 > this.transform.position.y)
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

            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("되돌아옴");
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

