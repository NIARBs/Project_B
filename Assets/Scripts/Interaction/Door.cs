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
    [SerializeField] bool isOpen = false;
    [SerializeField] float moveSpeed = 1f;

    [SerializeField] EDoorState state = EDoorState.Idle;

    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    [SerializeField] GameObject gate;
    
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
