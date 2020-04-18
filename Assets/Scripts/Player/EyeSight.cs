using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class EyeSight : MonoBehaviour
{
    struct LimitAngle
    {
        public float upRightAngle;
        public float upLeftAngle;
        public float downRightAngle;
        public float downLeftAngle;
    }

    [Header ("- 오브젝트 설정")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject eyeSight;
    [SerializeField] GameObject head;

    [Header ("- 마우스 트래킹 사용여부")]
    [SerializeField] bool isTracking = true;

    [Header ("- 마우스 비트래킹 각도")]
    [SerializeField] float limitEyeSightRange = 50.0f;

    [Header ("- 플레이어 시작 방향")]
    [SerializeField] bool isRightFront = true;

    private bool isHeadFlip = false;

    private LimitAngle eyeLimitAngle;
    private LimitAngle headLimitAngle;

    private Vector3 mousePos;
    private Vector3 playerPos;

    private GameObject hitObj = null;
    private bool alreadyHit = false;

    void Start() 
    {
        eyeLimitAngle.upRightAngle = 90.0f - limitEyeSightRange;
        eyeLimitAngle.upLeftAngle = 90.0f + limitEyeSightRange;
        eyeLimitAngle.downRightAngle = -90.0f + limitEyeSightRange;
        eyeLimitAngle.downLeftAngle = 270.0f - limitEyeSightRange;
        headLimitAngle.upRightAngle = 0.0f + limitEyeSightRange;
        headLimitAngle.upLeftAngle = 0.0f - limitEyeSightRange;
        headLimitAngle.downRightAngle = -90.0f + limitEyeSightRange;
        headLimitAngle.downLeftAngle = 90.0f - limitEyeSightRange;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(head.transform.position, mousePos - playerPos, 8);
        if(hit)
        {
            if(hit.collider.gameObject == null)
            {
                hitObj = null;
            }
            if(hitObj != null && hit.collider.gameObject == hitObj)
            {
                alreadyHit = true;
            }
            if(hitObj != null && hit.collider.gameObject != hitObj)
            {
                alreadyHit = false;
                
                // 전에 인식된 오브젝트가 생체인식일 경우
                if(hitObj.CompareTag("BiometricSensor"))
                {
                    hitObj.GetComponent<BiometricSensor>().PlayerExitSensor();
                }
                hitObj = hit.collider.gameObject;
            }
            hitObj = hit.collider.gameObject;
            if(hitObj != null && !alreadyHit)
            {
                // 현재 인식된 오브젝트가 생체인식일 경우
                if(hit.collider.CompareTag("BiometricSensor"))
                {
                    hitObj.GetComponent<BiometricSensor>().CheckStayPlayer();
                }
                Debug.Log(hit.collider.gameObject.name);
            }
        }

        // 디버깅용
        Debug.DrawRay(head.transform.position, mousePos - playerPos, Color.red);
        // Debug.DrawRay(head.transform.position, Vector3.right * 0.5f, Color.red);
        // Debug.DrawRay(head.transform.position, Vector3.left * 0.5f, Color.red);
    }

    void FixedUpdate()
    {
        if(isTracking)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));

            float distance;
            xy.Raycast(ray, out distance);
            mousePos = ray.GetPoint(distance);
            playerPos = player.transform.position;

            TrackingMouse();
            SetPlayerHeadFront();
        }
    }

    void TrackingMouse()
    {
        Vector2 structPos = new Vector2(mousePos.x - playerPos.x, mousePos.y - playerPos.y);

        float rad = Mathf.Atan2(structPos.x, structPos.y);
        float mouseRotate = (rad * 180) / Mathf.PI;
        float unityEyeAngle = (-mouseRotate + 90);
        float unityHeadAngle = !isHeadFlip ? (-mouseRotate - 90) : mouseRotate + 90;
        float convertToEyeAngle = ConvertToAngle(unityEyeAngle);
        float convertToHeadAngle = ConvertToHeadAngle(unityHeadAngle + (isRightFront ? 180 : 0));

        //head.transform.LookAt(mousePos);
        //Debug.Log((mousePos - playerPos).normalized);
        head.transform.localEulerAngles = new Vector3(0, 0, convertToHeadAngle);
        eyeSight.transform.localEulerAngles = new Vector3(0, 0, convertToEyeAngle - 90);
    }

    float ConvertToAngle(float unityAngle)
    {
        // 우상단 제한
        if(90.0f > unityAngle && eyeLimitAngle.upRightAngle < unityAngle)
        {
            return eyeLimitAngle.upRightAngle;
        }
        // 좌상단 제한
        else if(90.0f < unityAngle && eyeLimitAngle.upLeftAngle > unityAngle)
        {
            return eyeLimitAngle.upLeftAngle;
        }
        // 우하단 제한
        else if(-90.0f < unityAngle && eyeLimitAngle.downRightAngle > unityAngle)
        {
            return eyeLimitAngle.downRightAngle;
        }
        // 좌하단 제한
        else if(270.0f > unityAngle && eyeLimitAngle.downLeftAngle < unityAngle)
        {
            return eyeLimitAngle.downLeftAngle;
        }
        else
        {
            return unityAngle;
        }
    }

    float ConvertToHeadAngle(float unityHeadAngle)
    {
        // 우상단 제한
        if(0.0f > unityHeadAngle && headLimitAngle.upRightAngle < unityHeadAngle)
        {
            return headLimitAngle.upRightAngle;
        }
        // 좌상단 제한
        else if(0.0f < unityHeadAngle && headLimitAngle.upLeftAngle > unityHeadAngle)
        {
            return headLimitAngle.upLeftAngle;
        }
        // 우하단 제한
        else if(-90.0f < unityHeadAngle && headLimitAngle.downRightAngle > unityHeadAngle)
        {
            return headLimitAngle.downRightAngle;
        }
        // 좌하단 제한
        else if(90.0f > unityHeadAngle && headLimitAngle.downLeftAngle < unityHeadAngle)
        {
            return headLimitAngle.downLeftAngle;
        }
        else
        {
            return unityHeadAngle;
        }
    }

    void SetPlayerHeadFront()
    {
        if(isRightFront && mousePos.x < playerPos.x)
        {
            isRightFront = false;
            head.transform.localScale = new Vector3(head.transform.localScale.x, -head.transform.localScale.y, 1);
        }
        else if(!isRightFront && mousePos.x > playerPos.x)
        {
            isRightFront = true;
            head.transform.localScale = new Vector3(head.transform.localScale.x, -head.transform.localScale.y, 1);
        }

        if(Input.GetKey(KeyCode.A) && isHeadFlip)
        {
            InverseHeadFlip();
            head.transform.localScale = new Vector3(head.transform.localScale.x, -head.transform.localScale.y, 1);
        }
        else if(Input.GetKey(KeyCode.D) && !isHeadFlip)
        {
            InverseHeadFlip();
            head.transform.localScale = new Vector3(head.transform.localScale.x, -head.transform.localScale.y, 1);
        }
    }

    public void SetTracking(bool tracking)
    {
        isTracking = tracking;
    }

    public void InverseHeadFlip()
    {
        isHeadFlip = !isHeadFlip;
    }
}
