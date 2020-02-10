using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine;
using UnityEngine.UI;

public class EyeSight : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject head;
    [SerializeField] GameObject eyeSight;

    [SerializeField] bool isTracking = true;

    [SerializeField] float limitEyeSightRange = 50.0f;

    private float upRightAngle;
    private float upLeftAngle;
    private float downRightAngle;
    private float downLeftAngle;

    private bool isRightFrontHead = true;
    private bool isTrackingPoint = false;

    private Vector3 mousePos;
    private Vector3 playerPos;

    void Start() {
        upRightAngle = 90.0f - limitEyeSightRange;
        upLeftAngle = 90.0f + limitEyeSightRange;
        downRightAngle = -90.0f + limitEyeSightRange;
        downLeftAngle = 270.0f - limitEyeSightRange;
        
        if(eyeSight.GetComponent<Light2D>().lightType == Light2D.LightType.Point)
        {
            isTrackingPoint = eyeSight.name == "EyeSight_Point";
        }
    }

    void Update()
    {
        // 디버깅용
        Debug.DrawRay(head.transform.position, mousePos - playerPos, Color.red);
        // Debug.DrawRay(head.transform.position, Vector3.right * 0.5f, Color.red);
        // Debug.DrawRay(head.transform.position, Vector3.left * 0.5f, Color.red);

        // if (eyeSight.velocity.y <= 0)
        // {
        //     RaycastHit2D rayHit = Physics2D.Raycast(_rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
        //     if (rayHit.collider != null && _stateMachine.curState == JUMP)
        //     {
        //         if (rayHit.distance <= 0.6)
        //         {
        //             _stateMachine.changeState(IDLE);
        //         }
        //     }
        // }
    }

    void FixedUpdate()
    {
        if(!isTrackingPoint && isTracking)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            playerPos = player.transform.position;

            TrackingMouse();
            SetPlayerHeadFront();
        }
        else if(isTrackingPoint && isTracking)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            eyeSight.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }

    void TrackingMouse()
    {
        Vector2 structPos = new Vector2(mousePos.x - playerPos.x, mousePos.y - playerPos.y);

        float rad = Mathf.Atan2(structPos.x, structPos.y);
        float mouseRotate = (rad * 180) / Mathf.PI;
        float unityAngle = (-mouseRotate + 90);
        float convertToAngle = ConvertToAngle(unityAngle);
        
        head.transform.localEulerAngles = new Vector3(0, 0, convertToAngle);
        eyeSight.transform.localEulerAngles = new Vector3(0, 0, convertToAngle - 90);
    }

    float ConvertToAngle(float unityAngle)
    {
        // 우상단 제한
        if(90.0f > unityAngle && upRightAngle < unityAngle)
        {
            return upRightAngle;
        }
        // 좌상단 제한
        else if(90.0f < unityAngle && upLeftAngle > unityAngle)
        {
            return upLeftAngle;
        }
        // 우하단 제한
        else if(-90.0f < unityAngle && downRightAngle > unityAngle)
        {
            return downRightAngle;
        }
        // 좌하단 제한
        else if(270.0f > unityAngle && downLeftAngle < unityAngle)
        {
            return downLeftAngle;
        }
        else
        {
            return unityAngle;
        }

    }

    void SetPlayerHeadFront()
    {
        if(isRightFrontHead && mousePos.x < playerPos.x)
        {
            isRightFrontHead = false;
            head.transform.localScale = new Vector3(head.transform.localScale.x, -head.transform.localScale.y, 1);
            // eyeSight.transform.localScale = new Vector3(eyeSight.transform.localScale.x, -eyeSight.transform.localScale.y, 1);
        }
        else if(!isRightFrontHead && mousePos.x > playerPos.x)
        {
            isRightFrontHead = true;
            head.transform.localScale = new Vector3(head.transform.localScale.x, -head.transform.localScale.y, 1);
            // eyeSight.transform.localScale = new Vector3(eyeSight.transform.localScale.x, -eyeSight.transform.localScale.y, 1);
        }
    }

    public void SetTracking(bool tracking)
    {
        isTracking = tracking;
    }
}
