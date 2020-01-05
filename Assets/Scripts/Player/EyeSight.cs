using System.Collections;
using System.Collections.Generic;
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

    private Vector3 mousePos;
    private Vector3 playerPos;

    void Start() {
        upRightAngle = 90.0f - limitEyeSightRange;
        upLeftAngle = 90.0f + limitEyeSightRange;
        downRightAngle = -90.0f + limitEyeSightRange;
        downLeftAngle = 270.0f - limitEyeSightRange;
    }
    void FixedUpdate()
    {
        if(isTracking)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
