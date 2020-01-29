using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Vector2 moveVector;

    // Move Variables
    public float maxSpeed;
    public float acceleration;

    public float jumpForce;
    public float gravity;
    public float wallDragSpeed;


    private float xInput;
    private float internalXSpeed;

    
    private bool isGrounded;
    private bool isOnWall;

    private int flip;


    private enum WallDirection
    {
        RIGHT = -1,
        NONE = 0,
        LEFT = 1
    }

    private WallDirection wallDirection;


    // Raycasts
    public int vRayCount = 3;
    public int hRayCount = 3;

    public float bufferDist = 0.1f;

    private float height;
    private float width;

    // Rays
    private Vector3[] vRayOrigins;
    private Vector3[] hRayOrigins;

    
    [SerializeField]
    private BoxCollider2D boxCollider;

    [SerializeField]
    private TextDebugger textDebugger;

    void Reset()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private int ignoreLayer;


    // Start is called before the first frame update
    void Start()
    {
        vRayOrigins = new Vector3[vRayCount];
        hRayOrigins = new Vector3[hRayCount];
        moveVector = Vector2.zero;

        width = boxCollider.size.x * 0.9f;
        height = boxCollider.size.y * 0.9f;
        
        isGrounded = false;
        flip = 1;

        // 8: playerLayer
        ignoreLayer = ~(1 << 9);
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        // 캐릭터 좌/우 반전
        if(xInput < 0)
        {
            flip = -1;
        }
        else if(xInput > 0)
        {
            flip = 1;
        }
        // 적용
		transform.localScale = new Vector3(flip, 1, 1);

        // 가속
        if(xInput > 0)
        {
            internalXSpeed += acceleration;
            if(internalXSpeed > maxSpeed)
            {
                internalXSpeed = maxSpeed;
            }
        }
        else if(xInput < 0)
        {
            internalXSpeed -= acceleration;
            if(internalXSpeed < -maxSpeed)
            {
                internalXSpeed = -maxSpeed;
            }
        }
        // 공중에서의 반동을 없애려면 if(...) 부분을 삭제하고 else만 남겨둘 것
        else if(isGrounded || isOnWall)
        {
            if(internalXSpeed < 0)
            {
                internalXSpeed += acceleration;

                if(internalXSpeed > 0)
                {
                    internalXSpeed = 0;
                }
            }
            else if(internalXSpeed > 0)
            {
                internalXSpeed -= acceleration;
                
                if(internalXSpeed < 0)
                {
                    internalXSpeed = 0;
                }
            }
        }

        // 점프: 땅에 닿아있거나 벽에 닿아있는 도중에만 사용
        if(Input.GetKeyDown(KeyCode.Space) && (isOnWall || isGrounded))
        {
            Jump();
        }

        // 속도 적용
        moveVector.x = internalXSpeed * Time.deltaTime;

        // 중력
        moveVector.y -= gravity * Time.deltaTime;

        // 벽을 타는 동안에는 wallDragSpeed 이하로 내려가지 않도록 설정
        if(isOnWall)
        {
            if(moveVector.y < wallDragSpeed * Time.deltaTime * -1)
            {
                moveVector.y = wallDragSpeed * Time.deltaTime * -1;
            }
        }
        

        // 벽 및 장애물 계산
        CalculateMove();

        // 계산 후 실제 속도 적용
        transform.Translate(moveVector);
        transform.rotation = Quaternion.identity;

        AddText("Internal X Speed : " + internalXSpeed);
        AddText("X Speed : " + moveVector.x);
        AddText("Y Speed : " + moveVector.y);
        AddText("isGrounded : " + isGrounded);
        AddText("isOnWall  : " + isOnWall);
        AddText("WallDirection  : " + wallDirection);
        AddText("Speed : " + moveVector.magnitude);
    }


    void Jump()
    {
        moveVector.y = jumpForce;
        isGrounded = false;

        if(isOnWall)
        {
            internalXSpeed = maxSpeed * (int)wallDirection;
        }
    }



    void CalculateMove()
    {
        wallDirection = WallDirection.NONE;
        isGrounded = false;
        isOnWall = false;
        
        // Vertical
        CalculateVertical();
        
        // Horizontal
        CalculateHorizontal();
    }


    // 수평 방향 벽 감지
    void CalculateHorizontal()
    {
        // Ray 간격
        float hRayOffset = boxCollider.size.y / (hRayCount - 1);
        // Ray 방향
        float xSign = moveVector.x <= 0 ? -1 : 1;

        if(moveVector.x == 0) xSign = flip;

        for(int i = 0; i < hRayCount; i++)
        {
            // Ray 발사 지점
            hRayOrigins[i] = new Vector3(xSign * height * 0.5f,
                                         hRayOffset * (i - hRayCount / 2) + moveVector.y);

            // 발사
            RaycastHit2D hit = Physics2D.Raycast(transform.position + hRayOrigins[i],
                                                 Vector2.right, 
                                                 moveVector.x + bufferDist * xSign, // 현재 프레임에서 움직이는 거리 + 버퍼
                                                 ignoreLayer);
            
            // DEBUG
            Debug.DrawLine(transform.position + hRayOrigins[i],
                           transform.position + hRayOrigins[i] + new Vector3((moveVector.x + bufferDist * xSign), 0),
                           Color.yellow);

            // 벽에 닿음
            if(hit)
            {
                internalXSpeed = 0;

                // 이동 거리가 버퍼를 침범하는 경우
                if(Mathf.Abs(hit.distance - bufferDist) <= Mathf.Abs(moveVector.x))
                {
                    moveVector.x = (hit.distance - bufferDist) * xSign;
                }

                // 벽 슬라이딩
                if(isGrounded == false && xInput != 0)
                {
                    isOnWall = true;
                    
                    if(xSign < 0)
                    {
                        wallDirection = WallDirection.LEFT;
                    }
                    else
                    {
                        wallDirection = WallDirection.RIGHT;
                    }
                }
                
                AddText("Horizontal Hit [" + i + "] : O : " + hit.distance * xSign);
            }

            else
            {
                AddText("Horizontal Hit [" + i + "] : X");
            }
        }
    }


    // 수직 방향 벽 감지
    void CalculateVertical()
    {
        float vRayOffset = boxCollider.size.x / (vRayCount - 1);
        float ySign = (moveVector.y <= 0 || isGrounded == true) ? -1 : 1;

        int hitCount = 0;

        float minDist = (Mathf.Abs(moveVector.y) + bufferDist + 0.001f);
        float verticalHitLength = minDist * ySign;

        for(int i = 0; i < vRayCount; i++)
        {
            vRayOrigins[i] = new Vector3(vRayOffset * (i - vRayCount / 2),
                                         ySign * height * 0.5f);
                            
            RaycastHit2D hit = Physics2D.Raycast(transform.position + vRayOrigins[i],
                                                 Vector2.up, 
                                                 verticalHitLength,
                                                 ignoreLayer);
            
            // DEBUG
            Debug.DrawLine(transform.position + vRayOrigins[i],
                           transform.position + vRayOrigins[i] + new Vector3(0, verticalHitLength, 0),
                           Color.red);


            // 천장 또는 바닥에 닿음
            if(hit)
            {
                hitCount++;

                // 천장
                if(ySign > 0)
                {
                    if(hit.distance - bufferDist < moveVector.y)
                        minDist = Mathf.Min(minDist, hit.distance);
                }

                // 바닥
                else
                {
                    minDist = Mathf.Min(minDist, hit.distance);
                    isGrounded = true;
                }
                
                AddText("Vertical Hit [" + i + "] : O : " + hit.distance);
            }

            
            else
            {
                AddText("Vertical Hit [" + i + "] : X : " + verticalHitLength);
            }
        }

        if(hitCount > 0)
        {
            if(minDist <= bufferDist)
            {
                moveVector.y = 0;
            }
            else
            {
                moveVector.y = (minDist - bufferDist) * ySign;
            }
        }
    }


    // 화면의 디버깅 콘솔
    void AddText(string text)
    {
        if(textDebugger == null) return;
        textDebugger.AddText(text);
    }
}