using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    [SerializeField] Animator animator;
    [Tooltip ("플레이어의 시야 오브젝트를 넣습니다.")]
    [SerializeField] GameObject eyeSight;
    [SerializeField] GameObject hatBone;
    [Tooltip ("플레이어의 머리부분 오브젝트를 넣습니다. Bone이 있을 경우, 머리에 해당되는 Bone을 넣습니다. (눈동자 움직임이 있어야 하는 경우 Head Bone 자식으로 설정)")]
    [SerializeField] GameObject headBone;
    Transform eyeBoneR;
    Transform monsterEyeBone;

    [Header ("- 마우스 트래킹 설정")]
    [Tooltip ("플레이어 캐릭터가 마우스를 쳐다볼 것인지 설정합니다.")]
    [SerializeField] bool isTracking = true;
    [Tooltip ("플레이어 캐릭터가 마우스를 쳐다보는 각도를 제한합니다. 캐릭터를 기준으로 < 위(0도)와 아래(180도) +- limitEyeSightRange / 2 >")]
    [SerializeField] float limitEyeSightRange = 50.0f;

    [Header ("- 시야 설정")][Tooltip ("게임이 시작될 때 플레이어 캐릭터가 바라보는 방향을 설정합니다.")]
    [SerializeField] bool isRightFront = true;
    [Tooltip("플레이어 캐릭터의 눈 깜빡임을 설정합니다.")]
    [SerializeField] bool useBlink = false;

    Light2D light2D;
    bool isHeadFlip = false;
    bool isBlinking = false;

    LimitAngle eyeLimitAngle;
    LimitAngle headLimitAngle;

    Vector3 mousePos;
    Vector3 playerPos;

    GameObject hitObj = null;
    bool alreadyHit = false;

    float innerAngle;
    float outerAngle;

    void Start() 
    {
        eyeLimitAngle.upRightAngle = 0.0f + limitEyeSightRange;
        eyeLimitAngle.upLeftAngle = 0.0f + limitEyeSightRange;
        eyeLimitAngle.downRightAngle = 180.0f - limitEyeSightRange;
        eyeLimitAngle.downLeftAngle = 180.0f - limitEyeSightRange;
        headLimitAngle.upRightAngle = 0.0f + limitEyeSightRange;
        headLimitAngle.upLeftAngle = 0.0f - limitEyeSightRange;
        headLimitAngle.downRightAngle = -90.0f + limitEyeSightRange;
        headLimitAngle.downLeftAngle = 90.0f - limitEyeSightRange;

        eyeBoneR = headBone.transform.GetChild(1);
        monsterEyeBone = hatBone.transform.GetChild(0);
        light2D = eyeSight.GetComponent<Light2D>();

        innerAngle = light2D.pointLightInnerAngle;
        outerAngle = light2D.pointLightOuterAngle;
    }

    void Update()
    {
        if (useBlink && isBlinking == false)
        {
            StartCoroutine("StartBlink");
        }

        if (Input.GetMouseButton(1))
        {
            useBlink = false;
            animator.SetBool("Blink", true);
            light2D.pointLightInnerAngle = 0.0f;
            light2D.pointLightOuterAngle = 0.0f;
        }

        if (Input.GetMouseButtonUp(1))
        {
            useBlink = true;
            animator.SetBool("Blink", false);
            light2D.pointLightInnerAngle = innerAngle;
            light2D.pointLightOuterAngle = outerAngle;
        }

        Vector3 rightPos = eyeSight.transform.position;
        rightPos.x += 0.3f;

        Vector3 leftPos = eyeSight.transform.position;
        leftPos.x -= 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(isRightFront ? rightPos : leftPos, mousePos - playerPos, 8);
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
                if (hit.collider.CompareTag("BiometricSensor"))
                {
                    hitObj.GetComponent<BiometricSensor>().CheckStayPlayer();
                }

                Debug.Log("시야 안에 들어온 물체: " + hit.collider.gameObject.name);
            }
        }

        // 디버깅용
        Debug.DrawRay(isRightFront ? rightPos : leftPos, mousePos - playerPos, Color.red);
    }
    
    void FixedUpdate()
    {
        if(isTracking)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane xy = new Plane(Vector3.forward, Vector3.zero);

            float distance;
            xy.Raycast(ray, out distance);
            mousePos = ray.GetPoint(distance);
            playerPos = this.transform.position;

            SetPlayerHeadFront();
            TrackingMouse();
        }
    }

    void TrackingMouse()
    {
        Vector2 structPos = new Vector2(mousePos.x - playerPos.x, mousePos.y - playerPos.y);

        float rad = Mathf.Atan2(structPos.x, structPos.y);
        float mouseRotate = (rad * 180) / Mathf.PI;
        float unityEyeAngle = -mouseRotate;
        float unityHeadAngle = -mouseRotate - 90;
        float convertToEyeAngle = ConvertToAngle(unityEyeAngle);
        float convertToHeadAngle = ConvertToHeadAngle(unityHeadAngle);

        // 머리 회전
        headBone.transform.localEulerAngles = new Vector3(0, 0, isRightFront ? -(convertToHeadAngle * 0.15f) : convertToHeadAngle * 0.15f);

        // 눈 회전
        for (int idx = 0; idx < headBone.transform.childCount; ++idx)
        {
            Transform eyeBone = headBone.transform.GetChild(idx);
            if (eyeBone == null || eyeBone.gameObject.activeInHierarchy == false)
                continue;

            eyeBone.localEulerAngles = new Vector3(0, 0, convertToEyeAngle);
        }

        monsterEyeBone.localEulerAngles = new Vector3(0, 0, convertToEyeAngle);

        // 시야 회전
        if (eyeBoneR == null || eyeBoneR.gameObject.activeInHierarchy == false)
        {
            Debug.Log("우측 눈을 찾지 못하였습니다. (연결이 되지 않았거나, 꺼져있음)");
            return;
        }

        eyeSight.transform.localEulerAngles = isRightFront ? -monsterEyeBone.localEulerAngles : monsterEyeBone.localEulerAngles;
        eyeSight.transform.position = monsterEyeBone.position;
    }

    float ConvertToAngle(float unityAngle)
    {
        float eyeAngle = isRightFront ? -unityAngle : unityAngle;

        // 우상단 제한
        if (0.0f < eyeAngle && eyeLimitAngle.upRightAngle > eyeAngle)
        {
            return eyeLimitAngle.upRightAngle;
        }
        // 좌상단 제한
        else if(0.0f < eyeAngle && eyeLimitAngle.upLeftAngle > eyeAngle)
        {
            return eyeLimitAngle.upLeftAngle;
        }
        // 우하단 제한
        else if(180.0f > eyeAngle && eyeLimitAngle.downRightAngle < eyeAngle)
        {
            return eyeLimitAngle.downRightAngle;
        }
        // 좌하단 제한
        else if(180.0f > eyeAngle && eyeLimitAngle.downLeftAngle < eyeAngle)
        {
            return eyeLimitAngle.downLeftAngle;
        }
        else
        {
            return eyeAngle;
        }
    }

    float ConvertToHeadAngle(float unityHeadAngle)
    {
        float rightFrontAngle = isRightFront ? (isHeadFlip ? -180 :180) : 0;
        float headAngle = isHeadFlip ? unityHeadAngle - rightFrontAngle : unityHeadAngle + rightFrontAngle;

        // 우상단 제한
        if (0.0f > headAngle && headLimitAngle.upRightAngle < headAngle)
        {
            return headLimitAngle.upRightAngle;
        }
        // 좌상단 제한
        else if(0.0f < headAngle && headLimitAngle.upLeftAngle > headAngle)
        {
            return headLimitAngle.upLeftAngle;
        }
        // 우하단 제한
        else if(-90.0f < headAngle && headLimitAngle.downRightAngle > headAngle)
        {
            return headLimitAngle.downRightAngle;
        }
        // 좌하단 제한
        else if(90.0f > headAngle && headLimitAngle.downLeftAngle < headAngle)
        {
            return headLimitAngle.downLeftAngle;
        }
        else
        {
            return headAngle;
        }
    }

    void SetPlayerHeadFront()
    {
        if (Input.GetKey(KeyCode.A) && isHeadFlip)
        {
            InverseHeadFlip();
            TurnHead();
        }
        else if (Input.GetKey(KeyCode.D) && !isHeadFlip)
        {
            InverseHeadFlip();
            TurnHead();
        }

        if (isRightFront && mousePos.x < playerPos.x)
        {
            isRightFront = false;
            TurnHead();
        }
        else if(!isRightFront && mousePos.x > playerPos.x)
        {
            isRightFront = true;
            TurnHead();
        }
    }

    void TurnHead()
    {
        Transform neckBone = headBone.transform.parent;
        if (neckBone.gameObject.activeInHierarchy)
        {
            hatBone.transform.localScale = new Vector3(hatBone.transform.localScale.x, -hatBone.transform.localScale.y, 1);
            neckBone.transform.localScale = new Vector3(neckBone.transform.localScale.x, -neckBone.transform.localScale.y, 1);
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

    IEnumerator StartBlink()
    {
        isBlinking = true;
        float time = 0.0f;
        while (time < 2.0f)
        {
            time += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // 도중에 눈 깜빡임이 꺼질 경우
        if (useBlink == false)
        {
            isBlinking = false;

            yield break;
        }

        float intervalAngle = innerAngle / 3;

        animator.SetBool("Blink", true);
        // 눈 깜빡일 때 시야 라이트도 같이 줄어들고 커짐
        while (time <= 2.05f)
        {
            if (time < 2.03f)
            {
                light2D.pointLightInnerAngle -= intervalAngle;
                light2D.pointLightOuterAngle -= intervalAngle;
            }
            else
            {
                light2D.pointLightInnerAngle += intervalAngle;
                light2D.pointLightOuterAngle += intervalAngle;
            }
            time += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        
        // 시야 라이트 각도 원상복구
        light2D.pointLightInnerAngle = innerAngle;
        light2D.pointLightOuterAngle = outerAngle;
        animator.SetBool("Blink", false);

        if (useBlink)
        {
            StartCoroutine("StartBlink");
        }
    }
}
