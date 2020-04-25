using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBiometricSensorState
{
    None,
    DoorOpen,
    DoorClose,
    ItemDrop,
    AlertActivation,
    MoveBlock
}

public class BiometricSensor : MonoBehaviour
{
    [Header ("- 오브젝트 설정")][Tooltip ("생체인식에 연결할 오브젝트를 넣어주세요.")]
    [SerializeField] GameObject target = null;

    [Header ("- 오브젝트 종류 설정")][Tooltip ("None: 없음, DoorOpen: 문 개방, DoorClose: 문 폐쇄, ItemDrop: 아이템 획득, AlertActivation: 경보음 가동, MoveBlock: 움직이는 플랫폼")]
    [SerializeField] EBiometricSensorState state = EBiometricSensorState.None;

    [Header ("- 한 번만 실행")][Tooltip ("생체인식이 단 한번만 인식될 것인지, 시야에 들어올 때만 인식될 것인지 설정합니다.")]
    [SerializeField] bool isOnce = true;

    [Header ("- 인식 시간 (second)")][Tooltip ("isOnce가 true일 경우, 시야가 생체인식에 인식되는 시간을 설정합니다.")]
    [SerializeField] float recognitionTime = 2.0f;

    private bool isActive = false;
    private bool isReverse = false;

    void Start()
    {
        if(target.tag == "MoveBlock")
        {
            target.GetComponent<MoveBlock>().SetPause(true);
        }
        else if(target.tag == "Item")
        {
            target.SetActive(false);
        }

        if(!isOnce)
        {
            recognitionTime = 0.0f;
        }
    }

    void ProcessActionTarget()
    {
        switch (state)
        {
            case EBiometricSensorState.DoorOpen:
                target.GetComponent<Door>().SetState(!isReverse ? EDoorState.Opening : EDoorState.Closing);
                break;

            case EBiometricSensorState.DoorClose:
                target.GetComponent<Door>().SetState(!isReverse ? EDoorState.Closing : EDoorState.Opening);
                break;

            case EBiometricSensorState.ItemDrop:
                target.SetActive(!isReverse);
                break;

            case EBiometricSensorState.AlertActivation:
                target.SetActive(!isReverse);
                break;

            case EBiometricSensorState.MoveBlock:
                target.GetComponent<MoveBlock>().SetPause(isReverse);
                break;

            default:
                break;
        }
    }

    public void CheckStayPlayer()
    {
        Debug.Log("진입완료");
        
        // 생체인식 범위에 올라가 있어야만 상호작용함
        if(!isOnce)
        {
            isReverse = false;
        }

        // 생체인식 활성화되지 않았을 경우 인식 시작
        if(!isActive)
        {
            StartCoroutine("StartCheckPlayer");
        }
    }
    
    public void PlayerExitSensor()
    {
        // 인식 완료하기 전에 생체인식 범위에서 빠져나가면 취소됨
        if(!isActive)
        {
            Debug.Log("인식실패");
            StopCoroutine("StartCheckPlayer");
        }

        // 생체인식 범위에 올라가 있어야만 상호작용함
        if(!isOnce)
        {
            isActive = false;
            isReverse = true;
            
            ProcessActionTarget();
        }
    }

    IEnumerator StartCheckPlayer()
    {
        float time = recognitionTime;
        while(time > 0.0f)
        {
            time -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        isActive = true;
        ProcessActionTarget();
    }
}
