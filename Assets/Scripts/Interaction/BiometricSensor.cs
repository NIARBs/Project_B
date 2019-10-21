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
    [SerializeField] bool isOnce = true;

    [SerializeField] EBiometricSensorState state = EBiometricSensorState.None;

    [SerializeField] GameObject target;
    [SerializeField] GameObject sensingObject;

    [SerializeField] float recognitionTime = 2.0f;

    private bool isActive = false;

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
    }

    void ProcessActionTarget()
    {
        switch (state)
        {
            case EBiometricSensorState.DoorOpen:
                target.GetComponent<Door>().SetState(EDoorState.Opening);
                break;

            case EBiometricSensorState.DoorClose:
                target.GetComponent<Door>().SetState(EDoorState.Closing);
                break;

            case EBiometricSensorState.ItemDrop:
                target.SetActive(true);
                break;

            case EBiometricSensorState.AlertActivation:
                break;

            case EBiometricSensorState.MoveBlock:
                target.GetComponent<MoveBlock>().SetPause(false);
                break;

            default:
                break;
        }
    }

    public void CheckStayPlayer()
    {
        Debug.Log("진입완료");
        
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
