using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDoorState
{
    Idle,
    Opening,
    Closing
}

public class Door : MonoBehaviour
{
    [Header ("- 오브젝트 설정")][Tooltip ("좌측 문 오브젝트를 넣어주세요.")]
    [SerializeField] GameObject leftDoor = null;
    [Tooltip ("우측 문 오브젝트를 넣어주세요.")]
    [SerializeField] GameObject rightDoor = null;
    [Tooltip ("게이트 오브젝트를 넣어주세요.")]
    [SerializeField] GameObject gate = null;

    [Header ("- 속성 설정")][Tooltip ("idle: 기본 상태, Opening: 문 열리는 중, Closing: 문 닫히는 중")]
    [SerializeField] EDoorState state = EDoorState.Idle;

    [Space][Tooltip ("시작할 때 문이 열려있는지 닫혀있는지 설정합니다.")]
    [SerializeField] bool isOpen = false;
    [Tooltip ("문이 열리는 속도를 설정합니다.")]
    [SerializeField] float moveSpeed = 1.0f;

    private float closeLeftXCoord;
    private float closeRightXCoord;

    private float openLeftXCoord;
    private float openRightXCoord;

    void Start()
    {
        closeLeftXCoord = leftDoor.transform.position.x;
        closeRightXCoord = rightDoor.transform.position.x;
        openLeftXCoord = leftDoor.transform.position.x - 1.0f;
        openRightXCoord = rightDoor.transform.position.x + 1.0f;

        Vector3 leftPosition = leftDoor.transform.position;
        Vector3 rightPosition = rightDoor.transform.position;
        if(isOpen)
        {
            leftDoor.transform.position = new Vector3(openLeftXCoord, leftPosition.y, leftPosition.z);
            rightDoor.transform.position = new Vector3(openRightXCoord, rightPosition.y, rightPosition.z);
        }
        else
        {
            leftDoor.transform.position = new Vector3(closeLeftXCoord, leftPosition.y, leftPosition.z);
            rightDoor.transform.position = new Vector3(closeRightXCoord, rightPosition.y, rightPosition.z);
        }
    }

    void FixedUpdate()
    {
        if(state == EDoorState.Idle)
        {
            return;
        }

        if(state == EDoorState.Opening)
        {
            OpeningDoor();
        }
        else if(state == EDoorState.Closing)
        {
            ClosingDoor();
        }
    }

    void OpeningDoor()
    {
        if(openLeftXCoord < leftDoor.transform.position.x)
        {
            leftDoor.transform.position += Vector3.left * moveSpeed * 0.01f;
        }

        if(openRightXCoord > rightDoor.transform.position.x)
        {
            rightDoor.transform.position += Vector3.right * moveSpeed * 0.01f;
        }
        else
        {
            state = EDoorState.Idle;
            gate.SetActive(true);
        }
    }

    void ClosingDoor()
    {
        if(closeLeftXCoord > leftDoor.transform.position.x)
        {
            leftDoor.transform.position += Vector3.right * moveSpeed * 0.01f;
        }

        if(closeRightXCoord < rightDoor.transform.position.x)
        {
            rightDoor.transform.position += Vector3.left * moveSpeed * 0.01f;
        }
        else
        {
            state = EDoorState.Idle;
            gate.SetActive(false);
        }
    }

    public void SetState(EDoorState doorState)
    {
        state = doorState;
    }

    public EDoorState GetState()
    {
        return state;
    }
}
