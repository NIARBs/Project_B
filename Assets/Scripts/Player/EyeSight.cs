﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class EyeSight : MonoBehaviour
{
    [Header ("- 오브젝트 설정")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject head;
    [SerializeField] GameObject eyeSight;

    [Header ("- 마우스 트래킹 사용여부")]
    [SerializeField] bool isTracking = true;

    [Header ("- 마우스 비트래킹 각도")]
    [SerializeField] float limitEyeSightRange = 50.0f;

    private float upRightAngle;
    private float upLeftAngle;
    private float downRightAngle;
    private float downLeftAngle;

    private bool isRightFrontHead = true;

    private Vector3 mousePos;
    private Vector3 playerPos;

    private GameObject hitObj = null;
    private bool alreadyHit = false;

    void Start() 
    {
        upRightAngle = 90.0f - limitEyeSightRange;
        upLeftAngle = 90.0f + limitEyeSightRange;
        downRightAngle = -90.0f + limitEyeSightRange;
        downLeftAngle = 270.0f - limitEyeSightRange;
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
