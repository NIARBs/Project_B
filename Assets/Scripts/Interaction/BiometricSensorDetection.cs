using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiometricSensorDetection : MonoBehaviour
{
    [SerializeField] GameObject biometricSensor = null;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("플레이어 생체인식 시작");

            biometricSensor.GetComponent<BiometricSensor>().CheckStayPlayer();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("플레이어 생체인식 종료");

            biometricSensor.GetComponent<BiometricSensor>().PlayerExitSensor();
        }
    }
}
